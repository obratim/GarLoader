using System;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("ITEM")]
    public class MunicipalHierarchyItem
    {
        [XmlAttribute("ID")]
        public long Id { get; set; }

        [XmlAttribute("OBJECTID")]
        public long ObjectId { get; set; }

        [XmlAttribute("PARENTOBJID")]
        public string ParentObjectIdRaw { get; set; }
        [XmlIgnore] public long? ParentObjectId => long.TryParse(ParentObjectIdRaw, out var parsed) ? parsed : null;

        [XmlAttribute("CHANGEID")]
        public long ChangeTransactionId { get; set; }

        [XmlAttribute("OKTMO")]
        public string OKTMO { get; set; }
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

        [XmlAttribute("ISACTIVE")]
        public int IsActive { get; set; }
    }
}
