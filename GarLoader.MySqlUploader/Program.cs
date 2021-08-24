using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarLoader.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GarLoader.MySqlUploader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IUploader, UploaderToMySql>();
                    services
                        .AddOptions<UpdaterConfiguration>()
                        .Bind(hostContext.Configuration.GetSection("UpdaterConfiguration"))
                        .ValidateDataAnnotations();
                    services.AddSingleton<Updater>();
                    services.AddHostedService<Worker>();
                });
    }
}
