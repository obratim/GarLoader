using System.Collections.Generic;
using System.Text;

namespace GarLoader.Engine
{
    public class UpdaterConfiguration
    {
        public string ConnectionString { get; set; }
        public string ServiceUri { get; set; }
        public string ArchivesDirectory { get; set; }
        public string RegionNumber { get; set; }
        public int ManualDownloadFiasNumber { get; set; }
        public int ArchiveDownloadTimeoutInMilliseconds { get; set; }
        public int DbExecuteTimeoutSeconds { get; set; }

        public string GarFullPath { get; set; }

        public UpdaterConfiguration()
        {
            ArchivesDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "./archives/");
            ServiceUri = "https://fias.nalog.ru/WebServices/Public/GetAllDownloadFileInfo";
            ArchiveDownloadTimeoutInMilliseconds = 1_000_000;
            DbExecuteTimeoutSeconds = 900;
        }
    }
}
