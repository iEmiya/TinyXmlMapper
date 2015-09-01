using System.Collections.Generic;

namespace TinyXmlMapper.Tests
{
    class Person 
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        [PropertyMap]
        public Dictionary<string, string> PropertyNames { get; set; }
    }

    class Address
    {
        public string Data { get; set; }

        [PropertyMap]
        public Dictionary<string, string> PropertyNames { get; set; }
    }

    class Info
    {
        public string Note { get; set; }

        [PropertyMap]
        public Dictionary<string, string> PropertyNames { get; set; }
    }

    class Data
    {
        public Data()
        {
            Self = new Person();
            Owner = new Person();
            Current = new Address();
            Notes = new List<Info>();
        }

        [PropertyWithoutNode]
        public Person Self { get; set; }
        [PropertyWithoutNode]
        public Person Owner { get; set; }
        [PropertyWithoutNode]
        public Address Current { get; set; }

        [PropertyWithoutNode]
        public List<Info> Notes { get; set; }

        [PropertyMap]
        public Dictionary<string, string> PropertyNames { get; set; }
    }
}
