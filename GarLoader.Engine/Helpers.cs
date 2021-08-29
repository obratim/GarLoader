using System;
using System.Collections.Generic;
using System.Linq;

namespace GarLoader.Engine
{
    static class Helpers
    {
		public static DateTime GetUpdateDate(this FiasReference.DownloadFileInfo downloadFileInfo)
		{
			return DateTime.ParseExact(downloadFileInfo.TextVersion.Substring(11), "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
		}

		/*public static IEnumerable<T> FilterObjects<T>(this IEnumerable<T> objects)
			where T : FiasTypes.IObject
			=> objects.Where(x => x.RegionCode == Updater.UpdaterConfiguration.RegionNumber);

		public static IEnumerable<T> FilterObjects<T>(this IEnumerable<T> objects, HashSet<Guid> addressObjectGuids)
			where T : FiasTypes.ILegacyObject
			=> objects.Where(x => addressObjectGuids.Contains(x.AoGuid));

		public static IEnumerable<T> FilterObjects<T>(this IEnumerable<T> objects, DateTime minEndDate, HashSet<Guid> addressObjectGuids)
			where T : FiasTypes.ILegacyObject
			=> objects.Where(x => x.EndDate > minEndDate && addressObjectGuids.Contains(x.AoGuid));

*/
        internal static System.Xml.XmlReaderSettings XmlSettings = new System.Xml.XmlReaderSettings();
/*
        internal static readonly Lazy<log4net.ILog> Logger = new Lazy<log4net.ILog>(() =>
        {
            var logger = log4net.LogManager.GetLogger(typeof(Updater));

            var repo = log4net.LogManager.CreateRepository(
                System.Reflection.Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));

            var config = new System.Xml.XmlDocument();
            config.Load(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "log4net.config"));

            log4net.Config.XmlConfigurator.Configure(repo, config["log4net"]);

            return logger;
        });*/

    }
}
