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

		private readonly Lazy<HashSet<Guid>> _addressObjectGuids;
		private HashSet<Guid> AddressObjectGuids => _addressObjectGuids.Value;

		private readonly ILogger _logger;
        //public static log4net.ILog Logger => Helpers.Logger.Value;

		private readonly IOptions<UpdaterConfiguration> _configurationSnapshot;
		private UpdaterConfiguration _configurationFromArguments;
		private UpdaterConfiguration _updaterConfiguration;
		private UpdaterConfiguration UpdaterConfiguration => _updaterConfiguration ??= _configurationFromArguments.Combine(_configurationSnapshot.Value);

        public Updater(IUploader uploader, ILogger<Updater> logger, IOptions<UpdaterConfiguration> configurationSnapshot, UpdaterConfiguration configurationFromArguments)
        {
            _uploader = uploader;
            _logger = logger;
            _configurationSnapshot = configurationSnapshot;
            _configurationFromArguments = configurationFromArguments;
            _addressObjectGuids = new Lazy<HashSet<Guid>>(() => new HashSet<Guid>(_uploader.GetAddressObjectGuids()));

            _logger.LogInformation("Запущена программа обновления БД ФИАС");
        }

        public void Update()
        {
			// 0. если в папке с архивами лежит старый архив, значит предыдущие обновление завершилось неуспешно. выходим
			// 0.1 восстанавливаем структуру БД: проверяем создана ли табл с историей обновлений, пересоздаём временные таблицы
			// 1. попробовать получить id последнего загруженного обновления ФИАС
			// 1.1 если не было ни одного, получаем список всех обновлений, находим самый ранний более новый, если кроме него есть более ранние - переходим к п.2, если нет - выходим с ошибкой, т.к. возможно пропущены обновления и лучше перезагрузить всё
			// 1.2 получаем метаданные о последнем обновлении от фиас
			// 1.2.1 если № последнего обновления больше последнего загруженного на 1, закачиваем
			// 1.2.2 если разница больше одного, получаем все метаданные, находим самый старый более новый, так же проверяем чтобы он не был вообще самым старым
			// 2. скачиваем архив с обновлениями по ссылке
			// 3. создаём временные таблицы
			// 4. получаем коды необходимых в конкретной конфигурации адресных объектов
			// 5. по очереди загружаем необходимые объекты во временные таблицы
			// 6. копируем информацию из временных таблиц в основную БД
			// 7. удаляем временные таблицы, удаляем архив

			var svc = new FiasReference.DownloadServiceSoapClient(
				FiasReference.DownloadServiceSoapClient.EndpointConfiguration.DownloadServiceSoap12,
				UpdaterConfiguration.ServiceUri);
			try
			{
				_logger.LogInformation("Выполняется процесс загрузки");
				
				// if (System.IO.Directory.GetFiles(UpdaterConfiguration.ArchivesDirectory).Length > 0)
				// 	throw new PreviousUpdateFailedException("Последняя попытка обновления завершилась неудачно. Остались старые файлы");
				if (!System.IO.Directory.Exists(UpdaterConfiguration.ArchivesDirectory))
					System.IO.Directory.CreateDirectory(UpdaterConfiguration.ArchivesDirectory);
				foreach (var archPath in System.IO.Directory.GetFiles(UpdaterConfiguration.ArchivesDirectory))
					System.IO.File.Delete(archPath);

				if (string.IsNullOrEmpty(UpdaterConfiguration.GarFullPath))
				{
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
			}
			catch (Exception e)
			{
				_logger.LogError("Не удалось выполнить обновление", e);
			}
			finally
			{
				((System.ServiceModel.ICommunicationObject)svc).Close();
				_logger.LogInformation("Завершение работы программы");
				_uploader.CleanUp();
			}
		}
		
		private void Update(DownloadFileInfo downloadFileInfo)
		{
			var processDt = DateTime.Now;

			if (string.IsNullOrEmpty(UpdaterConfiguration.GarFullPath))
			{
				_logger.LogInformation($"Загрузка обновления {downloadFileInfo.VersionId} ({downloadFileInfo.TextVersion})");

				_logger.LogInformation($"Скачивание файла {downloadFileInfo.GarXMLFullURL} ...");
				using (var client = new System.Net.WebClient())
				{
					UpdaterConfiguration.GarFullPath = System.IO.Path.Combine(UpdaterConfiguration.ArchivesDirectory, $"gar {downloadFileInfo.VersionId}.zip");

					var t = client.DownloadFileTaskAsync(
						downloadFileInfo.GarXMLFullURL,
						UpdaterConfiguration.GarFullPath);
					t.Wait(UpdaterConfiguration.ArchiveDownloadTimeoutValue.Milliseconds);
					if (!t.IsCompleted) throw new TimeoutException("Истекло время ожидания скачивания архива с данными");
					_logger.LogInformation($"Загружено {(new System.IO.FileInfo(UpdaterConfiguration.GarFullPath).Length / 1024.0 / 1024.0):0.00} МиБ");
				}
				_logger.LogInformation("Архив с данными загружен: " + UpdaterConfiguration.GarFullPath);
			}
			else
				_logger.LogInformation("Будет использован архив с данными: " + UpdaterConfiguration.GarFullPath);
			
			_uploader.InitializeTempTables();
			_logger.LogInformation("Созданы временные таблицы");

			/*using (var arch = SharpCompress.Archives.Rar.RarArchive.Open(archPath))
			{
				List<FiasTypes.AddressObject> addressObjectsToDelete = new List<FiasTypes.AddressObject>();
				List<FiasTypes.House> housesToDelete = new List<FiasTypes.House>();
				List<FiasTypes.HouseInterval> houseIntervalsToDelete = new List<FiasTypes.HouseInterval>();
				List<FiasTypes.Landmark> landmarksToDelete = new List<FiasTypes.Landmark>();

				foreach (var entry in arch.Entries)
				{
					_logger.Debug($"Вложение {entry.Key}");

					switch (entry.Key)
					{
						case string entryName when Regex.IsMatch(entryName, "^AS_ADDROBJ_.*"):
							_logger.Debug($"Загрузка адресных объектов во временную таблицу ...");
							var objects = GetObjectsFromXmlReader<FiasTypes.AddressObject>(entry)
									.Where(x => x.RegionCode == UpdaterConfiguration.RegionNumber)
									.ToArray();

							_uploader.PutAddressObjectsChanges(entryName, objects);

							foreach (var x in objects)
								AddressObjectGuids.Add(x.Id);

							_logger.Debug($"Адресные объекты загружены во временную таблицу ({objects.Length} записей)");
							break;

						case string entryName when Regex.IsMatch(entryName, "^AS_HOUSE_.*"):
							_logger.Debug($"Загрузка домов во временную таблицу ...");
							var n = 0;
							_uploader.PutHousesChanges(
								entryName,
								GetObjectsFromXmlReader<FiasTypes.House>(entry)
									.FilterObjects(AddressObjectGuids)
									.Select(x => { ++n; return x; }));
							_logger.Debug($"Дома загружены во временную таблицу ({n} записей)");
							break;

						case string entryName when Regex.IsMatch(entryName, "^AS_HOUSEINT_.*"):
							_logger.Debug($"Загрузка диапазонов домов во временную таблицу ...");
							n = 0;
							_uploader.PutHouseIntsChanges(
								entryName,
								GetObjectsFromXmlReader<FiasTypes.HouseInterval>(entry)
									.FilterObjects(AddressObjectGuids)
									.Select(x => { ++n; return x; }));
							_logger.Debug($"Диапазоны домов загружены во временную таблицу ({n} записей)");
							break;

						case string entryName when Regex.IsMatch(entryName, "^AS_LANDMARK.*"):
							_logger.Debug($"Загрузка мест расположения имущественных объектов во временную таблицу ...");
							n = 0;
							_uploader.PutLandmarksChanges(
								entryName,
								GetObjectsFromXmlReader<FiasTypes.Landmark>(entry)
									.FilterObjects(AddressObjectGuids)
									.Select(x => { ++n; return x; }));
							_logger.Debug($"Места расположения загружены во временную таблицу ({n} записей)");
							break;


						case string entryName when Regex.IsMatch(entryName, "^AS_DEL_ADDROBJ_.*"):
							_logger.Debug($"Загрузка удаляемых адресных объектов ...");
							addressObjectsToDelete.AddRange(
								GetObjectsFromXmlReader<FiasTypes.AddressObject>(entry)
									.Where(x => x.RegionCode == UpdaterConfiguration.RegionNumber));
							_logger.Debug($"Устаревшие адресные объекты будут удалены ({addressObjectsToDelete.Count} записей)");
							break;

						case string entryName when Regex.IsMatch(entryName, "^AS_DEL_HOUSE_.*"):
							_logger.Debug($"Загрузка удаляемых домов ...");
							housesToDelete.AddRange(
								GetObjectsFromXmlReader<FiasTypes.House>(entry)
									.FilterObjects(AddressObjectGuids));
							_logger.Debug($"Устаревшие дома будут удалены ({housesToDelete.Count} записей)");
							break;

						case string entryName when Regex.IsMatch(entryName, "^AS_DEL_HOUSEINT_.*"):
							_logger.Debug($"Загрузка удаляемых диапазонов домов ...");
							houseIntervalsToDelete.AddRange(
								GetObjectsFromXmlReader<FiasTypes.HouseInterval>(entry)
									.FilterObjects(AddressObjectGuids));
							_logger.Debug($"Устаревшие диапазоны домов будут удалены ({houseIntervalsToDelete.Count} записей)");
							break;

						case string entryName when Regex.IsMatch(entryName, "^AS_DEL_LANDMARK_.*"):
							_logger.Debug($"Загрузка удаляемых мест расположения имущественных объектов ...");
							landmarksToDelete.AddRange(
								GetObjectsFromXmlReader<FiasTypes.Landmark>(entry)
									.FilterObjects(AddressObjectGuids));
							_logger.Debug($"Устаревшие места расположения будут удалены ({landmarksToDelete.Count} записей)");
							break;

						default:
							_logger.Debug($"Не обрабатывается, переход к следующему вложению");
							break;
					}
				}

				_logger.Debug("Запись данных в основные таблицы ...");
				_uploader.UpdateDb(
					downloadFileInfo.VersionId,
					downloadFileInfo.TextVersion,
					processDt,
					downloadFileInfo.FiasCompleteXmlUrl,
					downloadFileInfo.FiasDeltaXmlUrl,
					addressObjectsToDelete,
					housesToDelete,
					houseIntervalsToDelete,
					landmarksToDelete);
				_logger.Info($"База данных обновлена до версии {downloadFileInfo.VersionId} ({downloadFileInfo.TextVersion})");
			}
			System.IO.File.Delete(archPath);
			_logger.Debug("Архив с данными удалён");
		}

		private static IEnumerable<T> GetObjectsFromXmlReader<T>(SharpCompress.Archives.Rar.RarArchiveEntry entry)
			where T : class
		{
			using (var s = entry.OpenEntryStream())
			using (var xr = XmlReader.Create(s, Helpers.XmlSettings))
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
					    if (serializer.Deserialize(xr) is T value) yield return value;
						else break;
					}
				}
			}*/
		}

		private async IAsyncEnumerable<DownloadFileInfo> Downloads()
		{
			using var client = new System.Net.Http.HttpClient();
			var data = await client.GetAsync(UpdaterConfiguration.ServiceUri);
			if (!data.IsSuccessStatusCode)
				throw new Exception("Не удалось загрузить.\n" + data.StatusCode + "\n" + await data.Content.ReadAsStringAsync());
			foreach (var item in (await data.Content.ReadFromJsonAsync<IEnumerable<DownloadFileInfo>>()))
				yield return item;
		}

		private async Task<List<DownloadFileInfo>> DownloadsArray()
		{
			return await Downloads().ToListAsync();
		}
	}
}
