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
        public string RegionNumber { get; set; }
        public TimeSpan? ArchiveDownloadTimeout { get; set; }
        public TimeSpan? DbExecuteTimeout { get; set; }
        
        public TimeSpan ArchiveDownloadTimeoutValue => ArchiveDownloadTimeout ?? TimeSpan.FromHours(12);
        public TimeSpan DbExecuteTimeoutValue => DbExecuteTimeout ?? TimeSpan.FromSeconds(900);
        public int RegionsCountValue => RegionsCount ?? 99;

        public string GarFullPath { get; set; }

        public UpdaterConfiguration Combine (UpdaterConfiguration basicConfiguration)
            =>
                new UpdaterConfiguration
                {
                    ConnectionString = this.ConnectionString ?? basicConfiguration.ConnectionString,
                    ServiceUri = this.ServiceUri ?? basicConfiguration.ServiceUri ?? "https://fias.nalog.ru/WebServices/Public/GetAllDownloadFileInfo",
                    ArchivesDirectory = this.ArchivesDirectory ?? basicConfiguration.ArchivesDirectory ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "./archives/"),
                    RegionNumber = this.RegionNumber ?? basicConfiguration.RegionNumber,
                    ArchiveDownloadTimeout = this.ArchiveDownloadTimeout ?? basicConfiguration.ArchiveDownloadTimeout ?? TimeSpan.FromHours(12),
                    DbExecuteTimeout = this.DbExecuteTimeout ?? basicConfiguration.DbExecuteTimeout ?? TimeSpan.FromSeconds(900),
                    GarFullPath = this.GarFullPath ?? basicConfiguration.GarFullPath,
                    RegionsCount = this.RegionsCount ?? basicConfiguration.RegionsCountValue,
                };
    }
}
