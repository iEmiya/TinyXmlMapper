using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace TinyXmlMapper
{
    /// <summary>
    /// Определяет словарь Dictionary[string, string] сопоставление полей объекта их названиям
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyMapAttribute : Attribute {}

    /// <summary>
    /// Пропустить поле
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyIgnoreAttribute : Attribute {}

    /// <summary>
    /// Сохранять без создания root узла
    /// </summary>
    /// <example>
    /// /* not exists
    /// <root>
    ///     <list>
    ///         <item>A</item>
    ///         <item>B</item>
    ///         <item>C</item>
    ///     </list>
    /// </root>
    /// */
    /// 
    /// /* exists
    ///     <list>
    ///         <item>A</item>
    ///         <item>B</item>
    ///         <item>C</item>
    ///     </list>
    /// */
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyWithoutRootAttribute : Attribute { }

    /// <summary>
    /// Сохранять без создания узла
    /// </summary>
    /// <example>
    /// /* not exists
    /// <root>
    ///     <list>
    ///         <item>A</item>
    ///         <item>B</item>
    ///         <item>C</item>
    ///     </list>
    /// </root>
    /// */
    /// 
    /// /* exists
    /// <root>
    ///         <item>A</item>
    ///         <item>B</item>
    ///         <item>C</item>
    /// </root>
    /// */
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyWithoutNodeAttribute : Attribute { }

    /// <summary>
    /// Элементы класса должны быть отмечены как <see cref="PropertyIncludeAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ClassWithIncludeAttribute : Attribute { }

    /// <summary>
    /// Включить поле в выгрузку
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyIncludeAttribute : Attribute { }


    /// <summary>
    /// Имя узла для элементов перечисления
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyItemNameAttribute : Attribute
    {
        private readonly string _itemName;

        public PropertyItemNameAttribute(string itemName)
        {
            _itemName = itemName;
        }

        public string ItemName {  get { return _itemName; } }
    }



    public sealed class XmlMapper
    {
        #region 

        private readonly static Regex LocalNameRegex = new Regex("^[A-z][0-9A-z]*$", RegexOptions.Compiled);

        private delegate string GetValueCallback();
        private delegate void SetValueCallback(string value);

        interface IElement
        {
            string LocalName { get; }
        }

        class Property : IElement
        {
            public Wrapper Parent { get; }
            public string LocalName { get; }
            public GetValueCallback GetValue { get; set; }
            public SetValueCallback SetValue { get; set; }

            public Property(string localName, Wrapper parent)
            {
                if (!LocalNameRegex.IsMatch(localName))
                    throw new ArgumentException($"Element can't be named '{localName}'");

                Parent = parent;
                LocalName = localName;
            }
        }

        class Wrapper : IElement
        {
            public Wrapper Parent { get; }
            public string LocalName { get; }
            public List<IElement> Elements { get; }

            public Wrapper(string localName, Wrapper parent = null)
            {
                if (!LocalNameRegex.IsMatch(localName))
                    throw new ArgumentException($"Element can't be named '{localName}'");

                Parent = parent;
                LocalName = localName;
                Elements = new List<IElement>();
            }
        }

        private void StartElement(string localName)
        {
            Wrapper current = _current;
            if (current == null)
            {
                if (_root != null)
                {
                    throw new ApplicationException("Can't create element after closing 'Root'");
                }
                _root = _current = new Wrapper(localName);
                return;
            }

            Wrapper element = new Wrapper(localName, current);
            current.Elements.Add(element);
            _current = element;
        }

        private void EndElement()
        {
            Wrapper current = _current;
            if (current == _root)
            {
                _current = null;
                return;
            }
            _current = current.Parent;
        }

        #endregion
        #region Build

        public static XmlMapper Build(object that, string rootName = null)
        {
            if (that == null)
                throw new ArgumentNullException(nameof(that));
            if (string.IsNullOrEmpty(rootName)) rootName = that.GetType().Name;
            XmlMapper mapper = new XmlMapper();
            Build(mapper, that, rootName);
            return mapper;
        }

        private static void Build(XmlMapper mapper, object that, string rootName)
        {
            Type type = that.GetType();
            Attribute[] typeAttributes = Attribute.GetCustomAttributes(type);
            Attribute include = typeAttributes.FirstOrDefault(p => p is ClassWithIncludeAttribute);

            List<PropertyInfo> propertyInfos = new List<PropertyInfo>(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            List<PropertyInfo> ignores = new List<PropertyInfo>();

            PropertyInfo propertyMap = null;
            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
                {
                    // 2015-08-31 emiya: Not supported type
                    ignores.Add(propertyInfo);
                    continue;
                }
                object[] attributes = propertyInfo.GetCustomAttributes(true);
                if (attributes.Any(p => p is PropertyMapAttribute))
                {
                    propertyMap = propertyInfo;
                    continue;
                }
                if (attributes.Any(p => p is PropertyIgnoreAttribute))
                {
                    ignores.Add(propertyInfo);
                    continue;
                }
                if (include != null && !attributes.Any(p => p is PropertyIncludeAttribute))
                {
                    ignores.Add(propertyInfo);
                    continue;
                }

                map.Add(propertyInfo.Name, propertyInfo.Name);
            }

            if (propertyMap != null)
            {
                Dictionary<string, string> names = propertyMap.GetValue(that, null) as Dictionary<string, string>;
                if (names != null)
                {
                    propertyInfos.Remove(propertyMap);

                    Dictionary<string, string> merge = names.Concat(map).GroupBy(p => p.Key)
                        .ToDictionary(group => group.Key, group => group.First().Value);

                    map = merge;
                }
            }
            foreach (var ignore in ignores)
            {
                propertyInfos.Remove(ignore);
            }

            if (!string.IsNullOrEmpty(rootName)) mapper.StartElement(rootName);
            foreach (var propertyInfo in propertyInfos)
            {
                string localName;
                if (!map.TryGetValue(propertyInfo.Name, out localName)) continue;

                Type propertyType = propertyInfo.PropertyType;
                if (!propertyType.IsClass || propertyType == typeof(string))
                {
                    mapper.InsertElement(that, propertyInfo, localName);
                    continue;
                }


                object value = propertyInfo.GetValue(that, null);
                object[] attributes = propertyInfo.GetCustomAttributes(true);
                bool isCollection = typeof(IEnumerable).IsAssignableFrom(propertyType);
                bool withoutRoot = attributes.Any(p => p is PropertyWithoutRootAttribute);
                bool withoutNode = attributes.Any(p => p is PropertyWithoutNodeAttribute);
                if (isCollection)
                {
                    if (!withoutRoot) mapper.StartElement(localName);
                    string itemName = null;
                    if (!withoutNode)
                    {
                        PropertyItemNameAttribute propertyItem = (PropertyItemNameAttribute)attributes.FirstOrDefault(p => p is PropertyItemNameAttribute);
                        if (propertyItem == null)
                        {
                            if (propertyType.IsGenericType)
                            {
                                if (propertyType.GetGenericTypeDefinition() == typeof (List<>))
                                {
                                    itemName = propertyType.GetGenericArguments()[0].Name;
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                        }
                        else
                        {
                            itemName = propertyItem.ItemName;
                        }
                    }
                    foreach (object item in ((IEnumerable) value))
                    {
                        Build(mapper, item, itemName);
                    }
                    if (!withoutRoot) mapper.EndElement();
                    continue;
                }


                if (withoutNode) localName = null;
                Build(mapper, value, localName);

            }
            if (!string.IsNullOrEmpty(rootName)) mapper.EndElement();
        }
        private void InsertElement(object that, PropertyInfo propertyInfo, string localName)
        {
            IElement element = _current.Elements.FirstOrDefault(p => p.LocalName.Equals(localName));
            if (element != null)
                throw new ApplicationException($"Element has a property named '{localName}'");

            Property property = new Property(localName, _current);
            property.GetValue = () => (string)propertyInfo.GetValue(that, null);
            property.SetValue = (value) => propertyInfo.SetValue(that, value, null);
            _current.Elements.Add(property);
        }

        #endregion


        private Wrapper _root;
        private Wrapper _current;

        private XmlMapper()
        {
            _current = _root = null;
        }

        #region Read

        public void Read(XmlReader reader)
        {
            Read(reader, _root);
        }

        private static void Read(XmlReader reader, Wrapper element)
        {
            if (!reader.IsStartElement()) return;
            if (reader.IsEmptyElement) return;
            if (reader.Name != element.LocalName) return;
            if (!reader.Read()) return;

            List<IElement> elements = new List<IElement>(element.Elements);
            do
            {
                if (reader.MoveToContent() == XmlNodeType.EndElement && reader.Name.Equals(element.LocalName)) break;

                IElement item = elements.FirstOrDefault(p => reader.Name.Equals(p.LocalName));
                if (item == null) break;

                if (item is Property)
                {
                    Read(reader, item as Property);
                    if (reader.MoveToContent() == XmlNodeType.EndElement && reader.Name.Equals(item.LocalName))
                    {
                        if (!reader.Read()) return;
                    }
                }
                else if (item is Wrapper)
                {
                    Read(reader, item as Wrapper);
                }

                elements.Remove(item);

            } while (elements.Count > 0);

            if (reader.MoveToContent() == XmlNodeType.EndElement && reader.Name.Equals(element.LocalName))
            {
                reader.Read();
            }
        }

        private static void Read(XmlReader reader, Property element)
        {
            if (!reader.IsStartElement()) return;
            if (reader.IsEmptyElement) return;
            if (reader.Name != element.LocalName) return;
            if (!reader.Read()) return;
            string value = reader.Value;
            element.SetValue(value);
            reader.Read();
        }

        #endregion
        #region Write

        public void Write(XmlWriter writer)
        {
            writer.WriteStartDocument();
            Write(writer, _root);
            writer.WriteEndDocument();
        }

        private static void Write(XmlWriter writer, Wrapper element)
        {
            writer.WriteStartElement(element.LocalName);
            foreach (var item in element.Elements)
            {
                if (item is Property)
                {
                    Write(writer, item as Property);
                }
                else if (item is Wrapper)
                {
                    Write(writer, item as Wrapper);
                }
            }
            writer.WriteEndElement();
        }

        private static void Write(XmlWriter writer, Property element)
        {
            string value = element.GetValue();
            if (string.IsNullOrEmpty(value)) return;
            writer.WriteElementString(element.LocalName, value);
        }
        #endregion
    }
}
