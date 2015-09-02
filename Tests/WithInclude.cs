using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace TinyXmlMapper.Tests
{
    [TestFixture]
    public class WithInclude
    {
        /*
            <Data>
                <Login>Emiya</Login>
                <Info>simple</Info>
            </Data>
        */

        #region Data

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
            [PropertyWithoutNode]
            public DataImpl Data { get; set; }
        }

        #endregion

        internal static readonly string XmlData = "TinyXmlMapper.Resources.WithInclude.xml";
        internal static readonly string XmlDiffData = "TinyXmlMapper.Resources.WithInclude.Diff.xml";

        [Test]
        public void OnlyWrite()
        {
            string xmlString = Framework.LoadInternalAsString<WithInclude>(XmlData);
            xmlString = Framework.ReplaceWhitespace(xmlString);

            var writeData = new DataWithInclude()
            {
                Customer = new DataWithInclude.CustomerImpl()
                {
                    DefaultName = "Default name",
                },
                Info = new DataWithInclude.InfoImpl()
                {
                    TypeName = "owner",
                },
                Data = new DataWithInclude.DataImpl()
                {
                    Name = "Emiya",
                    Note = "simple",

                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(DataWithInclude.DataImpl.Name), "Login" },
                        { nameof(DataWithInclude.DataImpl.Note), "Info" },
                    }
                },
            };

            var writeMapper = XmlMapper.Build(writeData, "Data");

            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                writeMapper.Write(writer);
            }

            Assert.That(builder.ToString(), Is.EqualTo(xmlString));
        }

        [Test]
        public void OnlyRead()
        {
            string xmlString = Framework.LoadInternalAsString<WithInclude>(XmlData);

            var readData = new DataWithInclude()
            {
                Data = new DataWithInclude.DataImpl()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(DataWithInclude.DataImpl.Name), "Login" },
                        { nameof(DataWithInclude.DataImpl.Note), "Info" },
                    }
                },
            };

            var readMapper = XmlMapper.Build(readData, "Data");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                readMapper.Read(reader);
            }

            Assert.That(readData.Data.Name, Is.EqualTo("Emiya"));
            Assert.That(readData.Data.Note, Is.EqualTo("simple"));
        }

        [Test]
        public void OnlyReadDiff()
        {
            string xmlString = Framework.LoadInternalAsString<WithInclude>(XmlDiffData);

            var readData = new DataWithInclude()
            {
                Data = new DataWithInclude.DataImpl()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(DataWithInclude.DataImpl.Name), "Login" },
                        { nameof(DataWithInclude.DataImpl.Note), "Info" },
                    }
                },
            };

            var readMapper = XmlMapper.Build(readData, "Data");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                readMapper.Read(reader);
            }

            Assert.That(readData.Data.Name, Is.EqualTo("Emiya"));
            Assert.That(readData.Data.Note, Is.EqualTo("simple"));
        }

        [Test]
        public void FullTest()
        {
            var writeData = new DataWithInclude()
            {
                Customer = new DataWithInclude.CustomerImpl()
                {
                    DefaultName = "Default name",
                },
                Info = new DataWithInclude.InfoImpl()
                {
                    TypeName = "owner",
                },
                Data = new DataWithInclude.DataImpl()
                {
                    Name = "Emiya",
                    Note = "simple",

                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(DataWithInclude.DataImpl.Name), "Login" },
                        { nameof(DataWithInclude.DataImpl.Note), "Info" },
                    }
                },
            };

            var writeMapper = XmlMapper.Build(writeData, "Data");

            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                writeMapper.Write(writer);
            }
            string xmlString = builder.ToString();

            var readData = new DataWithInclude()
            {
                Data = new DataWithInclude.DataImpl()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(DataWithInclude.DataImpl.Name), "Login" },
                        { nameof(DataWithInclude.DataImpl.Note), "Info" },
                    }
                },
            };

            var readMapper = XmlMapper.Build(readData, "Data");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                readMapper.Read(reader);
            }

            Assert.That(readData.Data.Name, Is.EqualTo("Emiya"));
            Assert.That(readData.Data.Note, Is.EqualTo("simple"));
        }
    }
}
