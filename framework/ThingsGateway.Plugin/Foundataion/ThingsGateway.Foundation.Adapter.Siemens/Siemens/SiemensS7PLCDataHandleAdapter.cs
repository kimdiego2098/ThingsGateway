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


namespace ThingsGateway.Foundation.Adapter.Siemens;

/// <summary>
/// SiemensS7PLCDataHandleAdapter
/// </summary>
public class SiemensS7PLCDataHandleAdapter : ReadWriteDevicesTcpDataHandleAdapter<SiemensMessage>
{
    /// <inheritdoc/>
    public override byte[] PackCommand(byte[] command)
    {
        return command;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override SiemensMessage GetInstance()
    {
        return new SiemensMessage();
    }

    /// <inheritdoc/>
    protected override FilterResult UnpackResponse(SiemensMessage request, byte[] send, byte[] body, byte[] response)
    {
        var result = new OperResult<byte[]>();
        if (response[2] * 256 + response[3] == 7)
        {
            result = new OperResult<byte[]>(response);
        }
        else
        {
            //以请求方为准，分开返回类型校验
            switch (send[17])
            {
                case 0x04:
                    result = SiemensHelper.AnalysisReadByte(send, response);
                    break;
                case 0x05:
                    result = SiemensHelper.AnalysisWrite(response);
                    break;
            }
        }
        if (result.IsSuccess)
        {
            request.ResultCode = result.ResultCode;
            request.Message = result.Message;
            request.Content = result.Content;
            return FilterResult.Success;
        }
        else
        {
            //如果返回错误，具体分析
            if (response.Length < 21)
            {
                request.ResultCode = result.ResultCode;
                request.Message = result.Message;
                request.Content = result.Content;
                //如果长度不足，返回缓存
                return FilterResult.Cache;
            }
            else
            {
                request.ResultCode = result.ResultCode;
                request.Message = result.Message;
                request.Content = result.Content;
                //如果长度已经超了，说明这段报文已经不能继续解析了，直接返回放弃
                return FilterResult.Success;
            }
        }
    }
}