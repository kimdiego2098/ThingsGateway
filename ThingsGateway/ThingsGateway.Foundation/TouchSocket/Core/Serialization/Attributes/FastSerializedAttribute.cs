namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 强制Fast序列化。一般当某个属性为只读时，使用该特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastSerializedAttribute : Attribute
    {
    }
}
