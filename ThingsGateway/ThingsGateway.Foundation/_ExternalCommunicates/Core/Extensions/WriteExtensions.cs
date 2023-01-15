namespace ThingsGateway.Foundation.Extension;

public static class WriteExtensions
{
    /// <summary>
    /// 写入
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static void Write<T>(this T writer, byte[] buffer) where T : IWrite
    {
        writer.Write(buffer, 0, buffer.Length);
    }
}
