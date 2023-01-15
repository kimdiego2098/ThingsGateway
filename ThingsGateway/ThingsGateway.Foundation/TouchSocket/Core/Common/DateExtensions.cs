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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// DateExtensions
    /// </summary>
    public static class DateExtensions
    {
        private static readonly DateTime m_utc_time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 将时间转为毫秒级别的短整形
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertTime(this in DateTime time)
        {
            return (uint)(Convert.ToInt64(time.Subtract(m_utc_time).TotalMilliseconds) & 0xffffffff);
        }

        private static readonly DateTimeOffset m_utc1970 = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// 将时间转为毫秒级别的短整形
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertTime(this in DateTimeOffset time)
        {
            return (uint)(Convert.ToInt64(time.Subtract(m_utc1970).TotalMilliseconds) & 0xffffffff);
        }
    }
}