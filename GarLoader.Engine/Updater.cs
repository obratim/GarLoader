using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GarLoader.Engine
{
	public class Updater
	{
		private readonly IUploader _uploader;

		private readonly ILogger _logger;

		private readonly UpdaterConfiguration _updaterConfiguration;

        public Updater(IUploader uploader, ILogger<Updater> logger, UpdaterConfiguration updaterConfiguration)
        {
            _uploader = uploader;
            _logger = logger;
            _updaterConfiguration = updaterConfiguration;

            _logger.LogInformation("Запущена программа обновления БД ФИАС");
        }

        public void Update()
        {
			try
			{
				_logger.LogInformation("Выполняется процесс загрузки");
				
				if (!System.IO.Directory.Exists(_updaterConfiguration.ArchivesDirectory))
					System.IO.Directory.CreateDirectory(_updaterConfiguration.ArchivesDirectory);
				foreach (var archPath in System.IO.Directory.GetFiles(_updaterConfiguration.ArchivesDirectory))
					System.IO.File.Delete(archPath);

				if (string.IsNullOrEmpty(_updaterConfiguration.GarFullPath))
				{
					var updates = DownloadsArray().Result;
					_logger.LogInformation("Получены сведения ссылки на загрузку архивов: " + updates.Count);

					var newestUpdate = updates
						.Where(x => x.ParsedDate != null && !string.IsNullOrEmpty(x.GarXMLFullURL))
						.OrderByDescending(x => x.ParsedDate)
						.FirstOrDefault();
					
					if (newestUpdate == null)
					{
						_logger.LogError("Не удалось получить информацию об обновлениях");
						throw new Exception("Не удалось получить информацию об обновлениях");
					}

					_logger.LogInformation("Самая актуальная версия для загрузки: " + System.Text.Json.JsonSerializer.Serialize(newestUpdate));

					Update(newestUpdate);
				}
				else
					Update(default);
				
				if (_updaterConfiguration.DeleteArchiveFile)
					System.IO.File.Delete(_updaterConfiguration.GarFullPath);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Не удалось выполнить обновление");
			}
			finally
			{
				_logger.LogInformation("Завершение работы программы");
				_uploader.CleanUp();
			}
		}
		
		private void Update(DownloadFileInfo downloadFileInfo)
		{
			var processDt = DateTime.Now;

			if (string.IsNullOrEmpty(_updaterConfiguration.GarFullPath))
			{
				_logger.LogInformation($"Загрузка обновления {downloadFileInfo.VersionId} ({downloadFileInfo.TextVersion})");

				_logger.LogInformation($"Скачивание файла {downloadFileInfo.GarXMLFullURL} ...");
				using (var client = new System.Net.WebClient())
				{
					_updaterConfiguration.GarFullPath = System.IO.Path.Combine(_updaterConfiguration.ArchivesDirectory, $"gar {downloadFileInfo.VersionId}.zip");

					var t = client.DownloadFileTaskAsync(
						downloadFileInfo.GarXMLFullURL,
						_updaterConfiguration.GarFullPath);
					t.Wait(_updaterConfiguration.ArchiveDownloadTimeoutValue.Milliseconds);
					if (!t.IsCompleted) throw new TimeoutException("Истекло время ожидания скачивания архива с данными");
					_logger.LogInformation($"Загружено {(new System.IO.FileInfo(_updaterConfiguration.GarFullPath).Length / 1024.0 / 1024.0):0.00} МиБ");
				}
				_logger.LogInformation("Архив с данными загружен: " + _updaterConfiguration.GarFullPath);
			}
			else
				_logger.LogInformation("Будет использован архив с данными: " + _updaterConfiguration.GarFullPath);
			
			using var archiveFile = System.IO.File.OpenRead(_updaterConfiguration.GarFullPath);
			using var arch = new System.IO.Compression.ZipArchive(archiveFile);
			_logger.LogInformation("Вставка данных ...");

			LoadGlobalEntry<AddressObjectType>(arch, "AS_ADDR_OBJ_TYPES_");
			LoadGlobalEntry<ObjectLevel>(arch, "AS_OBJECT_LEVELS_");
			LoadGlobalEntry<OperationType>(arch, "AS_OPERATION_TYPES_");
			LoadGlobalEntry<ParamType>(arch, "AS_PARAM_TYPES_");

			_logger.LogInformation("Справочные данные загружены");

			LoadRegionEntryInParallel<AddressObject>(_updaterConfiguration.GarFullPath, "AS_ADDR_OBJ_", (item, region) => { item.Region = region; return item; });
			LoadRegionEntryInParallel<Parameter>(_updaterConfiguration.GarFullPath, "AS_ADDR_OBJ_PARAMS_");
			LoadRegionEntryInParallel<AdministrativeHierarchyItem>(_updaterConfiguration.GarFullPath, "AS_ADM_HIERARCHY_");
			LoadRegionEntryInParallel<MunicipalHierarchyItem>(_updaterConfiguration.GarFullPath, "AS_MUN_HIERARCHY_");

			Complete();
		}

		private static IEnumerable<T> GetObjectsFromXmlReader<T>(System.IO.Stream entry, Func<T, T> prepareItem = null)
		{
			prepareItem ??= (x => x);
			using (var xr = XmlReader.Create(entry, Helpers.XmlSettings))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
				var name = ((typeof(T).GetCustomAttributes(
					typeof(System.Xml.Serialization.XmlRootAttribute), true)
					.SingleOrDefault() as System.Xml.Serialization.XmlRootAttribute)
					?? throw new UpdateException("Некорректный тип для сериализации"))
					.ElementName;
				xr.MoveToContent();
				while (xr.Read())
				{
					while (true)
					{
						if (!(xr.NodeType == XmlNodeType.Element && xr.Name == name)) break;
					    if (serializer.Deserialize(xr) is T value)
							yield return prepareItem(value);
						else break;
					}
				}
			}
		}

		private async IAsyncEnumerable<DownloadFileInfo> Downloads()
		{
			using var client = new System.Net.Http.HttpClient();
			var data = await client.GetAsync(_updaterConfiguration.ServiceUri);
			if (!data.IsSuccessStatusCode)
				throw new Exception("Не удалось загрузить.\n" + data.StatusCode + "\n" + await data.Content.ReadAsStringAsync());
			foreach (var item in (await data.Content.ReadFromJsonAsync<IEnumerable<DownloadFileInfo>>()))
				yield return item;
		}

		private async Task<List<DownloadFileInfo>> DownloadsArray()
		{
			return await Downloads().ToListAsync();
		}

		private void LoadGlobalEntry<T>(System.IO.Compression.ZipArchive arch, string entryBeginingSubname, Func<T, T> prepareItem = null)
		{
			var entry = arch.Entries.FirstOrDefault(e => e.FullName.StartsWith(entryBeginingSubname));
			if (entry == null)
			{
				_logger.LogWarning("Отсутствуют справочные данные " + entryBeginingSubname);
				return;
			}
			using var stream = entry.Open();
			_uploader.InsertAddressObjectItems(GetObjectsFromXmlReader<T>(stream, prepareItem));
		}

		private void LoadRegionEntry<T>(System.IO.Compression.ZipArchive archive, string entryBeginingSubname, Func<T, int, T> prepareItem = null)
		{
			for (var i = 1; i <= _updaterConfiguration.RegionsCountValue; ++i)
			{
				_logger.LogInformation($"Вставка объектов {entryBeginingSubname} по региону {i}...");
				Func<T, T> converter = prepareItem == null ? null : (x => prepareItem(x, i));
				LoadGlobalEntry<T>(archive, $"{i:00}/{entryBeginingSubname}", converter);
			}
		}

		private void LoadRegionEntryInParallel<T>(string archivePath, string entryBeginingSubname, Func<T, int, T> prepareItem = null)
		{
			Enumerable
				.Range(1, _updaterConfiguration.RegionsCountValue)
				.AsParallel()
				.ForAll(i => {
					using var archiveFile = System.IO.File.OpenRead(_updaterConfiguration.GarFullPath);
					using var arch = new System.IO.Compression.ZipArchive(archiveFile);
					Func<T, T> converter = prepareItem == null ? null : (x => prepareItem(x, i));
					LoadGlobalEntry<T>(arch, $"{i:00}/{entryBeginingSubname}", converter);
					_logger.LogInformation($"Вставлены данные из объектов {entryBeginingSubname} по региону {i}");
				});
		}

		private void Complete()
		{
			_uploader.CleanUp();
		}
	}
}
