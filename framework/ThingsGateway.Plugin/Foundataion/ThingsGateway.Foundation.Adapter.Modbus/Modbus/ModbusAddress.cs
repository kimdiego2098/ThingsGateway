﻿#region copyright
//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway-docs/
//  QQ群：605534569
//------------------------------------------------------------------------------
#endregion

using System.Text;

namespace ThingsGateway.Foundation.Adapter.Modbus;

/// <summary>
/// Modbus协议地址
/// </summary>
public class ModbusAddress : DeviceAddressBase
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ModbusAddress()
    {

    }
    /// <inheritdoc/>
    public ModbusAddress(string address, ushort len)
    {
        Station = 0;
        AddressStart = 0;
        Parse(address, len);
    }

    /// <inheritdoc/>
    public ModbusAddress(string address, byte station)
    {
        Station = station;
        AddressStart = 0;
        Parse(address, 0);
    }

    /// <summary>
    /// 读取功能码
    /// </summary>
    public byte ReadFunction { get; set; }

    /// <summary>
    /// 站号信息
    /// </summary>
    public byte Station { get; set; }

    /// <summary>
    /// 写入功能码
    /// </summary>
    public byte WriteFunction { get; set; }

    /// <inheritdoc/>
    public override void Parse(string address, int length)
    {
        var result = ParseFrom(address, length);
        if (result.IsSuccess)
        {
            Length = result.Content.Length;
            AddressStart = result.Content.AddressStart;
            ReadFunction = result.Content.ReadFunction;
            Station = result.Content.Station;
            WriteFunction = result.Content.WriteFunction;
        }
    }
    /// <summary>
    /// 解析地址
    /// </summary>
    /// <param name="address"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static OperResult<ModbusAddress> ParseFrom(string address, int length)
    {

        try
        {
            ModbusAddress modbusAddress = new ModbusAddress();

            modbusAddress.Length = length;
            if (address.IndexOf(';') < 0)
            {
                Address(address);

            }
            else
            {
                string[] strArray = address.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < strArray.Length; ++index)
                {
                    if (strArray[index].ToUpper().StartsWith("S="))
                    {
                        if (Convert.ToInt16(strArray[index].Substring(2)) > 0)
                            modbusAddress.Station = byte.Parse(strArray[index].Substring(2));
                    }
                    else if (strArray[index].ToUpper().StartsWith("W="))
                    {
                        if (Convert.ToInt16(strArray[index].Substring(2)) > 0)
                            modbusAddress.WriteFunction = byte.Parse(strArray[index].Substring(2));
                    }
                    else if (!strArray[index].Contains("="))
                    {
                        Address(strArray[index]);
                    }
                }
            }

            return OperResult.CreateSuccessResult(modbusAddress);

            void Address(string address)
            {
                var readF = ushort.Parse(address.Substring(0, 1));
                if (readF > 4)
                    throw new("功能码错误");
                switch (readF)
                {
                    case 0:
                        modbusAddress.ReadFunction = 1;
                        break;
                    case 1:
                        modbusAddress.ReadFunction = 2;
                        break;
                    case 3:
                        modbusAddress.ReadFunction = 4;
                        break;
                    case 4:
                        modbusAddress.ReadFunction = 3;
                        break;
                }
                modbusAddress.AddressStart = int.Parse(address.Substring(1)) - 1;
            }


        }
        catch (Exception ex)
        {
            return new OperResult<ModbusAddress>(ex);
        }
    }



    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder stringGeter = new();
        if (Station > 0)
        {
            stringGeter.Append("s=" + Station.ToString() + ";");
        }
        if (WriteFunction > 0)
        {
            stringGeter.Append("w=" + WriteFunction.ToString() + ";");
        }
        stringGeter.Append(GetFunctionString(ReadFunction) + (AddressStart + 1).ToString());
        return stringGeter.ToString();
    }

    private string GetFunctionString(int readF)
    {
        return readF switch
        {
            1 => "0",
            2 => "1",
            3 => "4",
            4 => "3",
            _ => "4",
        };
    }
}