namespace ThingsGateway.Application.Core;


/// <summary>
/// 数据类型
/// </summary>
public enum CoreDataType
{
    Object,

    Bcd,
    DateTime,
    String,

    Bool,
    Byte,
    SByte,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Float,
    Double,
}
/// <summary>
/// 数据类型信息
/// </summary>
public class DataTypeItem
{
    /// <summary>
    /// 数据类型字典静态集合
    /// </summary>
    public static Dictionary<CoreDataType, DataTypeItem> DictTypes = new Dictionary<CoreDataType, DataTypeItem>()
        {

            { CoreDataType.Object,new DataTypeItem(CoreDataType.Object,0,typeof(object))},
            { CoreDataType.Bool,new DataTypeItem(CoreDataType.Bool,-1,typeof(bool))},
            { CoreDataType.Bcd,new DataTypeItem(CoreDataType.Bcd,-1,typeof(string))},
            { CoreDataType.DateTime,new DataTypeItem(CoreDataType.DateTime,0,typeof(DateTime))},
            { CoreDataType.String,new DataTypeItem(CoreDataType.String,-1,typeof(string))},
            { CoreDataType.Byte,new DataTypeItem(CoreDataType.Byte,1,typeof(byte))},
            { CoreDataType.Double,new DataTypeItem(CoreDataType.Double,8,typeof(double))},
            { CoreDataType.Float,new DataTypeItem(CoreDataType.Float,4,typeof(float))},
            { CoreDataType.Int16,new DataTypeItem(CoreDataType.Int16,2,typeof(short))},
            { CoreDataType.Int32,new DataTypeItem(CoreDataType.Int32,4,typeof(int))},
            { CoreDataType.Int64,new DataTypeItem(CoreDataType.Int64,8,typeof(long))},
            { CoreDataType.SByte,new DataTypeItem(CoreDataType.SByte,1,typeof(sbyte))},
            { CoreDataType.UInt16,new DataTypeItem(CoreDataType.UInt16,2,typeof(ushort))},
            { CoreDataType.UInt32,new DataTypeItem(CoreDataType.UInt32,4,typeof(uint))},
            { CoreDataType.UInt64,new DataTypeItem(CoreDataType.UInt64,8,typeof(ulong))},

        };

    public DataTypeItem(CoreDataType coreDataType, int length, Type type)
    {
        CoreDataType = coreDataType;
        Length = length;
        Type = type;
    }

    public Type Type;

    /// <summary>
    /// 类型长度
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public CoreDataType CoreDataType { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return CoreDataType.ToString();
    }

}