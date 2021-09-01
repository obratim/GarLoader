using System;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("PARAM")]
    public class Parameter
    {
        [XmlAttribute("ID")]
        public long Id { get; set; }

        [XmlAttribute("OBJECTID")]
        public long ObjectId { get; set; }

        [XmlAttribute("CHANGEID")]
        public long? ChangeTransactionId { get; set; }

        [XmlAttribute("CHANGEIDEND")]
        public long? ChangeTransactionIdEnd { get; set; }
        
        [XmlAttribute("TYPEID")]
        public int TypeId { get; set; }
        
        [XmlAttribute("VALUE")]
        public string Value { get; set; }

        [XmlAttribute("UPDATEDATE")]
        public DateTime UpdateDate { get; set; }

        [XmlAttribute("STARTDATE")]
        public DateTime StartDate { get; set; }

        [XmlAttribute("ENDDATE")]
        public DateTime EndDate { get; set; }
    }
}
