namespace ThingsGateway.Core;

public class DateTimeUtil
{
    /// <summary>
    /// 获取开始时间
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateTime GetBeginTime(DateTime? dateTime, int days = 0)
    {
        if (dateTime == DateTime.MinValue || dateTime == null)
            return DateTime.Now.AddDays(days);

        return dateTime ?? DateTime.Now;
    }

    /// <summary>
    ///  时间戳转本地时间-时间戳精确到秒
    /// </summary>
    public static DateTime ToLocalTimeDateBySeconds(long unix)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unix).ToLocalTime().DateTime;
    }

    /// <summary>
    ///  时间转时间戳Unix-时间戳精确到秒
    /// </summary>
    public static long ToUnixTimestampBySeconds(DateTime dt)
    {
        return new DateTimeOffset(dt).ToUnixTimeSeconds();
    }

    /// <summary>
    ///  时间戳转本地时间-时间戳精确到毫秒
    /// </summary>
    public static DateTime ToLocalTimeDateByMilliseconds(long unix)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(unix).ToLocalTime().DateTime;
    }

    /// <summary>
    ///  时间转时间戳Unix-时间戳精确到毫秒
    /// </summary>
    public static long ToUnixTimestampByMilliseconds(DateTime dt)
    {
        return new DateTimeOffset(dt).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 毫秒转天时分秒
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static string FormatTime(long ms)
    {
        int ss = 1000;
        int mi = ss * 60;
        int hh = mi * 60;
        int dd = hh * 24;

        long day = ms / dd;
        long hour = (ms - (day * dd)) / hh;
        long minute = (ms - (day * dd) - (hour * hh)) / mi;
        long second = (ms - (day * dd) - (hour * hh) - (minute * mi)) / ss;
        long milliSecond = ms - (day * dd) - (hour * hh) - (minute * mi) - (second * ss);

        string sDay = day < 10 ? "0" + day : "" + day; //天
        string sHour = hour < 10 ? "0" + hour : "" + hour;//小时
        string sMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
        string sSecond = second < 10 ? "0" + second : "" + second;//秒
        string sMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒
        sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : "" + sMilliSecond;

        return string.Format("{0} 天 {1} 小时 {2} 分 {3} 秒", sDay, sHour, sMinute, sSecond);
    }

    /// <summary>
    /// 获取unix时间戳
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static long GetUnixTimeStamp(DateTime dt)
    {
        return ((DateTimeOffset)dt).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取日期天的最小时间
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static DateTime GetDayMinDate(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
    }

    /// <summary>
    /// 获取日期天的最大时间
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>

    public static DateTime GetDayMaxDate(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
    }

    /// <summary>
    /// 获取日期天的最大时间
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string FormatDateTime(DateTime? dt)
    {
        if (dt == null) return string.Empty;

        if (dt.Value.Year == DateTime.Now.Year)
            return dt.Value.ToString("MM-dd HH:mm");
        else
            return dt.Value.ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// 获取今天日期范围00:00:00 - 23:59:59
    /// </summary>
    /// <returns></returns>
    public static List<DateTime> GetTodayTimeList(DateTime time)
    {
        return new List<DateTime>
        {
            Convert.ToDateTime(time.ToString("D").ToString()),
            Convert.ToDateTime(time.AddDays(1).ToString("D").ToString()).AddSeconds(-1)
        };
    }

    /// <summary>
    /// 获取星期几
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string GetWeekByDate(DateTime dt)
    {
        var day = new[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        return day[Convert.ToInt32(dt.DayOfWeek.ToString("d"))];
    }

    /// <summary>
    /// 获取这个月的第几周
    /// </summary>
    /// <param name="daytime"></param>
    /// <returns></returns>
    public static int GetWeekNumInMonth(DateTime daytime)
    {
        int dayInMonth = daytime.Day;
        // 本月第一天
        DateTime firstDay = daytime.AddDays(1 - daytime.Day);
        // 本月第一天是周几
        int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
        // 本月第一周有几天
        int firstWeekEndDay = 7 - (weekday - 1);
        // 当前日期和第一周之差
        int diffday = dayInMonth - firstWeekEndDay;
        diffday = diffday > 0 ? diffday : 1;
        // 当前是第几周，若整除7就减一天
        return ((diffday % 7) == 0 ? ((diffday / 7) - 1) : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
    }
}