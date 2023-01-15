using System.Globalization;
using System.Reflection;

namespace ThingsGateway.Foundation.Extension
{
    /// <summary>
    /// 为System提供扩展。
    /// </summary>
    public static class SystemExtensions
    {
        #region 其他

        /// <summary>
        /// 安全性释放（不用判断对象是否为空）。不会抛出任何异常。
        /// </summary>
        /// <param name="dis"></param>
        /// <returns></returns>
        public static void SafeDispose(this IDisposable dis)
        {
            if (dis == default)
            {
                return;
            }
            try
            {
                dis.Dispose();
            }
            catch
            {
            }
        }

        #endregion 其他

        /// <summary>
        /// 获取自定义attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum enumObj) where T : Attribute
        {
            Type type = enumObj.GetType();
            Attribute attr = null;
            string enumName = Enum.GetName(type, enumObj);  //获取对应的枚举名
            FieldInfo field = type.GetField(enumName);
            attr = field.GetCustomAttribute(typeof(T), false);
            return (T)attr;
        }

        /// <summary>
        /// 格林尼治标准时间
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string ToGMTString(this DateTime dt, string v)
        {
            return dt.ToString("r", CultureInfo.InvariantCulture);
        }


    }
}