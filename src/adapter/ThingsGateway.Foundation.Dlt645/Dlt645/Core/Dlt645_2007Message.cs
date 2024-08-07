﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using TouchSocket.Core;

namespace ThingsGateway.Foundation.Dlt645;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal class Dlt645_2007Response : Dlt645_2007Request
{
    /// <summary>
    /// 错误码
    /// </summary>
    public byte? ErrorCode { get; set; }
}

/// <summary>
/// <inheritdoc/>
/// </summary>
internal class Dlt645_2007Message : MessageBase, IResultMessage
{
    private readonly byte[] ReadStation = [0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA];

    private int HeadCodeIndex;

    public Dlt645_2007Send? Dlt645_2007Send { get; set; }

    /// <inheritdoc/>
    public override int HeaderLength { get; set; } = 10;

    public Dlt645_2007Address? Request { get; set; }
    public Dlt645_2007Response Response { get; set; } = new();

    public override FilterResult CheckBody<TByteBlock>(ref TByteBlock byteBlock)
    {
        int sendHeadCodeIndex = Dlt645_2007Send.SendHeadCodeIndex;

        var pos = byteBlock.Position - HeaderLength;
        var endIndex = HeaderLength + BodyLength;
        if (byteBlock[endIndex - 1] == 0x16)
        {
            //检查校验码
            int sumCheck = 0;
            for (int i = HeadCodeIndex; i < endIndex - 2; i++)
                sumCheck += byteBlock[i];
            if ((byte)sumCheck != byteBlock[endIndex - 2])
            {
                //校验错误
                this.ErrorMessage = DltResource.Localizer["SumError"];
                this.OperCode = 999;
                return FilterResult.Success;
            }
            Response.Station = byteBlock.Span.Slice(HeadCodeIndex + 1, 6).ToArray();
            if (!Response.Station.SequenceEqual(Request.Station))//设备地址不符合时，返回错误
            {
                if (!Request.Station.SequenceEqual(ReadStation))//读写通讯地址例外
                {
                    this.ErrorMessage = DltResource.Localizer["StationNotSame"];
                    this.OperCode = 999;
                    return FilterResult.Success;
                }
            }

            var controlCode = byteBlock[HeadCodeIndex + 8];
            if ((controlCode & 0x40) == 0x40)//控制码bit6为1时，返回错误
            {
                Response.ErrorCode = (byte)(byteBlock[HeadCodeIndex + 10] - 0x33);
                var error = Dlt645Helper.Get2007ErrorMessage(Response.ErrorCode.Value);
                this.ErrorMessage = DltResource.Localizer["FunctionError", $"0x{controlCode:X2}", error];
                this.OperCode = 999;
                return FilterResult.Success;
            }
            if (controlCode != ((byte)Dlt645_2007Send.ControlCode) + 0x80)//控制码不符合时，返回错误
            {
                this.ErrorMessage =
                     DltResource.Localizer["FunctionNotSame", $"0x{controlCode:X2}", $"0x{(byte)Dlt645_2007Send.ControlCode:X2}"];
                this.OperCode = 999;
                return FilterResult.Success;
            }
            if (Dlt645_2007Send.ControlCode == ControlCode.Read || Dlt645_2007Send.ControlCode == ControlCode.Write)
            {
                //数据标识不符合时，返回错误
                Response.DataId = byteBlock.Span.Slice(HeadCodeIndex + 10, 4).ToArray().BytesAdd(-0x33);
                if (!Response.DataId.SequenceEqual(Request.DataId))
                {
                    this.ErrorMessage = DltResource.Localizer["DataIdNotSame"];
                    this.OperCode = 999;
                    return FilterResult.Success;
                }
            }

            this.OperCode = 0;
            this.Content = byteBlock.ToArray(HeadCodeIndex + 10, BodyLength - 2);
            return FilterResult.Success;
        }

        return FilterResult.GoOn;
    }

    /// <inheritdoc/>
    public override bool CheckHead<TByteBlock>(ref TByteBlock byteBlock)
    {
        if (Request != null)
        {
            //因为设备可能带有FE前导符开头，这里找到0x68的位置

            if (byteBlock != null)
            {
                for (int index = byteBlock.Position; index < byteBlock.Length; index++)
                {
                    if (byteBlock[index] == 0x68)
                    {
                        HeadCodeIndex = index;
                        break;
                    }
                }
            }

            //帧起始符 地址域  帧起始符 控制码 数据域长度共10个字节
            HeaderLength = HeadCodeIndex - byteBlock.Position + 10;
            if (byteBlock.CanReadLength < HeaderLength + HeadCodeIndex)
            {
                return true;
            }
            BodyLength = byteBlock[HeadCodeIndex + 9] + 2;
            return true;
        }
        else
        {
            return false;//不是主动请求的，可能是心跳DTU包，直接放弃
        }
    }

    public override void SendInfo(ISendMessage sendMessage)
    {
        Dlt645_2007Send = ((Dlt645_2007Send)sendMessage);
        Request = Dlt645_2007Send.Dlt645_2007Address;
    }
}
