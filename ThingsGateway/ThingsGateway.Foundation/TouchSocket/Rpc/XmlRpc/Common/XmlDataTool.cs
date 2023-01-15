//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using System.Collections;
using System.Reflection;
using System.Text;


using System.Xml;

namespace ThingsGateway.Foundation.Rpc.XmlRpc
{
    internal static class XmlDataTool
    {
        public static object GetValue(XmlNode valueNode, Type type)
        {
            if (valueNode == null)
            {
                return type.GetDefault();
            }
            switch (valueNode.Name)
            {
                case "boolean":
                    {
                        return bool.Parse(valueNode.InnerText);
                    }
                case "i4":
                case "int":
                    {
                        return int.Parse(valueNode.InnerText);
                    }
                case "double":
                    {
                        return double.Parse(valueNode.InnerText);
                    }
                case "dateTime.iso8601":
                    {
                        return DateTime.Parse(valueNode.InnerText);
                    }
                case "base64":
                    {
                        return valueNode.InnerText;
                    }
                case "struct":
                    {
                        object instance = Activator.CreateInstance(type);
                        foreach (XmlNode memberNode in valueNode.ChildNodes)
                        {
                            string name = memberNode.SelectSingleNode("name").InnerText;
                            PropertyInfo property = type.GetProperty(name);
                            property.SetValue(instance, GetValue(memberNode.SelectSingleNode("value").FirstChild, property.PropertyType));
                        }
                        return instance;
                    }
                case "arrays":
                case "array":
                    {
                        if (type.GetElementType() != null)
                        {
                            XmlNode dataNode = valueNode.SelectSingleNode("data");
                            Array array = Array.CreateInstance(type.GetElementType(), dataNode.ChildNodes.Count);

                            int index = 0;
                            foreach (XmlNode arrayValueNode in dataNode.ChildNodes)
                            {
                                array.SetValue(GetValue(arrayValueNode.FirstChild, type.GetElementType()), index);
                                index++;
                            }
                            return array;
                        }
                        else if (type.GetGenericArguments().Length == 1)
                        {
                            XmlNode dataNode = valueNode.SelectSingleNode("data");
                            IList array = (IList)Activator.CreateInstance(type);

                            foreach (XmlNode arrayValueNode in dataNode.ChildNodes)
                            {
                                array.Add(GetValue(arrayValueNode.FirstChild, type.GetGenericArguments()[0]));
                            }
                            return array;
                        }
                        return type.GetDefault();
                    }
                default:
                case "string":
                    {
                        return valueNode.InnerText;
                    }
            }
        }

        public static HttpRequest CreateRequest(string host, string url, string method, object[] parameters)
        {
            XmlDocument xml = new XmlDocument();

            XmlDeclaration xmlDecl = xml.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
            xml.AppendChild(xmlDecl);

            XmlElement xmlElement = xml.CreateElement("methodCall");
            xml.AppendChild(xmlElement);

            XmlElement methodNameElement = xml.CreateElement("methodName");
            methodNameElement.InnerText = method;
            xmlElement.AppendChild(methodNameElement);

            XmlElement paramsElement = xml.CreateElement("params");
            xmlElement.AppendChild(paramsElement);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    XmlElement paramElement = xml.CreateElement("param");
                    paramsElement.AppendChild(paramElement);

                    XmlElement valueElement = xml.CreateElement("value");
                    paramElement.AppendChild(valueElement);

                    CreateParam(xml, valueElement, param);
                }
            }

            HttpRequest request = new HttpRequest();
            request.FromXML(xml.OuterXml)
                .InitHeaders()
                .SetUrl(url)
                .SetHost(host)
                .AsPost();
            return request;
        }

        public static void CreateParam(XmlDocument xml, XmlNode xmlNode, object value)
        {
            if (value == null)
            {
                return;
            }
            if (value is int)
            {
                XmlElement valueElement = xml.CreateElement("i4");
                valueElement.InnerText = value.ToString();
                xmlNode.AppendChild(valueElement);
            }
            else if (value is bool)
            {
                XmlElement valueElement = xml.CreateElement("boolean");
                valueElement.InnerText = value.ToString();
                xmlNode.AppendChild(valueElement);
            }
            else if (value is double)
            {
                XmlElement valueElement = xml.CreateElement("double");
                valueElement.InnerText = ((double)value).ToString();
                xmlNode.AppendChild(valueElement);
            }
            else if (value is string)
            {
                XmlElement valueElement = xml.CreateElement("string");
                valueElement.InnerText = value.ToString();
                xmlNode.AppendChild(valueElement);
            }
            else if (value is DateTime)
            {
                XmlElement valueElement = xml.CreateElement("dateTime.iso8601");
                valueElement.InnerText = ((DateTime)value).ToString();
                xmlNode.AppendChild(valueElement);
            }
            else if (value is byte[])
            {
                XmlElement valueElement = xml.CreateElement("base64");
                string str = Convert.ToBase64String((byte[])value);
                valueElement.InnerText = str;
                xmlNode.AppendChild(valueElement);
            }
            else if (typeof(IList).IsAssignableFrom(value.GetType()))
            {
                IList array = (IList)value;
                XmlElement arrayElement;

                arrayElement = xml.CreateElement("array");

                xmlNode.AppendChild(arrayElement);

                XmlElement dataElememt = xml.CreateElement("data");
                arrayElement.AppendChild(dataElememt);

                foreach (var item in array)
                {
                    XmlElement valueElement = xml.CreateElement("value");
                    dataElememt.AppendChild(valueElement);
                    CreateParam(xml, valueElement, item);
                }
            }
            else
            {
                XmlElement valueElement = xml.CreateElement("struct");
                xmlNode.AppendChild(valueElement);

                PropertyInfo[] propertyInfos = value.GetType().GetProperties();
                foreach (var propertyInfo in propertyInfos)
                {
                    XmlElement memberElement = xml.CreateElement("member");
                    valueElement.AppendChild(memberElement);

                    XmlElement nameElement = xml.CreateElement("name");
                    nameElement.InnerText = propertyInfo.Name;
                    memberElement.AppendChild(nameElement);

                    XmlElement oValueElement = xml.CreateElement("value");
                    memberElement.AppendChild(oValueElement);

                    object oValue = propertyInfo.GetValue(value);
                    CreateParam(xml, oValueElement, oValue);
                }
            }
        }

        public static void CreatResponse(HttpResponse httpResponse, object value)
        {
            XmlDocument xml = new XmlDocument();

            XmlDeclaration xmlDecl = xml.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
            xml.AppendChild(xmlDecl);

            XmlElement xmlElement = xml.CreateElement("methodResponse");

            xml.AppendChild(xmlElement);

            XmlElement paramsElement = xml.CreateElement("params");
            xmlElement.AppendChild(paramsElement);

            XmlElement paramElement = xml.CreateElement("param");
            paramsElement.AppendChild(paramElement);

            XmlElement valueElement = xml.CreateElement("value");
            paramElement.AppendChild(valueElement);

            CreateParam(xml, valueElement, value);

            ByteBlock xmlBlock = BytePool.GetByteBlock(1024 * 4);
            xml.Save(xmlBlock);

            string xmlString = Encoding.UTF8.GetString(xmlBlock.Buffer, 0, xmlBlock.Len);

            httpResponse.FromXML(xmlString);
            xmlBlock.Dispose();
        }
    }
}