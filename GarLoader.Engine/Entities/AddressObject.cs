using System;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("OBJECT")]
    public class AddressObject
    {
        [XmlAttribute("ID")]
        public long Id { get; set; }

        [XmlAttribute("OBJECTID")]
        public long ObjectId { get; set; }

        [XmlAttribute("OBJECTGUID")]
        public Guid ObjectGuid { get; set; }

        [XmlAttribute("CHANGEID")]
        public long ChangedTransactionId { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

        [XmlAttribute("TYPENAME")]
        public string TypeName { get; set; }

        [XmlAttribute("LEVEL")]
        public long Level { get; set; }

        [XmlAttribute("OPERTYPEID")]
        public int? ObjectTypeId { get; set; }

        [XmlAttribute("PREVID")]
        public long? PrevId { get; set; }

        [XmlAttribute("NEXTID")]
        public long? NextId { get; set; }

        [XmlAttribute("UPDATEDATE")]
        public DateTime UpdateDate { get; set; }

        [XmlAttribute("STARTDATE")]
        public DateTime StartDate { get; set; }

        [XmlAttribute("ENDDATE")]
        public DateTime EndDate { get; set; }

        [XmlAttribute("ISACTUAL")]
        public bool IsActual { get; set; }

        [XmlAttribute("ISACTIVE")]
        public bool IsActive { get; set; }
    }
}
