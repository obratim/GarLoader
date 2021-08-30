using System;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("OPERATIONTYPE")]
    public class OperationType
    {
        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

        [XmlAttribute("SHORTNAME")]
        public string ShortName { get; set; }

        [XmlAttribute("DESC")]
        public string Description { get; set; }

        [XmlAttribute("UPDATEDATE")]
        public DateTime UpdateDate { get; set; }

        [XmlAttribute("STARTDATE")]
        public DateTime StartDate { get; set; }

        [XmlAttribute("ENDDATE")]
        public DateTime EndDate { get; set; }

        [XmlAttribute("ISACTIVE")]
        public bool IsActive { get; set; }
    }
}
