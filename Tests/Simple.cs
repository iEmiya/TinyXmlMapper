using System.Collections.Generic;

namespace TinyXmlMapper.Tests
{
    class PersonXml
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    internal class AddressXml
    {
        public string Data { get; set; }
    }

    class InfoXml
    {
        public string Note { get; set; }
    }

    class DataXml
    {
        public DataXml()
        {
            Self = new PersonXml();
            Owner = new PersonXml();
            Current = new AddressXml();
            Notes = new List<InfoXml>();
        }

        public PersonXml Self { get; set; }
        
        public PersonXml Owner { get; set; }
        
        public AddressXml Current { get; set; }

        public List<InfoXml> Notes { get; set; }
    }
}
