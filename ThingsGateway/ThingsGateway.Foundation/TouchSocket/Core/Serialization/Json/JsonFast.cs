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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// Json高速转换
    /// <para>此代码来源：https://gitee.com/majorworld</para>
    /// </summary>
    public static class JsonFastConverter
    {
        /// <summary>
        /// 全局时间序列化样式，默认为yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string TimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 将Json字符串转为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T JsonFrom<T>(string s) where T : class
        {
            return (T)JsonFrom(s, typeof(T));
        }

        /// <summary>
        /// 将Json字符串转为指定类型
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static object JsonFrom(string s, Type t)
        {
            if (s == null)
                throw new NullReferenceException("不能为null");
            StringBuilder sb = new StringBuilder(s.Length);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<object> list = new List<object>();
            int index = 0;
            switch (s[0])
            {
                case '{':
                    return s.GetObject(ref index, sb, list).ToObject(t);

                case '[':
                    var data = s.GetArray(ref index, sb, dict);
                    if (t == typeof(DataTable))
                        return data.ToDataTable();//处理表类型
                    if (t.GetGenericArguments().Length < 1)
                    {
                        if (t.IsArray)
                            JsonChange.ChangeArray(t.GetElementType(), data);//处理数组类型
                        return JsonChange.ChangeData(t, data);//处理动态类型
                    }
                    if (t.IsList())
                    {
                        return JsonChange.ChangeList(t.GetGenericArguments()[0], data);//处理集合类型
                    }
                    throw new NullReferenceException("类型应该为集合或数组或dynamic");
                default:
                    throw new NullReferenceException("第一个字符缺失{或[");
            }
        }

        /// <summary>
        /// 将Json字符串转为对象<br/>
        /// 重载，当前数据类型速度最快
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonFrom(string s)
        {
            return JsonFrom<Dictionary<string, object>>(s);
        }

        /// <summary>
        /// 将对象转为Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="timeFormat">时间序列化样式，默认为yyyy-MM-dd HH:mm:ss</param>
        /// <returns></returns>
        public static string JsonTo<T>(T t, string timeFormat = null)
        {
            StringBuilder sb = new StringBuilder();
            t.CodeObject(sb, timeFormat ?? TimeFormat);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 过滤不需要序列化的字段
    /// </summary>
    public class JsonFastIgnore : Attribute
    { }

    /// <summary>
    /// 转换为实体类对应的类型
    /// </summary>
    internal static class JsonChange
    {
        #region 类型判断

        internal static bool IsDictionary(this Type type) => typeof(IDictionary).IsAssignableFrom(type);

        internal static bool IsList(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        #endregion 类型判断

        #region 转换

        /// <summary>
        /// 处理数组类型
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static object ChangeArray(Type type, IList data)
        {
            var array = Array.CreateInstance(type, data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                array.SetValue(type.ChangeData(data[i]), i);
            }
            return array;
        }

        /// <summary>
        /// 转换ArrayList类型
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ArrayList ChangeArrayList(object value)
        {
            ArrayList array = new ArrayList();
            if (value is IList list)
            {
                foreach (var item in list)
                    array.Add(item);
            }
            return array;
        }

        /// <summary>
        /// 转换Color类型
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static Color ChangeColor(string s)
        {
            if (s[0] == '#')
                return Color.FromArgb(Convert.ToInt32(s.Substring(1), 16));
            var c = s.Split(',');
            if (c.Length == 3)
                return Color.FromArgb(int.Parse(c[0]), int.Parse(c[1]), int.Parse(c[2]));
            else if (c.Length == 4)
                return Color.FromArgb(int.Parse(c[0]), int.Parse(c[1]), int.Parse(c[2]), int.Parse(c[3]));
            return Color.FromName(s);
        }

        /// <summary>
        /// 转换为各种数据
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="p"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static object ChangeData(this Type p, object value)
        {
            if (value is Dictionary<string, object> dictionary)
            {
                return ToObject(dictionary, p);//解析子级实体类的数据
            }
            else if (p == typeof(string))
            {
                return Convert.ChangeType(value, p);
            }
            else if (p.IsPrimitive && p != typeof(char))
            {
                return Convert.ChangeType(value, p);
            }
            else if (p == typeof(byte[]))
            {
                return Convert.FromBase64String(value as string);
            }
            else if (p == typeof(Color))
            {
                return ChangeColor(value as string);
            }
            else if (p == typeof(Point))
            {
                return ChangePoint(value as string);
            }
            else if (p == typeof(Guid))
            {
                return Guid.Parse(value as string);
            }
            else if (p == typeof(ArrayList))
            {
                return ChangeArrayList(value);//处理动态数组类型
            }
            else if (value is IList list)
            {
                if (p.GetGenericArguments().Length < 1)
                {
                    if (p.IsArray)
                        return ChangeArray(p.GetElementType(), value as IList);//处理数组类型
                    List<dynamic> d = new List<dynamic>();
                    foreach (var kv in list as dynamic)
                    {
                        if (kv is Dictionary<string, object> dict)
                            d.Add(dict.ToObject(typeof(object)));
                    }
                    return d;//解析dynamic
                }
                return ChangeList(p.GetGenericArguments()[0], value as List<object>);//处理List类型
            }
            return Convert.ChangeType(value, p);
        }

        /// <summary>
        /// 处理字典类型
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        internal static object ChangeDictionary(Type type, Dictionary<string, object> dict)
        {
            //反射创建泛型字典
            var t = typeof(Dictionary<,>).MakeGenericType(new[] { type.GetGenericArguments()[0], type.GetGenericArguments()[1] }); ;
            var d = Activator.CreateInstance(t) as IDictionary;
            foreach (var item in dict)
                d.Add(type.GetGenericArguments()[0].ChangeData(item.Key), type.GetGenericArguments()[1].ChangeData(item.Value));
            return d;
        }

        /// <summary>
        /// 处理集合类型
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static object ChangeList(Type type, List<object> data)
        {
            IList list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;
            foreach (var item in data)
                list.Add(type.ChangeData(item));
            return list;
        }

        /// <summary>
        /// 转换Point类型
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static Point ChangePoint(string s)
        {
            var c = s.Split(',');
            return new Point(int.Parse(c[0]), int.Parse(c[1]));
        }

        /// <summary>
        /// List转DataTable
        /// https://gitee.com/majorworld
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static DataTable ToDataTable<T>(this List<T> list)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> dict = list[i] as Dictionary<string, object>;
                if (i == 0)
                {
                    foreach (var item in dict)
                    {
                        dt.Columns.Add(item.Key, item.Value.GetType());
                    }
                }
                dt.Rows.Add(dict.Values.ToArray());
            }
            return dt;
        }

        /// <summary>
        /// 转换为对象
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object ToObject(this Dictionary<string, object> dict, Type type)
        {
            //(1/4)返回原始解析数据
            if (type == typeof(Dictionary<string, object>))
            {
                return dict;
            }
            //(2/4)返回dynamic类型数据
            if (type.UnderlyingSystemType.Name == "Object")
            {
                dynamic d = new ExpandoObject();
                foreach (var kv in dict)
                {
                    if (kv.Value is Dictionary<string, object> dictionary)
                        (d as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(kv.Key, ToObject(dictionary, typeof(object))));
                    else
                        (d as ICollection<KeyValuePair<string, object>>).Add(kv);
                }
                return d;
            }
            //(3/4)返回DataSet类型数据
            if (type == typeof(DataSet))
            {
                DataSet ds = new DataSet();
                foreach (var item in dict)
                {
                    if (item.Value is List<object> list)
                    {
                        var dt = list.ToDataTable();
                        dt.TableName = item.Key;
                        ds.Tables.Add(dt);
                    }
                }
                return ds;
            }
            //(4/4)返回所绑定的实体类数据
            var obj = Activator.CreateInstance(type);
            var props = type.GetCacheInfo();
            foreach (var kv in dict)
            {
                var prop = props.Where(x => string.Equals(x.Name, kv.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (prop is null)
                {
                    if (type.IsDictionary())
                    {
                        return ChangeDictionary(type, dict);//解析值是字典的数据（非缓存字段的字典）
                    }
                    continue;
                }
                if (prop.CanWrite)
                {
                    prop.SetValue(obj, prop.PropertyType.ChangeData(kv.Value), null);//递归调用当前方法，解析子级
                }
            }
            return obj;
        }

        private static object Go<T>(string sValue)
        {
            Type type = typeof(T);
            return System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(sValue.ToString());
        }

        #endregion 转换
    }

    /// <summary>
    /// 解析字符串为对象
    /// </summary>
    internal static class JsonDecode
    {
        #region 解析

        /// <summary>
        /// 解析集合
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sb"></param>
        /// <param name="dict"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static List<object> GetArray(this string s, ref int index, StringBuilder sb, Dictionary<string, object> dict)
        {
            index++;
            List<object> list = new List<object>();
            while (index < s.Length)
            {
                switch (s[index])
                {
                    case ',':
                        index++;
                        break;

                    case '"':
                        list.Add(s.GetString(ref index, sb));
                        break;

                    case ']':
                        ++index;
                        return list;

                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                    case '\f':
                    case '\b':
                        ++index;
                        break;

                    default:
                        list.Add(s.GetData(s[index], ref index, sb, dict, list));
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// 解析对象
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sb"></param>
        /// <param name="list"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static Dictionary<string, object> GetObject(this string s, ref int index, StringBuilder sb, List<object> list)
        {
            index++;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string key = string.Empty;
            bool iskey = true;
            while (index < s.Length)
            {
                switch (s[index])
                {
                    case ',':
                        iskey = true;
                        key = string.Empty;
                        index++;
                        break;

                    case ':':
                        iskey = false;
                        index++;
                        break;

                    case '}':
                        ++index;
                        return dict;

                    case '"':
                        if (iskey)
                            key = s.GetString(ref index, sb);
                        else
                            dict.Add(key, s.GetString(ref index, sb));
                        break;

                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                    case '\f':
                    case '\b':
                        index++;
                        break;

                    default:
                        dict.Add(key, s.GetData(s[index], ref index, sb, dict, list));
                        break;
                }
            }
            throw new FormatException("解析错误，不完整的Json");
        }

        /// <summary>
        /// 获取布尔数据
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool GetBool(this string s, ref int index, bool state)
        {
            if (state)
            {
                if (s[index + 1] == 'r' && s[index + 2] == 'u' && s[index + 3] == 'e')
                {
                    index += 4;
                    return true;
                }
            }
            else
            {
                if (s[index + 1] == 'a' && s[index + 2] == 'l' && s[index + 3] == 's' && s[index + 4] == 'e')
                {
                    index += 5;
                    return false;
                }
            }
            throw new FormatException($"\"{string.Concat(s[index], s[index + 1], s[index + 2], s[index + 3])}\"处Json格式无法解析");
        }

        /// <summary>
        /// 自动获取数据
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <param name="index"></param>
        /// <param name="sb"></param>
        /// <param name="dict"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static object GetData(this string s, char c, ref int index, StringBuilder sb, Dictionary<string, object> dict, List<object> list)
        {
            switch (c)
            {
                case 't':
                    return s.GetBool(ref index, true);

                case 'f':
                    return s.GetBool(ref index, false);

                case 'n':
                    return s.GetNull(ref index);

                case '{':
                    return s.GetObject(ref index, sb, list);

                case '[':
                    return s.GetArray(ref index, sb, dict);

                default:
                    return s.GetNumber(ref index, sb);
            }
        }

        /// <summary>
        /// 获取空数据
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="index"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static object GetNull(this string s, ref int index)
        {
            if (s[index + 1] == 'u' && s[index + 2] == 'l' && s[index + 3] == 'l')
            {
                index += 4;
                return null;
            }
            throw new FormatException($"\"{string.Concat(s[index], s[index + 1], s[index + 2], s[index + 3])}\"处Json格式无法解析");
        }

        /// <summary>
        /// 获取数字数据
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        private static object GetNumber(this string s, ref int index, StringBuilder sb)
        {
            sb.Clear();
            for (; index < s.Length; ++index)
            {
                if (s[index] == ',' || s[index] == '}' || s[index] == ']' || s[index] == ' ' || s[index] == '\n' || s[index] == '\r')
                    break;
                else
                    sb.Append(s[index]);
            }
            string code = sb.ToString();
            if (long.TryParse(code, out long x))
                return x;
            if (double.TryParse(code, out double y))
                return y;
            throw new FormatException($"\"{code}\"处Json格式无法解析");
        }

        /// <summary>
        /// 获取字符串数据
        /// https://gitee.com/majorworld
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sb"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string GetString(this string s, ref int index, StringBuilder sb)
        {
            sb.Clear();
            index++;
            for (; index < s.Length; ++index)
            {
                switch (s[index])
                {
                    case '"':
                        index++;
                        return sb.ToString();

                    case '\\':
                        if (s[index + 1] == '"' || s[index + 1] == '\\')
                            index++;
                        sb.Append(s[index]);
                        break;

                    default:
                        sb.Append(s[index]);
                        break;
                }
            }
            throw new FormatException($"\"{sb}\"处Json格式无法解析");
        }

        #endregion 解析
    }

    /// <summary>
    /// 编码对象为字符串
    /// </summary>
    internal static class JsonEncode
    {
        /// <summary>
        /// 缓存数据加速序列化速度，主要是减少不必要的GetCustomAttributes获取特性并过滤字段
        /// </summary>
        internal static ConcurrentDictionary<Type, PropertyInfo[]> InfoCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sb"></param>
        /// <param name="timeFormat"></param>
        internal static void CodeObject(this object obj, StringBuilder sb, string timeFormat)
        {
            bool get = true;
            switch (obj)
            {
                case null:
                    sb.Append("null");
                    break;

                case Enum _:
                    sb.Append($"{Convert.ToInt32(obj)}");
                    break;

                case byte[] bytes:
                    sb.Append($"\"{Convert.ToBase64String(bytes)}\"");
                    break;

                case Array array:
                    sb.Append('[');
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (i != 0)
                            sb.Append(",");
                        array.GetValue(i).CodeObject(sb, timeFormat);
                    }
                    sb.Append(']');
                    break;

                case string _:
                    sb.Append($"\"{obj}\"");
                    break;

                case char _:
                    sb.Append($"\"{obj}\"");
                    break;

                case bool _:
                    sb.Append($"{obj.ToString().ToLower()}");
                    break;

                case DataTable dt:
                    dt.CodeDataTable(sb, timeFormat);
                    break;

                case DataSet ds:
                    sb.Append('{');
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        if (i != 0)
                            sb.Append(",");
                        sb.Append($"\"{ds.Tables[i].TableName}\":");
                        ds.Tables[i].CodeDataTable(sb, timeFormat);
                    }
                    sb.Append('}');
                    break;

                case DateTime time:
                    sb.AppendFormat($"\"{time.ToString(timeFormat)}\"");
                    break;

                case Guid id:
                    sb.AppendFormat($"\"{id}\"");
                    break;

                case Color color:
                    if (color.A == 255)
                        sb.Append($"\"{color.R},{color.G},{color.B}\"");
                    else
                        sb.Append($"\"{color.A},{color.R},{color.G},{color.B}\"");
                    break;

                case Point point:
                    sb.Append($"\"{point.X},{point.Y}\"");
                    break;

                case ArrayList list:
                    sb.Append('[');
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i != 0)
                            sb.Append(",");
                        list[i].CodeObject(sb, timeFormat);
                    }
                    sb.Append(']');
                    break;

                default:
                    get = false;
                    break;
            }
            if (get)
                return;
            Type type = obj.GetType();
            //数字
            if (type.IsPrimitive && type != typeof(char))
            {
                sb.Append($"{obj}");
                return;
            }
            //字典
            else if (type.IsDictionary())
            {
                sb.Append('{');
                var collection = obj as IDictionary;
                var enumerator = collection.GetEnumerator();
                int index = 0;
                while (enumerator.MoveNext())
                {
                    if (index != 0)
                        sb.Append(",");
                    sb.Append($"\"{enumerator.Key}\":");
                    enumerator.Value.CodeObject(sb, timeFormat);
                    index++;
                }
                sb.Append('}');
                return;
            }
            //集合
            else if (type.IsList())
            {
                sb.Append('[');
                if (obj is IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i != 0)
                            sb.Append(",");
                        list[i].CodeObject(sb, timeFormat);
                    }
                }
                sb.Append(']');
                return;
            }
            else if (type.UnderlyingSystemType.Name == "ExpandoObject")
            {
                sb.Append('{');
                bool first = true;
                foreach (dynamic item in obj as dynamic)
                {
                    if (!first)
                        sb.Append(',');
                    first = false;
                    object value = item.Value;
                    sb.Append($"\"{item.Key}\":");
                    value.CodeObject(sb, timeFormat);
                }
                sb.Append('}');
                return;
            }

            //对象
            var prop = type.GetCacheInfo();
            if (prop is null)
            {
                sb.Append("null");
                return;
            }
            sb.Append('{');
            for (int i = 0; i < prop.Length; i++)
            {
                PropertyInfo p = prop[i];
                if (i != 0)
                    sb.Append(",");
                var data = p.GetValue(obj, null);
                sb.Append($"\"{p.Name}\":");
                data.CodeObject(sb, timeFormat);
            }
            sb.Append("}");
        }

        /// <summary>
        /// 尝试获取缓存中的类型，排除忽略的字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static PropertyInfo[] GetCacheInfo(this Type type)
        {
            if (InfoCache.TryGetValue(type, out PropertyInfo[] props)) { }
            else
            {
                props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                List<PropertyInfo> cache = new List<PropertyInfo>();
                foreach (var item in props)
                {
                    if (Attribute.GetCustomAttributes(item, typeof(JsonFastIgnore))?.Length == 0)
                        cache.Add(item);
                }
                InfoCache[type] = cache.ToArray();
                props = cache.ToArray();
            }
            return props;
        }

        /// <summary>
        /// 序列化DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sb"></param>
        /// <param name="timeFormat">时间格式化样式，默认为yyyy-MM-dd HH:mm:ss</param>
        private static void CodeDataTable(this DataTable dt, StringBuilder sb, string timeFormat)
        {
            sb.Append('[');
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i != 0)
                    sb.Append(",");
                var item = dt.Rows[i];
                sb.Append('{');
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j != 0)
                        sb.Append(",");
                    var cell = dt.Columns[j];
                    sb.Append($"\"{cell}\":");
                    item[j].CodeObject(sb, timeFormat);
                }
                sb.Append('}');
            }
            sb.Append(']');
        }
    }
}