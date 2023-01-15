
using Newtonsoft.Json.Linq;

using System.Text;

namespace ThingsGateway.Foundation.Extension
{
    /// <summary>
    /// JsonExtension
    /// </summary>
    public static class JsonExtensions
    {
        public static string FormatJson(this string json) { return JToken.Parse(json).ToString(); }
        #region Json转换

        /// <summary>
        /// 序列化成Json数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToJsonBytes(this object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None));
        }

        /// <summary>
        /// 转换为json字符串。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }
        /// <summary>
        /// 转换为json字符串。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonStringWithSettings(this object obj, JsonSerializerSettings jsonSerializerSettings)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, jsonSerializerSettings);
        }
        /// <summary>
        ///  反序列化成Json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static T ToJsonObject<T>(this byte[] buffer, int offset, int len)
        {
            return Encoding.UTF8.GetString(buffer, offset, len).ToJsonObject<T>();
        }

        /// <summary>
        ///  反序列化成Json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T ToJsonObject<T>(this byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer).ToJsonObject<T>();
        }

        /// <summary>
        ///  反序列化成Json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T ToJsonObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 反序列化成Json数据
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToJsonObject(this string jsonString, Type type)
        {
            return JsonConvert.DeserializeObject(jsonString, type);
        }

        #endregion Json转换
    }
}