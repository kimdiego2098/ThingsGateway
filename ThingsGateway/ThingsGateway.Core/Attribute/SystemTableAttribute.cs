namespace ThingsGateway.Core;

/// <summary>
/// 非实体表特性
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class SystemTableAttribute : Attribute
{
}