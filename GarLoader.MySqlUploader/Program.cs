using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using GarLoader.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GarLoader.MySqlUploader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args?.Length == 1 && (args[0] == "--help" || args[0] == "-h"))
            {
                Console.WriteLine("Загрузчик данных Государственного Адресного Реестра в БД MySql");
                Console.WriteLine("Параметры:");
                foreach (var argument in CmdOptions.CmdArguments)
                {
                    Console.WriteLine(
                        " {0} {1,-15} {2,-5} {3,-30}",
                        argument.Value.Required ? '*' : ' ',
                        argument.Value.LongName,
                        argument.Value.ShortName,
                        argument.Value.HelpText);
                }
                return;
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = CommandLine.Parser.Default
                .ParseArguments<CmdOptions>(args)
                .MapResult(o => (true, new UpdaterConfiguration {
                    GarFullPath = o.GarFilePath,
                    ConnectionString = o.ConnectionString,
                    ServiceUri = o.FiasServiceUrl,
                    RegionsCount = o.RegionsCount,
                    ArchivesDirectory = o.ArchivesDirectory,
                    ArchiveDownloadTimeout = TimeSpan.FromSeconds(o.ArchiveDownloadTimeout),
                    DbExecuteTimeout = TimeSpan.FromSeconds(o.DbExecuteTimeout),
                }, default(IEnumerable<Error>)),
                errors => (false, default, errors));
            if (!config.Item1)
            {
                foreach (var e in config.Item3)
                    Console.WriteLine(e.ToString());
                throw new Exception("Не распарсил");
            }
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IUploader, UploaderToMySql>();
                    services
                        .AddOptions<UpdaterConfiguration>()
                        .Bind(hostContext.Configuration.GetSection("UpdaterConfiguration"))
                        .ValidateDataAnnotations();
                    services.AddSingleton<UpdaterConfiguration>(provider => config.Item2.Combine(provider.GetRequiredService<IOptions<UpdaterConfiguration>>().Value));
                    services.AddSingleton<Updater>();
                    services.AddHostedService<Worker>();
                });
        }
    }

    class CmdOptions
    {
        [Option('u', "url", HelpText = "Адрес сервиса ФИАС с версиями файлов, доступных для скачивания")]
        public string FiasServiceUrl { get; set; }

        [Option('c', "conn", HelpText = "Строка подключения к БД")]
        public string ConnectionString { get; set; }

        [Option('f', "filepath", HelpText = "Путь к архиву ГАР")]
        public string GarFilePath { get; set; }

        [Option('a', "archdir", HelpText = "Полный путь к директории для сохранения загруженных архивов")]
        public string ArchivesDirectory { get; set; }

        [Option('r', "regions", HelpText = "Максимальный номер региона (по-умолчанию 99)")]
        public int RegionsCount { get; set; }

        [Option('T', "archivetimeout", HelpText = "Таймаут скачивания архива в секундах")]
        public int ArchiveDownloadTimeout { get; set; }
        
        [Option('t', "dbtimeout", HelpText = "Таймаут выполнения запросов к БД в секундах")]
        public int DbExecuteTimeout { get; set; }

        public static IEnumerable<KeyValuePair<PropertyDescriptor, OptionAttribute>> CmdArguments
            =>
                TypeDescriptor
                    .GetProperties(typeof(CmdOptions))
                    .Cast<PropertyDescriptor>()
                    .Select(p => new KeyValuePair<PropertyDescriptor, OptionAttribute>(p, p.Attributes.OfType<OptionAttribute>().FirstOrDefault()))
                    .Where(x => x.Value != null);
    }
}
