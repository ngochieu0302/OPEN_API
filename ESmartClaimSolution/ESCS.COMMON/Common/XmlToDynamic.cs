using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ESCS.COMMON.Common
{
    public class XmlToDynamic
    {
        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }
                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.LocalName.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.LocalName.ToString(), attribute.Value.Trim());
                    }
                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }
                    AddProperty(parent, node.Name.LocalName.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.LocalName.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }
    }
}
