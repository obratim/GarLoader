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
        public string ParentObjectIdRaw { get; set; }
        [XmlIgnore] public long? ParentObjectId => long.TryParse(ParentObjectIdRaw, out var parsed) ? parsed : null;

        [XmlAttribute("CHANGEID")]
        public long ChangeTransactionId { get; set; }

        [XmlAttribute("REGIONCODE")]
        public string RegionCodeRaw { get; set; }
        [XmlIgnore] public int? RegionCode => int.TryParse(RegionCodeRaw, out var parsed) ? parsed : null;

        [XmlAttribute("AREACODE")]
        public string AreaCodeRaw { get; set; }
        [XmlIgnore] public int? AreaCode => int.TryParse(AreaCodeRaw, out var parsed) ? parsed : null;

        [XmlAttribute("CITYCODE")]
        public string CityCodeRaw { get; set; }
        [XmlIgnore] public int? CityCode => int.TryParse(CityCodeRaw, out var parsed) ? parsed : null;

        [XmlAttribute("PLACECODE")]
        public string PlaceCodeRaw { get; set; }
        [XmlIgnore] public int? PlaceCode => int.TryParse(PlaceCodeRaw, out var parsed) ? parsed : null;

        [XmlAttribute("PLANCODE")]
        public string PlanCodeRaw { get; set; }
        [XmlIgnore] public int? PlanCode => int.TryParse(PlanCodeRaw, out var parsed) ? parsed : null;

        [XmlAttribute("STREETCODE")]
        public string StreetCodeRaw { get; set; }
        [XmlIgnore] public int? StreetCode => int.TryParse(StreetCodeRaw, out var parsed) ? parsed : null;

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

        [XmlAttribute("ISACTIVE")]
        public int IsActive { get; set; }
    }
}
