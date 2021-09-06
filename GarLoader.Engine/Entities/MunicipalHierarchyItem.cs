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
        public long? ParentObjectId { get; set; }

        [XmlAttribute("CHANGEID")]
        public long ChangeTransactionId { get; set; }

        [XmlAttribute("OKTMO")]
        public string OKTMO { get; set; }
        public long? PrevId { get; set; }

        [XmlAttribute("NEXTID")]
        public long? NextId { get; set; }

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
