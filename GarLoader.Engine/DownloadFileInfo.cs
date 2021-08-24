using System;
using System.Text.Json.Serialization;

namespace GarLoader.Engine
{
    public class DownloadFileInfo
    {
        public int VersionId { get; set; }
        public string TextVersion { get; set; }
        public string FiasCompleteDbfUrl { get; set; }
        public string FiasCompleteXmlUrl { get; set; }
        public string FiasDeltaDbfUrl { get; set; }
        public string FiasDeltaXmlUrl { get; set; }
        public string Kladr4ArjUrl { get; set; }
        public string Kladr47ZUrl { get; set; }
        public string GarXMLFullURL { get; set; }
        public string GarXMLDeltaURL { get; set; }
        public string Date { get; set; }
        
        [JsonIgnore]
        public DateTime? ParsedDate
        {
            get
            {
                if (DateTime.TryParse(Date, out var result))
                    return result;
                return default;
            }
        }
    }
}
