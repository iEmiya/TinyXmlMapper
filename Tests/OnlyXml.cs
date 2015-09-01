using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace TinyXmlMapper.Tests
{
    [TestFixture]
    public class OnlyXml
    {
        /*
            <DataXml>
              <Self>
                <Name>Emiya</Name>
                <Phone>777</Phone>
              </Self>
              <Owner>
                <Name>None</Name>
                <Phone>*</Phone>
              </Owner>
              <Current>
                <Data>full address ...</Data>
              </Current>
              <Notes>
                <InfoXml>
                  <Note>some 1</Note>
                </InfoXml>
                <InfoXml>
                  <Note>some 2</Note>
                </InfoXml>
                <InfoXml>
                  <Note>some 3</Note>
                </InfoXml>
                <InfoXml>
                  <Note>some 4</Note>
                </InfoXml>
              </Notes>
            </DataXml>
        */
        internal static readonly string XmlData = "TinyXmlMapper.Resources.OnlyXml.xml";
        internal static readonly string XmlDiffData = "TinyXmlMapper.Resources.OnlyXml.Diff.xml";

        [Test]
        public void OnlyWrite()
        {
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlData);
            xmlString = Framework.ReplaceWhitespace(xmlString);

            var writeData = new DataXml()
            {
                Self = new PersonXml()
                {
                    Name = "Emiya",
                    Phone = "777",
                },
                Owner = new PersonXml()
                {
                    Name = "None",
                    Phone = "*",
                },
                Current = new AddressXml()
                {
                    Data = "full address ...",
                },
                Notes = new List<InfoXml>()
                {
                    new InfoXml() { Note = "some 1"},
                    new InfoXml()
                    {
                        Note = "some 2"
                    },
                    new InfoXml()
                    {
                        Note = "some 3"
                    },
                    new InfoXml()
                    {
                        Note = "some 4"
                    },
                }
            };

            var writeMapper = XmlMapper.Build(writeData, nameof(DataXml));

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
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlData);

            DataXml readData = new DataXml()
            {
                Self = new PersonXml()
                {
                },
                Owner = new PersonXml()
                {
                },
                Current = new AddressXml()
                {
                },
                Notes = new List<InfoXml>()
                {
                    new InfoXml() { },
                    new InfoXml()
                    {
                    },
                    new InfoXml()
                    {
                    },
                    new InfoXml()
                    {
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, nameof(DataXml));

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                readMapper.Read(reader);
            }

            Assert.That(readData.Self.Name, Is.EqualTo("Emiya"));
            Assert.That(readData.Self.Phone, Is.EqualTo("777"));
            Assert.That(readData.Owner.Name, Is.EqualTo("None"));
            Assert.That(readData.Owner.Phone, Is.EqualTo("*"));
            Assert.That(readData.Current.Data, Is.EqualTo("full address ..."));
            Assert.IsNotNull(readData.Notes);
            Assert.That(readData.Notes.Count, Is.EqualTo(4));
            Assert.That(readData.Notes[0].Note, Is.EqualTo("some 1"));
            Assert.That(readData.Notes[1].Note, Is.EqualTo("some 2"));
            Assert.That(readData.Notes[2].Note, Is.EqualTo("some 3"));
            Assert.That(readData.Notes[3].Note, Is.EqualTo("some 4"));
        }

        [Test]
        public void OnlyReadDiff()
        {
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlDiffData);

            DataXml readData = new DataXml()
            {
                Self = new PersonXml()
                {
                },
                Owner = new PersonXml()
                {
                },
                Current = new AddressXml()
                {
                },
                Notes = new List<InfoXml>()
                {
                    new InfoXml() { },
                    new InfoXml()
                    {
                    },
                    new InfoXml()
                    {
                    },
                    new InfoXml()
                    {
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, nameof(DataXml));

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                readMapper.Read(reader);
            }

            Assert.That(readData.Self.Name, Is.EqualTo("Emiya"));
            Assert.That(readData.Self.Phone, Is.EqualTo("777"));
            Assert.That(readData.Owner.Name, Is.EqualTo("None"));
            Assert.That(readData.Owner.Phone, Is.EqualTo("*"));
            Assert.That(readData.Current.Data, Is.EqualTo("full address ..."));
            Assert.IsNotNull(readData.Notes);
            Assert.That(readData.Notes.Count, Is.EqualTo(4));
            Assert.That(readData.Notes[0].Note, Is.EqualTo("some 1"));
            Assert.That(readData.Notes[1].Note, Is.EqualTo("some 2"));
            Assert.That(readData.Notes[2].Note, Is.EqualTo("some 3"));
            Assert.That(readData.Notes[3].Note, Is.EqualTo("some 4"));
        }

        [Test]
        public void FullTest()
        {
            DataXml writeData = new DataXml()
            {
                Self = new PersonXml()
                {
                    Name = "Emiya",
                    Phone = "777",
                },
                Owner = new PersonXml()
                {
                    Name = "None",
                    Phone = "*",
                },
                Current = new AddressXml()
                {
                    Data = "full address ...",
                },
                Notes = new List<InfoXml>()
                {
                    new InfoXml() { Note = "some 1" },
                    new InfoXml()
                    {
                        Note = "some 2"
                    },
                    new InfoXml()
                    {
                        Note = "some 3"
                    },
                    new InfoXml()
                    {
                        Note = "some 4"
                    },
                }
            };

            var writeMapper = XmlMapper.Build(writeData, nameof(DataXml));

            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                writeMapper.Write(writer);
            }
            string xmlString = builder.ToString();


            DataXml readData = new DataXml()
            {
                Self = new PersonXml()
                {
                },
                Owner = new PersonXml()
                {
                },
                Current = new AddressXml()
                {
                },
                Notes = new List<InfoXml>()
                {
                    new InfoXml() { },
                    new InfoXml()
                    {
                    },
                    new InfoXml()
                    {
                    },
                    new InfoXml()
                    {
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, nameof(DataXml));

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                readMapper.Read(reader);
            }

            Assert.That(readData.Self.Name, Is.EqualTo("Emiya"));
            Assert.That(readData.Self.Phone, Is.EqualTo("777"));
            Assert.That(readData.Owner.Name, Is.EqualTo("None"));
            Assert.That(readData.Owner.Phone, Is.EqualTo("*"));
            Assert.That(readData.Current.Data, Is.EqualTo("full address ..."));
            Assert.IsNotNull(readData.Notes);
            Assert.That(readData.Notes.Count, Is.EqualTo(4));
            Assert.That(readData.Notes[0].Note, Is.EqualTo("some 1"));
            Assert.That(readData.Notes[1].Note, Is.EqualTo("some 2"));
            Assert.That(readData.Notes[2].Note, Is.EqualTo("some 3"));
            Assert.That(readData.Notes[3].Note, Is.EqualTo("some 4"));
        }
    }
}
