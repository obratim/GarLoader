using System;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("ITEM")]
    public class AdministrativeHierarchyItem
    {
        [XmlAttribute("ID")]
        public long Id { get; set; }

        [XmlAttribute("OBJECTID")]
        public long ObjectId { get; set; }

        [XmlAttribute("PARENTOBJID")]
        public long? ParentObjectId { get; set; }

        [XmlAttribute("CHANGEID")]
        public long ChangeTransactionId { get; set; }

        [XmlAttribute("REGIONCODE")]
        public int? RegionCode { get; set; }

        [XmlAttribute("AREACODE")]
        public int? AreaCode { get; set; }

        [XmlAttribute("CITYCODE")]
        public int? CityCode { get; set; }

        [XmlAttribute("PLACECODE")]
        public int? PlaceCode { get; set; }

        [XmlAttribute("PLANCODE")]
        public int? PlanCode { get; set; }

        [XmlAttribute("STREETCODE")]
        public int? StreetCode { get; set; }

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

        [XmlAttribute("ISACTIVE")]
        public int IsActive { get; set; }
    }
}
