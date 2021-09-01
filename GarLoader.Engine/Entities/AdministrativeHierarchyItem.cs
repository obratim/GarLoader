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
        public string RegionCode { get; set; }

        [XmlAttribute("AREACODE")]
        public string AreaCode { get; set; }

        [XmlAttribute("CITYCODE")]
        public string CityCode { get; set; }

        [XmlAttribute("PLACECODE")]
        public string PlaceCode { get; set; }

        [XmlAttribute("PLANCODE")]
        public string PlanCode { get; set; }

        [XmlAttribute("STREETCODE")]
        public string StreetCode { get; set; }

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
        public byte IsActive { get; set; }
    }
}
