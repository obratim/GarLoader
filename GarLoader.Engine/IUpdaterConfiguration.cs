using System;
using System.Collections.Generic;
using System.Text;

namespace GarLoader.Engine
{
    public interface IUpdaterConfiguration
    {
		string ConnectionString { get; set; }
		string ServiceUri { get; set; }
		string ArchivesDirectory { get; set; }
		string RegionNumber { get; set; }
		int ManualDownloadFiasNumber { get; set; }
		int ArchiveDownloadTimeoutInMilliseconds { get; set; }
	}
}
