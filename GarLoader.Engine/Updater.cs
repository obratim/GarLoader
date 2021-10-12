using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
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

		private readonly IHttpClientFactory _httpFactory;

        public Updater(IUploader uploader, ILogger<Updater> logger, UpdaterConfiguration updaterConfiguration, IHttpClientFactory httpClientFactory)
        {
            _uploader = uploader;
            _logger = logger;
            _updaterConfiguration = updaterConfiguration;
			_httpFactory = httpClientFactory;

            _logger.LogInformation("Запущена программа обновления БД ФИАС");
        }

        public async Task Update(System.Threading.CancellationToken token)
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

					await Update(newestUpdate, token);
				}
				else
					await Update(default, token);
				
				if (_updaterConfiguration.DeleteArchiveFile)
					System.IO.File.Delete(_updaterConfiguration.GarFullPath);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Не удалось выполнить обновление");
			}
			finally
			{
				_logger.LogInformation("Загрузка завершена");
			}
		}
		
		private async Task Update(DownloadFileInfo downloadFileInfo, System.Threading.CancellationToken c)
		{
			var processDt = DateTime.Now;

			if (string.IsNullOrEmpty(_updaterConfiguration.GarFullPath))
			{
				_logger.LogInformation($"Загрузка обновления {downloadFileInfo.VersionId} ({downloadFileInfo.TextVersion})");

				_logger.LogInformation($"Скачивание файла {downloadFileInfo.GarXMLFullURL} ...");
				using var client = _httpFactory.CreateClient();
				client.Timeout = _updaterConfiguration.ArchiveDownloadTimeoutValue;
				_updaterConfiguration.GarFullPath = System.IO.Path.Combine(_updaterConfiguration.ArchivesDirectory, $"gar {downloadFileInfo.VersionId}.zip");

				await using var fileStream = System.IO.File.OpenWrite(_updaterConfiguration.GarFullPath);
				using var downloadTask = await client.SendAsync(new HttpRequestMessage
					{
						Method = HttpMethod.Get,
						RequestUri = new Uri(downloadFileInfo.GarXMLFullURL),
					},
					HttpCompletionOption.ResponseHeadersRead,
					c);
				var downloadLength = downloadTask.Content.Headers.ContentLength;
				_logger.LogDebug("Проверка результата запроса");
				downloadTask.EnsureSuccessStatusCode();
				_logger.LogDebug("Положительный результат запроса");
				var prevPersent = 0.0;
				await CopyAsync(
					await downloadTask.Content.ReadAsStreamAsync(),
					fileStream,
					1024 * 1024,
					c,
					(readen, length) => {
						length = length ?? downloadLength;
						if (length == null)
						{
							_logger.LogInformation($"Скачано {readen/1024.0/1024.0:0.00} МБ");
							return;
						}
						var persent = 100.0 * readen / length.Value;
						if (persent - prevPersent >= 3)
						{
							_logger.LogInformation($"Скачано {persent:0.00}%; {readen/1024.0/1024.0:0.00} из {length.Value/1024.0/1024.0:0.00} МБ");
							prevPersent = persent;
						}
					});

				_logger.LogInformation($"Загружено {(new System.IO.FileInfo(_updaterConfiguration.GarFullPath).Length / 1024.0 / 1024.0):0.00} МиБ");
				
				_logger.LogInformation($"Загружено {(new System.IO.FileInfo(_updaterConfiguration.GarFullPath).Length / 1024.0 / 1024.0):0.00} МиБ");
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

		private static async Task CopyAsync(Stream from, Stream to, int bufferSize, CancellationToken token, Action<long, long?> progress)
		{
			long? srcLength = null;
			try
			{
				srcLength = from.Length;
			}
			catch
			{}
			Memory<byte> buf = new byte[bufferSize];
			int readen = 0;
			long totalReaden = 0;
			int i = 0;
			while((readen = await from.ReadAsync(buf, token)) > 0)
			{
				totalReaden += readen;
				if (token.IsCancellationRequested)
					return;
				await to.WriteAsync(buf.Slice(0, readen), token);
				if (++i >= 1000)
				{
					i = 0;
					progress(totalReaden, srcLength);
				}
			}
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
