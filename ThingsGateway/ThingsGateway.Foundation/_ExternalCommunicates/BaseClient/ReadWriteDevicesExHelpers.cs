using System.Threading.Tasks;

namespace ThingsGateway.Foundation
{
    public static class ReadWriteDevicesExHelpers
    {
        public static bool GetBoolValue(this string value)
        {
            if (value == "1")
                return true;
            if (value == "0")
                return false;
            value = value.ToUpper();
            if (value == "TRUE")
                return true;
            if (value == "FALSE")
                return false;
            if (value == "ON")
                return true;
            return !(value == "OFF") && bool.Parse(value);
        }
        public static Task<OperResult> WriteAsync(this IReadWriteDevice readWriteDevice, Type type, string address, string value)
        {
            if (type == typeof(bool))
                return readWriteDevice.WriteAsync(address, GetBoolValue(value));
            else if (type == typeof(byte))
                return readWriteDevice.WriteAsync(address, Convert.ToByte(value));
            else if (type == typeof(sbyte))
                return readWriteDevice.WriteAsync(address, Convert.ToSByte(value));
            else if (type == typeof(short))
                return readWriteDevice.WriteAsync(address, Convert.ToInt16(value));
            else if (type == typeof(ushort))
                return readWriteDevice.WriteAsync(address, Convert.ToUInt16(value));
            else if (type == typeof(int))
                return readWriteDevice.WriteAsync(address, Convert.ToInt32(value));
            else if (type == typeof(uint))
                return readWriteDevice.WriteAsync(address, Convert.ToUInt32(value));
            else if (type == typeof(long))
                return readWriteDevice.WriteAsync(address, Convert.ToInt64(value));
            else if (type == typeof(ulong))
                return readWriteDevice.WriteAsync(address, Convert.ToUInt64(value));
            else if (type == typeof(float))
                return readWriteDevice.WriteAsync(address, Convert.ToSingle(value));
            else if (type == typeof(double))
                return readWriteDevice.WriteAsync(address, Convert.ToDouble(value));
            return Task.FromResult(new OperResult());
        }
    }
}