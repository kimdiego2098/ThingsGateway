﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

namespace ThingsGateway.Foundation.Modbus;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal class ModbusTcpMessage : MessageBase, IResultMessage
{
    /// <inheritdoc/>
    public override int HeaderLength => 9;

    public ModbusResponse Response { get; set; } = new();

    public override bool CheckHead<TByteBlock>(ref TByteBlock byteBlock)
    {
        this.Sign = byteBlock.ReadUInt16(EndianType.Big);
        byteBlock.Position += 2;
        this.BodyLength = byteBlock.ReadUInt16(EndianType.Big) - 3;
        Response.Station = byteBlock.ReadByte();
        bool error = false;
        var code = byteBlock.ReadByte();
        if ((code & 0x80) == 0)
        {
            Response.FunctionCode = code;
        }
        else
        {
            code = code.SetBit(7, false);
            Response.FunctionCode = code;
            error = true;
        }

        if (error)
        {
            Response.ErrorCode = byteBlock.ReadByte();
            this.OperCode = Response.ErrorCode;
            this.ErrorMessage = ModbusHelper.GetDescriptionByErrorCode(Response.ErrorCode.Value);
            return true;
        }
        else
        {
            if (Response.FunctionCode <= 4)
            {
                Response.Length = byteBlock.ReadByte();
            }
            else
            {
                Response.StartAddress = byteBlock.ReadUInt16();
            }
            return true;
        }
    }

    public override FilterResult CheckBody<TByteBlock>(ref TByteBlock byteBlock)
    {
        if (Response.ErrorCode.HasValue)
        {
            return FilterResult.Success;
        }

        if (Response.FunctionCode <= 4)
        {
            this.OperCode = 0;
            this.Content = byteBlock.ToArrayTake(BodyLength);
            Response.Data = this.Content;
            return FilterResult.Success;
        }
        else if (Response.FunctionCode == 5 || Response.FunctionCode == 6)
        {
            byteBlock.Position = HeaderLength - 1;
            Response.StartAddress = byteBlock.ReadUInt16();
            this.OperCode = 0;
            this.Content = byteBlock.ToArrayTake(BodyLength - 1);
            Response.Data = this.Content;
            return FilterResult.Success;
        }
        else if (Response.FunctionCode == 15 || Response.FunctionCode == 16)
        {
            byteBlock.Position = HeaderLength - 1;
            Response.StartAddress = byteBlock.ReadUInt16(EndianType.Big);
            Response.Length = byteBlock.ReadUInt16(EndianType.Big);
            this.OperCode = 0;
            this.Content = Array.Empty<byte>();
            return FilterResult.Success;
        }
        return FilterResult.GoOn;
    }
}
