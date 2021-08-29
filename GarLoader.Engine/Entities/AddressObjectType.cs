using System;
using System.IO;
using System.Xml.Serialization;

namespace GarLoader.Engine
{
    [XmlRoot("ADDRESSOBJECTTYPE")]
    public class AddressObjectType
    {
        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlAttribute("LEVEL")]
        public int Level { get; set; }

        [XmlAttribute("SHORTNAME")]
        public string ShortName { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

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

    [XmlRoot("ADDRESSOBJECTTYPES")]
    public class AddressObjectTypes
    {
        [XmlElement("ADDRESSOBJECTTYPE")]
        public AddressObjectType[] Items { get; set; }

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(AddressObjectTypes));

        public static AddressObjectTypes Deserialize(Stream input) => (AddressObjectTypes)_serializer.Deserialize(input);
    }
}
