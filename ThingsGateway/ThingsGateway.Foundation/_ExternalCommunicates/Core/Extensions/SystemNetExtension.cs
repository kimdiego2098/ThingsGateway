

using System.Net;

namespace ThingsGateway.Foundation.Extension
{
    /// <summary>
    /// SystemNet扩展
    /// </summary>
    public static class SystemNetExtension
    {
        /// <summary>
        /// 从<see cref="EndPoint"/>中获得IP地址。
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static string GetIP(this EndPoint endPoint)
        {
            int r = endPoint.ToString().LastIndexOf(":");
            return endPoint.ToString().Substring(0, r);
        }

        /// <summary>
        /// 从<see cref="EndPoint"/>中获得Port。
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static int GetPort(this EndPoint endPoint)
        {
            int r = endPoint.ToString().LastIndexOf(":");
            return Convert.ToInt32(endPoint.ToString().Substring(r + 1, endPoint.ToString().Length - (r + 1)));
        }
    }
}
