using System;
using System.Collections.Generic;
using System.Text;

namespace GarLoader.Engine
{
    public class UpdaterConfiguration
    {
        public string ConnectionString { get; set; }
        public string ServiceUri { get; set; }
        public int? RegionsCount { get; set; }
        public string ArchivesDirectory { get; set; }
        public TimeSpan? ArchiveDownloadTimeout { get; set; }
        public TimeSpan? DbExecuteTimeout { get; set; }
        
        public TimeSpan ArchiveDownloadTimeoutValue => ArchiveDownloadTimeout ?? TimeSpan.FromHours(2);
        public TimeSpan DbExecuteTimeoutValue => DbExecuteTimeout ?? TimeSpan.FromSeconds(900);
        public int RegionsCountValue => RegionsCount ?? 99;
        
        public bool DeleteArchiveFile { get; set; }

        public string GarFullPath { get; set; }

        public UpdaterConfiguration Combine (UpdaterConfiguration basicConfiguration)
            =>
                new UpdaterConfiguration
                {
                    ConnectionString = this.ConnectionString ?? basicConfiguration.ConnectionString,
                    ServiceUri = this.ServiceUri ?? basicConfiguration.ServiceUri ?? "https://fias.nalog.ru/WebServices/Public/GetAllDownloadFileInfo",
                    ArchivesDirectory = this.ArchivesDirectory ?? basicConfiguration.ArchivesDirectory ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "./archives/"),
                    ArchiveDownloadTimeout = this.ArchiveDownloadTimeout ?? basicConfiguration.ArchiveDownloadTimeoutValue,
                    DbExecuteTimeout = this.DbExecuteTimeout ?? basicConfiguration.DbExecuteTimeoutValue,
                    GarFullPath = this.GarFullPath ?? basicConfiguration.GarFullPath,
                    RegionsCount = this.RegionsCount ?? basicConfiguration.RegionsCountValue,
                };
    }
}
