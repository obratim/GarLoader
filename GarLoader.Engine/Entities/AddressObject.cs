using System;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("OBJECT")]
    public class AddressObject
    {
        [XmlAttribute("ID")]
        public long Id { get; set; }

        [XmlIgnore]
        public int? Region { get; set; }

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
        public int Level { get; set; }

        [XmlAttribute("OPERTYPEID")]
        public int OperationTypeId { get; set; }

        [XmlAttribute("PREVID")]
        public string PrevIdRaw { get; set; }
        [XmlIgnore] public long? PrevId => long.TryParse(PrevIdRaw, out var parsed) ? parsed : null;

        [XmlAttribute("NEXTID")]
        public string NextIdRaw { get; set; }
        [XmlIgnore] public long? NextId => long.TryParse(NextIdRaw, out var parsed) ? parsed : null;

        [XmlAttribute("UPDATEDATE")]
        public DateTime UpdateDate { get; set; }

        [XmlAttribute("STARTDATE")]
        public DateTime StartDate { get; set; }

        [XmlAttribute("ENDDATE")]
        public DateTime EndDate { get; set; }

        [XmlAttribute("ISACTUAL")]
        public sbyte IsActual { get; set; }

        [XmlAttribute("ISACTIVE")]
        public sbyte IsActive { get; set; }
    }
}
