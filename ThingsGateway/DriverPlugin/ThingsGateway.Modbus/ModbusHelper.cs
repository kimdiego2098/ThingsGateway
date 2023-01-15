using Microsoft.Extensions.Logging;

using System.Text;

using ThingsGateway.Application.Core;
using ThingsGateway.Foundation;
using ThingsGateway.Foundation.Adapter.Modbus;
using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Modbus
{
    internal static class ModbusHelper
    {
        internal static OperResult<List<DeviceVariableSourceRead>> LoadSourceRead(this List<DeviceVariable> deviceVariables, ILogger _logger, IThingsGatewayBitConverter byteConverter, int MaxPack)
        {
            var result = new List<DeviceVariableSourceRead>();
            try
            {
                //需要先剔除额外信息，比如dataformat等
                foreach (var item in deviceVariables)
                {
                    var address = item.VariableAddress;

                    IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(
                     ref address, byteConverter, out Encoding encoding, out ushort length, out BCDFormat bCDFormat);
                    item.ThingsGatewayBitConverter = transformParameter;
                    item.StringEncoding = encoding ?? Encoding.ASCII;
                    item.StringLength = length;
                    item.StringBCDFormat = bCDFormat;
                    item.VariableAddress = address;

                    int bitIndex = 0;
                    string[] addressSplits = new string[] { address };
                    if (address.IndexOf('.') > 0)
                    {
                        addressSplits = address.SplitDot();
                        try
                        {
                            bitIndex = Convert.ToInt32(addressSplits.Last());

                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError("自动分包方法获取Bit失败:{0}", ex);
                        }
                    }
                    item.Index = bitIndex;
                }

                //按读取间隔分组
                var tags = deviceVariables.GroupBy(it => it.InvokeInterval);
                foreach (var item in tags)
                {
                    Dictionary<ModbusAddress, DeviceVariable> map = item.ToDictionary(it =>
                    {
                        var lastLen = DataTypeItem.DictTypes[it.CoreDataType].Length;
                        if (lastLen == -1)
                        {
                            if (DataTypeItem.DictTypes[it.CoreDataType].Type == typeof(bool))
                            {
                                lastLen = 2;
                            }
                            else if (DataTypeItem.DictTypes[it.CoreDataType].Type == typeof(string))
                            {
                                lastLen = it.StringLength;
                            }
                        }
                        var address = it.VariableAddress;
                        if (address.IndexOf('.') > 0)
                        {
                            var addressSplits = address.SplitDot();

                            address = addressSplits.RemoveLast(1).ArrayToString(".");
                        }

                        var result = new ModbusAddress(address, (ushort)lastLen);
                        if (result == null)
                        {
                        }

                        return result;
                    });

                    //获取变量的地址
                    var modbusAddressList = map.Keys.ToList();

                    //获取功能码
                    var functionCodes = modbusAddressList.Select(t => t.ReadFunction).Distinct();
                    foreach (var functionCode in functionCodes)
                    {
                        var modbusAddressSameFunList = modbusAddressList
                            .Where(t => t.ReadFunction == functionCode);
                        var stationNumbers = modbusAddressSameFunList
                            .Select(t => t.Station).Distinct();
                        foreach (var stationNumber in stationNumbers)
                        {
                            var addressList = modbusAddressSameFunList.Where(t => t.Station == stationNumber)
                                .ToDictionary(t => t, t => map[t]);
                            var tempResult = LoadSourceRead(addressList, functionCode, item.Key, MaxPack);
                            result.AddRange(tempResult.Content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError("自动分包失败:{0}", ex);
            }
            return OperResult.CreateSuccessResult(result);
        }

        private static OperResult<List<DeviceVariableSourceRead>> LoadSourceRead(Dictionary<ModbusAddress, DeviceVariable> addressList, int functionCode, int timeInterval, int MaxPack)
        {
            List<DeviceVariableSourceRead> sourceReads = new List<DeviceVariableSourceRead>();
            var addresss = addressList.Keys.OrderBy(it =>
            {
                int address = 0;
                address = it.AddressStart;
                return address + (it.Length / 2);
            }).ToList();
            var minAddress = addresss.First().AddressStart;
            var maxAddress = addresss.Last().AddressStart;
            while (maxAddress >= minAddress)
            {
                int readLength = MaxPack;
                if (functionCode == 1 || functionCode == 2)
                {
                    readLength = MaxPack * 8 * 2;
                }

                var tempAddress = addresss.Where(t => t.AddressStart >= minAddress && (t.AddressStart + (t.Length / 2)) <= minAddress + readLength).ToList();

                while (tempAddress.Last().AddressStart + (tempAddress.Last().Length / 2) - tempAddress.First().AddressStart > readLength)
                {
                    tempAddress.Remove(tempAddress.Last());
                }
                //读取寄存器长度
                var sourceLen = tempAddress.Last().AddressStart + (tempAddress.Last().Length / 2) - tempAddress.First().AddressStart;
                DeviceVariableSourceRead sourceRead = new DeviceVariableSourceRead(timeInterval);
                sourceRead.Address = tempAddress.OrderBy(it => it.AddressStart).First().ToString();
                sourceRead.Length = sourceLen.ToString();
                foreach (var item in tempAddress)
                {
                    var readNode = addressList[item];
                    if ((functionCode == -1 || functionCode == 3 || functionCode == 4) &&
                        readNode.CoreDataType == CoreDataType.Bool)
                    {
                        readNode.Index = ((item.AddressStart - tempAddress.First().AddressStart) * 16) + readNode.Index;
                    }
                    else
                    {
                        readNode.Index = ((item.AddressStart - tempAddress.First().AddressStart) * 2) + readNode.Index;
                    }
                    if (functionCode == 1 || functionCode == 2)
                        readNode.Index = item.AddressStart - tempAddress.First().AddressStart + readNode.Index;

                    sourceRead.DeviceVariables.Add(readNode);
                    addresss.Remove(item);
                }
                sourceReads.Add(sourceRead);
                if (addresss.Count > 0)
                    minAddress = addresss.First().AddressStart;
                else
                    break;
            }
            return OperResult.CreateSuccessResult(sourceReads);
        }

    }
}
