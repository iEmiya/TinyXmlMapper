using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace TinyXmlMapper.Tests
{
    [TestFixture]
    public class Hierarchy
    {
        /*
            <Information>
              <Self_Name>Emiya</Self_Name>
              <Self_Phone>777</Self_Phone>
              <Owner>None</Owner>
              <Owner_Phone>*</Owner_Phone>
              <Local_Address>full address ...</Local_Address>
            </Information>
        */

        #region Data

        interface IElement { }

        [ClassWithInclude]
        class Person : IElement
        {
            [PropertyInclude]
            public string Name { get; set; }

            [PropertyInclude]
            public string Phone { get; set; }

            [PropertyMap]
            public Dictionary<string, string> PropertyNames { get; set; }
        }

        [ClassWithInclude]
        class Address : IElement
        {
            [PropertyInclude]
            public string Text { get; set; }

            [PropertyMap]
            public Dictionary<string, string> PropertyNames { get; set; }
        }

        [ClassWithInclude]
        class Container : IElement
        {
            [PropertyInclude]
            [PropertyWithoutRoot]
            [PropertyWithoutNode]
            public List<IElement> DataList { get; set; }
        }

        

        #endregion

        internal static readonly string XmlData = "TinyXmlMapper.Resources.Hierarchy.xml";
        internal static readonly string XmlDiffData = "TinyXmlMapper.Resources.Hierarchy.Diff.xml";

        [Test]
        public void OnlyWrite()
        {
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlData);
            xmlString = Framework.ReplaceWhitespace(xmlString);

            Container writeData = new Container()
            {
                DataList = new List<IElement>()
                {
                    new Container()
                    {
                        DataList = new List<IElement>()
                        {
                            new Person()
                            {
                                Name = "Emiya",
                                Phone = "777",

                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Self_Name" },
                                    { nameof(Person.Phone), "Self_Phone" },
                                }
                            },
                            new Person()
                            {
                                Name = "None",
                                Phone = "*",

                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Owner" },
                                    { nameof(Person.Phone), "Owner_Phone" },
                                }
                            },
                        }
                    },
                    new Address()
                    {
                        Text = "full address ...",

                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Address.Text), "Local_Address" },
                        }
                    }
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

            Container readData = new Container()
            {
                DataList = new List<IElement>()
                {
                    new Container()
                    {
                        DataList = new List<IElement>()
                        {
                            new Person()
                            {
                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Self_Name" },
                                    { nameof(Person.Phone), "Self_Phone" },
                                }
                            },
                            new Person()
                            {
                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Owner" },
                                    { nameof(Person.Phone), "Owner_Phone" },
                                }
                            },
                        }
                    },
                    new Address()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Address.Text), "Local_Address" },
                        }
                    }
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

            Assert.IsNotNull(readData.DataList);
            Assert.That(readData.DataList.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<Container>(readData.DataList[0]);
            Container list = (Container)readData.DataList[0];
            Assert.That(list.DataList.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<Person>(list.DataList[0]);
            Person self = (Person)list.DataList[0];
            Assert.That(self.Name, Is.EqualTo("Emiya"));
            Assert.That(self.Phone, Is.EqualTo("777"));
            Assert.IsInstanceOf<Person>(list.DataList[1]);
            Person owner = (Person)list.DataList[1];
            Assert.That(owner.Name, Is.EqualTo("None"));
            Assert.That(owner.Phone, Is.EqualTo("*"));
            Assert.IsInstanceOf<Address>(readData.DataList[1]);
            Address current = (Address) readData.DataList[1];
            Assert.That(current.Text, Is.EqualTo("full address ..."));
        }

        [Test]
        public void OnlyReadDiff()
        {
            string xmlString = Framework.LoadInternalAsString<WriteAndRead>(XmlDiffData);

            Container readData = new Container()
            {
                DataList = new List<IElement>()
                {
                    new Container()
                    {
                        DataList = new List<IElement>()
                        {
                            new Person()
                            {
                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Self_Name" },
                                    { nameof(Person.Phone), "Self_Phone" },
                                }
                            },
                            new Person()
                            {
                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Owner" },
                                    { nameof(Person.Phone), "Owner_Phone" },
                                }
                            },
                        }
                    },
                    new Address()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Address.Text), "Local_Address" },
                        }
                    }
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

            Assert.IsNotNull(readData.DataList);
            Assert.That(readData.DataList.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<Container>(readData.DataList[0]);
            Container list = (Container)readData.DataList[0];
            Assert.That(list.DataList.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<Person>(list.DataList[0]);
            Person self = (Person)list.DataList[0];
            Assert.That(self.Name, Is.EqualTo("Emiya"));
            Assert.That(self.Phone, Is.EqualTo("777"));
            Assert.IsInstanceOf<Person>(list.DataList[1]);
            Person owner = (Person)list.DataList[1];
            Assert.That(owner.Name, Is.EqualTo("None"));
            Assert.That(owner.Phone, Is.EqualTo("*"));
            Assert.IsInstanceOf<Address>(readData.DataList[1]);
            Address current = (Address)readData.DataList[1];
            Assert.That(current.Text, Is.EqualTo("full address ..."));
        }

        [Test]
        public void FullTest()
        {
            Container writeData = new Container()
            {
                DataList = new List<IElement>()
                {
                    new Container()
                    {
                        DataList = new List<IElement>()
                        {
                            new Person()
                            {
                                Name = "Emiya",
                                Phone = "777",

                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Self_Name" },
                                    { nameof(Person.Phone), "Self_Phone" },
                                }
                            },
                            new Person()
                            {
                                Name = "None",
                                Phone = "*",

                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Owner" },
                                    { nameof(Person.Phone), "Owner_Phone" },
                                }
                            },
                        }
                    },
                    new Address()
                    {
                        Text = "full address ...",

                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Address.Text), "Local_Address" },
                        }
                    }
                }
            };

            var writeMapper = XmlMapper.Build(writeData, "Information");

            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                writeMapper.Write(writer);
            }

            string xmlString = builder.ToString();

            Container readData = new Container()
            {
                DataList = new List<IElement>()
                {
                    new Container()
                    {
                        DataList = new List<IElement>()
                        {
                            new Person()
                            {
                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Self_Name" },
                                    { nameof(Person.Phone), "Self_Phone" },
                                }
                            },
                            new Person()
                            {
                                PropertyNames = new Dictionary<string, string>()
                                {
                                    { nameof(Person.Name), "Owner" },
                                    { nameof(Person.Phone), "Owner_Phone" },
                                }
                            },
                        }
                    },
                    new Address()
                    {
                        PropertyNames = new Dictionary<string, string>()
                        {
                            { nameof(Address.Text), "Local_Address" },
                        }
                    }
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

            Assert.IsNotNull(readData.DataList);
            Assert.That(readData.DataList.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<Container>(readData.DataList[0]);
            Container list = (Container)readData.DataList[0];
            Assert.That(list.DataList.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<Person>(list.DataList[0]);
            Person self = (Person)list.DataList[0];
            Assert.That(self.Name, Is.EqualTo("Emiya"));
            Assert.That(self.Phone, Is.EqualTo("777"));
            Assert.IsInstanceOf<Person>(list.DataList[1]);
            Person owner = (Person)list.DataList[1];
            Assert.That(owner.Name, Is.EqualTo("None"));
            Assert.That(owner.Phone, Is.EqualTo("*"));
            Assert.IsInstanceOf<Address>(readData.DataList[1]);
            Address current = (Address)readData.DataList[1];
            Assert.That(current.Text, Is.EqualTo("full address ..."));
        }
    }
}
