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

        public Worker(ILogger<Worker> logger, IUploader uploader, Updater updater, IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _uploader = uploader;
            _updater = updater;
            _lifetime = lifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _updater.Update(stoppingToken);
            _lifetime.StopApplication();
        }
    }

    public class GenericContext<T>
    {
        private T _value;
        volatile private bool _initialized = false;

        public void Initialize(T value)
        {
            _value = value;
            _initialized = true;
        }

        public T GetValue()
        {
            if (_initialized)
                return _value;
            else
                throw new Exception("Context not initialized");
        }
    }
}
