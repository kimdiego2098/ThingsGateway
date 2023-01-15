using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThingsGateway.Foundation.Extension
{
    /// <summary>
    /// TypeExtension
    /// </summary>
    public static class TypeExtensions
    {

        #region Type扩展

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetRefOutType(this Type type)
        {
            if (type.IsByRef)
            {
                return type.GetElementType();
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object GetDefault(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// 判断是否为静态类。
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool IsStatic(this Type targetType)
        {
            return targetType.IsAbstract && targetType.IsSealed;
        }

        /// <summary>
        /// 判断为结构体
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool IsStruct(this Type targetType)
        {
            if (!targetType.IsPrimitive && !targetType.IsClass && !targetType.IsEnum && targetType.IsValueType)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断该类型是否为可空类型
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type theType)
        {
            return theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (TouchSocketCoreUtility.nullableType);
        }

        #endregion Type扩展

        public static Dictionary<string, List<PropertyInfo>> _propertyCache { get; set; } = new Dictionary<string, List<PropertyInfo>>();

        public static List<PropertyInfo> GetAllProperties(this Type self)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                _propertyCache.Add(self.FullName, properties);
                return properties;
            }
            else
            {
                return _propertyCache[self.FullName];
            }
        }

        public static PropertyInfo GetSingleProperty(this Type self, string name)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                _propertyCache.Add(self.FullName, properties);
                return properties.Where(x => x.Name == name).FirstOrDefault();
            }
            else
            {
                return _propertyCache[self.FullName].Where(x => x.Name == name).FirstOrDefault();
            }
        }

        public static PropertyInfo GetSingleProperty(this Type self, Func<PropertyInfo, bool> where)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                _propertyCache.Add(self.FullName, properties);
                return properties.Where(where).FirstOrDefault();
            }
            else
            {
                return _propertyCache[self.FullName].Where(where).FirstOrDefault();
            }
        }

        public static bool IsBool(this Type self)
        {
            return self == typeof(bool);
        }

        /// <summary>
        /// 判断是否是 bool or bool?类型
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsBoolOrNullableBool(this Type self)
        {
            if (self == null)
            {
                return false;
            }
            if (self == typeof(bool) || self == typeof(bool?))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否为枚举
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsEnum(this Type self)
        {
            return self.GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// 判断是否为枚举或者可空枚举
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsEnumOrNullableEnum(this Type self)
        {
            if (self == null)
            {
                return false;
            }
            if (self.IsEnum)
            {
                return true;
            }
            else
            {
                if (self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>) && self.GetGenericArguments()[0].IsEnum)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 判断是否是泛型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <param name="innerType">泛型类型</param>
        /// <returns>判断结果</returns>
        public static bool IsGeneric(this Type self, Type innerType)
        {
            if (self.GetTypeInfo().IsGenericType && self.GetGenericTypeDefinition() == innerType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 判断是否为Nullable<>类型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsNullable(this Type self)
        {
            return self.IsGeneric(typeof(Nullable<>));
        }
        public static bool IsNumber(this Type self)
        {
            Type checktype = self;
            if (self.IsNullable())
            {
                checktype = self.GetGenericArguments()[0];
            }
            if (checktype == typeof(int) || checktype == typeof(short) || checktype == typeof(long) || checktype == typeof(float) || checktype == typeof(decimal) || checktype == typeof(double))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否为值类型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsPrimitive(this Type self)
        {
            return self.GetTypeInfo().IsPrimitive || self == typeof(decimal);
        }




    }



}