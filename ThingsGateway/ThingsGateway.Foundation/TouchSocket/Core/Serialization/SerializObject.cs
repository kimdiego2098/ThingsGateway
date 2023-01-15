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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThingsGateway.Foundation.Core
{
    internal class SerializObject
    {
        private MemberInfo[] m_MemberInfos;
        private FieldInfo[] m_fieldInfos;
        private PropertyInfo[] m_properties;
        public IFastBinaryConverter Converter { get; set; }
        public SerializObject(Type type)
        {
            Type = type;
            if (type.IsArray)//数组
            {
                this.InstanceType = InstanceType.Array;
                this.ArrayType = type.GetElementType();
            }
            else if (type.IsClass || type.IsStruct())
            {
                if (type.IsNullableType())
                {
                    type = type.GetGenericArguments()[0];
                }
                this.Type = type;
                if (TouchSocketCoreUtility.listType.IsAssignableFrom(type))
                {
                    Type genericType = type;
                    while (true)
                    {
                        if (genericType.IsGenericType)
                        {
                            break;
                        }
                        genericType = genericType.BaseType;
                        if (genericType == TouchSocketCoreUtility.objType)
                        {
                            break;
                        }
                    }
                    this.ArgTypes = genericType.GetGenericArguments();
                    this.AddMethod = new Method(type.GetMethod("Add"));
                    this.InstanceType = InstanceType.List;
                }
                else if (TouchSocketCoreUtility.dicType.IsAssignableFrom(type))
                {
                    Type genericType = type;
                    while (true)
                    {
                        if (genericType.IsGenericType)
                        {
                            break;
                        }
                        genericType = genericType.BaseType;
                        if (genericType == TouchSocketCoreUtility.objType)
                        {
                            break;
                        }
                    }
                    this.ArgTypes = genericType.GetGenericArguments();
                    this.AddMethod = new Method(type.GetMethod("Add"));
                    this.InstanceType = InstanceType.Dictionary;
                }
                else
                {
                    this.InstanceType = InstanceType.Class;
                    MemberAccessor = new MemberAccessor(type)
                    {
                        OnGetFieldInfes = GetFieldInfos,
                        OnGetProperties = GetProperties
                    };
                    MemberAccessor.Build();
                }
                if (type.GetCustomAttribute<FastConverterAttribute>() is FastConverterAttribute attribute)
                {
                    this.Converter = (IFastBinaryConverter)Activator.CreateInstance(attribute.Type);
                }
                this.PropertiesDic = GetProperties(type).ToDictionary(a => a.Name);
                this.FieldInfosDic = GetFieldInfos(type).ToDictionary(a => a.Name);
                if (type.IsGenericType)
                {
                    this.ArgTypes = type.GetGenericArguments();
                }
            }
            this.IsStruct = type.IsStruct();
        }

        public bool IsStruct { get; private set; }

        public Method AddMethod { get; private set; }

        public Type[] ArgTypes { get; private set; }

        public Type ArrayType { get; private set; }

        public FieldInfo[] FieldInfos
        {
            get
            {
                m_fieldInfos ??= FieldInfosDic.Values.ToArray();
                return m_fieldInfos;
            }
        }

        public MemberInfo[] MemberInfos
        {
            get
            {
                if (m_MemberInfos == null)
                {
                    List<MemberInfo> infos = new List<MemberInfo>();
                    infos.AddRange(FieldInfosDic.Values);
                    infos.AddRange(PropertiesDic.Values);
                    m_MemberInfos = infos.ToArray();
                }
                return m_MemberInfos;
            }
        }

        public Dictionary<string, FieldInfo> FieldInfosDic { get; private set; }

        public InstanceType InstanceType { get; private set; }

        public MemberAccessor MemberAccessor { get; private set; }

        public PropertyInfo[] Properties
        {
            get
            {
                m_properties ??= PropertiesDic.Values.ToArray();
                return m_properties;
            }
        }

        public Dictionary<string, PropertyInfo> PropertiesDic { get; private set; }

        public Type Type { get; private set; }

        public object GetNewInstance()
        {
            return Activator.CreateInstance(Type);
        }

        private static FieldInfo[] GetFieldInfos(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default)
                .Where(p =>
                {
                    return !p.IsInitOnly && (!p.IsDefined(typeof(FastNonSerializedAttribute), true));
                })
                .ToArray();
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default)
                          .Where(p =>
                          {
                              if (p.IsDefined(typeof(FastSerializedAttribute), true))
                              {
                                  return true;
                              }
                              else
                              {
                                  return p.CanWrite &&
                              p.CanRead &&
                              !p.IsDefined(typeof(FastNonSerializedAttribute), true) &&
                              (p.SetMethod.GetParameters().Length == 1) &&
                              (p.GetMethod.GetParameters().Length == 0);
                              }
                          })
                          .ToArray();
        }
    }
}