using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace TinyXmlMapper.Tests
{
    [TestFixture]
    public class WriteAndRead
    {
        /*
            <Information>
                <Self_Name>Emiya</Self_Name>
                <Self_Phone>777</Self_Phone>
                <Owner>None</Owner>
                <Owner_Phone>*</Phone>
                <Local_Address>full address ...</Local_Address>
                <MyNotes>
                    <Note>some 1</Note>
                    <E>some 2</E>
                    <E2>some 3</E2>
                    <E3>some 4</E3>
                </MyNotes>
            </Information>
        */

        #region Data

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

        #endregion

        internal static readonly string XmlData = "TinyXmlMapper.Resources.WriteAndRead.xml";
        internal static readonly string XmlDiffData = "TinyXmlMapper.Resources.WriteAndRead.Diff.xml";

        [Test]
        public void OnlyWrite()
        { 
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlData);
            xmlString = Framework.ReplaceWhitespace(xmlString);

            var writeData = new Data()
            {
                PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Data.Notes), "MyNotes" },
                    },
                Self = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Self_Name" },
                        { nameof(Person.Phone), "Self_Phone" },
                    },


                    Name = "Emiya",
                    Phone = "777",
                },
                Owner = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Owner" },
                        { nameof(Person.Phone), "Owner_Phone" },
                    },


                    Name = "None",
                    Phone = "*",
                },
                Current = new Address()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Address.Data), "Local_Address" },
                    },

                    Data = "full address ...",
                },
                Notes = new List<Info>()
                {
                    new Info() { Note = "some 1"},
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E" },
                        },

                        Note = "some 2"
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E2" },
                        },

                        Note = "some 3"
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E3" },
                        },

                        Note = "some 4"
                    },
                }
            };


            var writeMapper = XmlMapper.Build(writeData, "Information");

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
                PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Data.Notes), "MyNotes" },
                    },

                Self = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Self_Name" },
                        { nameof(Person.Phone), "Self_Phone" },
                    },
                },
                Owner = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Owner" },
                        { nameof(Person.Phone), "Owner_Phone" },
                    },
                },
                Current = new Address()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Address.Data), "Local_Address" },
                    },
                },
                Notes = new List<Info>()
                {
                    new Info() { },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E" },
                        },
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E2" },
                        },
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E3" },
                        },
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, "Information");

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
                PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Data), "Information" },
                        { nameof(Data.Notes), "MyNotes" },
                    },

                Self = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Self_Name" },
                        { nameof(Person.Phone), "Self_Phone" },
                    },
                },
                Owner = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Owner" },
                        { nameof(Person.Phone), "Owner_Phone" },
                    },
                },
                Current = new Address()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Address.Data), "Local_Address" },
                    },
                },
                Notes = new List<Info>()
                {
                    new Info() { Note = "some 1"},
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E" },
                        },
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E2" },
                        },
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E3" },
                        },
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, "Information");

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
            var writeData = new Data()
            {
                PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Data), "Information" },
                        { nameof(Data.Notes), "MyNotes" },
                    },

                Self = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Self_Name" },
                        { nameof(Person.Phone), "Self_Phone" },
                    },


                    Name = "Emiya",
                    Phone = "777",
                },
                Owner = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Owner" },
                        { nameof(Person.Phone), "Owner_Phone" },
                    },


                    Name = "None",
                    Phone = "*",
                },
                Current = new Address()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Address.Data), "Local_Address" },
                    },

                    Data = "full address ...",
                },
                Notes = new List<Info>()
                {
                    new Info() { Note = "some 1"},
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E" },
                        },

                        Note = "some 2"
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E2" },
                        },

                        Note = "some 3"
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E3" },
                        },

                        Note = "some 4"
                    },
                }
            };

            var writeMapper = XmlMapper.Build(writeData, "Information");

            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                writeMapper.Write(writer);
            }
            string xmlString = builder.ToString();
            

            Data readData = new Data()
            {
                PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Data), "Information" },
                        { nameof(Data.Notes), "MyNotes" },
                    },

                Self = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Self_Name" },
                        { nameof(Person.Phone), "Self_Phone" },
                    },
                },
                Owner = new Person()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Person.Name), "Owner" },
                        { nameof(Person.Phone), "Owner_Phone" },
                    },
                },
                Current = new Address()
                {
                    PropertyNames = new Dictionary<string, string>()
                    {
                        { nameof(Address.Data), "Local_Address" },
                    },
                },
                Notes = new List<Info>()
                {
                    new Info() { Note = "some 1"},
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E" },
                        },
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E2" },
                        },
                    },
                    new Info()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Info.Note), "E3" },
                        },
                    },
                }
            };

            var readMapper = XmlMapper.Build(readData, "Information");

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
