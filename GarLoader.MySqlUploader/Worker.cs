using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarLoader.Engine;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GarLoader.MySqlUploader
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IUploader _uploader;
        private readonly Updater _updater;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly UpdaterConfiguration _instanceConfig;

        public Worker(ILogger<Worker> logger, IUploader uploader, Updater updater, IHostApplicationLifetime lifetime, UpdaterConfiguration instanceConfig)
        {
            _logger = logger;
            _uploader = uploader;
            _updater = updater;
            _lifetime = lifetime;
            _instanceConfig = instanceConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updater.Update(_instanceConfig);
            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/
            _lifetime.StopApplication();
        }
    }
}
