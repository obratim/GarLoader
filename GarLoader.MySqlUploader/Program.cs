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
                    services.AddSingleton<Updater>(provider => new Updater(
                        provider.GetRequiredService<IUploader>(),
                        provider.GetRequiredService<ILogger<Updater>>(),
                        provider.GetRequiredService<IOptions<UpdaterConfiguration>>(),
                        config.Item2
                    ));
                    services.AddHostedService<Worker>();
                });
        }
    }

    class CmdOptions
    {
        [Option('f', "filepath", HelpText = "Путь к архиву ГАР")]
        public string GarFilePath { get; set; }

        public static IEnumerable<KeyValuePair<PropertyDescriptor, OptionAttribute>> CmdArguments
            =>
                TypeDescriptor
                    .GetProperties(typeof(CmdOptions))
                    .Cast<PropertyDescriptor>()
                    .Select(p => new KeyValuePair<PropertyDescriptor, OptionAttribute>(p, p.Attributes.OfType<OptionAttribute>().FirstOrDefault()))
                    .Where(x => x.Value != null);
    }
}
