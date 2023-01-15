namespace ThingsGateway.Application.Core
{
    public interface IDeviceVariable
    {
        public long Id { get; set; }

        public DateTime? CreateTime { get; set; }


        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 创建者Id
        /// </summary>
        public long? CreateUserId { get; set; }


        public long? UpdateUserId { get; set; }


        public bool IsDelete { get; set; }


        IThingsGatewayBitConverter ThingsGatewayBitConverter { get; set; }
        DateTime ChangeTime { get; set; }
        DateTime CollectTime { get; set; }
        Device Device { get; set; }
        long DeviceId { get; set; }
        string DeviceName { get; set; }
        string Expressions { get; set; }
        int Index { get; set; }
        int InvokeInterval { get; set; }
        object ObjectTag { get; set; }
        string OtherMethod { get; set; }
        int Quality { get; set; }
        BCDFormat StringBCDFormat { get; set; }
        Encoding StringEncoding { get; set; }
        int StringLength { get; set; }
        object Value { get; set; }
        string VariableAddress { get; set; }
        VariableCahngeEventHandler VariableCollectChange { get; set; }
        VariableCahngeEventHandler VariableCollectIntelnalChange { get; set; }
        VariableCahngeEventHandler VariableValueChange { get; set; }
        CoreDataType CoreDataType { get; set; }
        Type DataType { get; }
        string Description { get; set; }
        string InitialValue { get; set; }
        string Name { get; set; }
        ProtectTypeEnum ProtectType { get; set; }
        object RawValue { get; set; }
        VariableAlarm VariableAlarms { get; set; }
        VariableHis VariableHiss { get; set; }
        VariableProperty VariablePropertys { get; set; }
    }

    public class DeviceVariableCopy : IDeviceVariable
    {
        public long Id { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public long? CreateUserId { get; set; }
        public long? UpdateUserId { get; set; }
        public bool IsDelete { get; set; }
        public IThingsGatewayBitConverter ThingsGatewayBitConverter { get; set; }
        public DateTime ChangeTime { get; set; }
        public DateTime CollectTime { get; set; }
        public Device Device { get; set; }
        public long DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Expressions { get; set; }
        public int Index { get; set; }
        public int InvokeInterval { get; set; }
        public object ObjectTag { get; set; }
        public string OtherMethod { get; set; }
        public int Quality { get; set; }
        public BCDFormat StringBCDFormat { get; set; }
        public Encoding StringEncoding { get; set; }
        public int StringLength { get; set; }
        public object Value { get; set; }
        public string VariableAddress { get; set; }
        public VariableCahngeEventHandler VariableCollectChange { get; set; }
        public VariableCahngeEventHandler VariableCollectIntelnalChange { get; set; }
        public VariableCahngeEventHandler VariableValueChange { get; set; }
        public CoreDataType CoreDataType { get; set; }

        public Type DataType { get; set; }
        public string Description { get; set; }
        public string InitialValue { get; set; }
        public string Name { get; set; }
        public ProtectTypeEnum ProtectType { get; set; }
        public object RawValue { get; set; }
        public VariableAlarm VariableAlarms { get; set; }
        public VariableHis VariableHiss { get; set; }
        public VariableProperty VariablePropertys { get; set; }
    }
}