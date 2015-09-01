using System.Collections.Generic;

namespace TinyXmlMapper.Tests
{
    [ClassWithInclude]
    class DataWithInclude
    {
        public class CustomerImpl
        {
            public string DefaultName { get; set; }
        }

        public class InfoImpl 
        {
            public string TypeName { get; set; }
        }

        public class DataImpl
        {
            public string Name { get; set; }
            public string Note { get; set; }

            [PropertyMap]
            public Dictionary<string, string> PropertyNames { get; set; }
        }

        public CustomerImpl Customer { get; set; }

        public InfoImpl Info { get; set; }

        [PropertyInclude]
        [PropertyWithoutNodeAttribute]
        public DataImpl Data { get; set; }
    }
}
