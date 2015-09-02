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
            <Data>
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
                <Info>
                  <Note>some 1</Note>
                </Info>
                <Info>
                  <Note>some 2</Note>
                </Info>
                <Info>
                  <Note>some 3</Note>
                </Info>
                <Info>
                  <Note>some 4</Note>
                </Info>
              </Notes>
            </Data>
        */
        internal static readonly string XmlData = "TinyXmlMapper.Resources.OnlyXml.xml";
        internal static readonly string XmlDiffData = "TinyXmlMapper.Resources.OnlyXml.Diff.xml";

        #region Data

        class Person
        {
            public string Name { get; set; }
            public string Phone { get; set; }
        }

        internal class Address
        {
            public string Data { get; set; }
        }

        class Info
        {
            public string Note { get; set; }
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

            public Person Self { get; set; }

            public Person Owner { get; set; }

            public Address Current { get; set; }

            public List<Info> Notes { get; set; }
        }

        #endregion

        [Test]
        public void OnlyWrite()
        {
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlData);
            xmlString = Framework.ReplaceWhitespace(xmlString);

            var writeData = new Data()
            {
                Self = new Person()
                {
                    Name = "Emiya",
                    Phone = "777",
                },
                Owner = new Person()
                {
                    Name = "None",
                    Phone = "*",
                },
                Current = new Address()
                {
                    Data = "full address ...",
                },
                Notes = new List<Info>()
                {
                    new Info() { Note = "some 1"},
                    new Info()
                    {
                        Note = "some 2"
                    },
                    new Info()
                    {
                        Note = "some 3"
                    },
                    new Info()
                    {
                        Note = "some 4"
                    },
                }
            };

            var writeMapper = XmlMapper.Build(writeData, nameof(Data));

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

            Data readData = new Data()
            {
                Self = new Person()
                {
                },
                Owner = new Person()
                {
                },
                Current = new Address()
                {
                },
                Notes = new List<Info>()
                {
                    new Info() { },
                    new Info()
                    {
                    },
                    new Info()
                    {
                    },
                    new Info()
                    {
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, nameof(Data));

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

            Data readData = new Data()
            {
                Self = new Person()
                {
                },
                Owner = new Person()
                {
                },
                Current = new Address()
                {
                },
                Notes = new List<Info>()
                {
                    new Info() { },
                    new Info()
                    {
                    },
                    new Info()
                    {
                    },
                    new Info()
                    {
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, nameof(Data));

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
            Data writeData = new Data()
            {
                Self = new Person()
                {
                    Name = "Emiya",
                    Phone = "777",
                },
                Owner = new Person()
                {
                    Name = "None",
                    Phone = "*",
                },
                Current = new Address()
                {
                    Data = "full address ...",
                },
                Notes = new List<Info>()
                {
                    new Info() { Note = "some 1" },
                    new Info()
                    {
                        Note = "some 2"
                    },
                    new Info()
                    {
                        Note = "some 3"
                    },
                    new Info()
                    {
                        Note = "some 4"
                    },
                }
            };

            var writeMapper = XmlMapper.Build(writeData, nameof(Data));

            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                writeMapper.Write(writer);
            }
            string xmlString = builder.ToString();


            Data readData = new Data()
            {
                Self = new Person()
                {
                },
                Owner = new Person()
                {
                },
                Current = new Address()
                {
                },
                Notes = new List<Info>()
                {
                    new Info() { },
                    new Info()
                    {
                    },
                    new Info()
                    {
                    },
                    new Info()
                    {
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, nameof(Data));

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
