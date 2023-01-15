//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using System.Text;

namespace ThingsGateway.Foundation.Http.WebSockets
{
    /// <summary>
    /// WSDataFrame辅助扩展类
    /// </summary>
    public static class WSDataFrameExtensions
    {
        /// <summary>
        /// 追加文本
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static WSDataFrame AppendText(this WSDataFrame dataFrame, string text, Encoding encoding = default)
        {
            dataFrame.Opcode = WSDataType.Text;
            byte[] data = (encoding == default ? Encoding.UTF8 : encoding).GetBytes(text);
            if (dataFrame.PayloadData == null)
            {
                dataFrame.PayloadData = new ByteBlock();
            }
            dataFrame.PayloadData.Write(data);
            return dataFrame;
        }

        /// <summary>
        /// 追加二进制流
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static WSDataFrame AppendBinary(this WSDataFrame dataFrame, byte[] buffer, int offset, int length)
        {
            dataFrame.Opcode = WSDataType.Binary;
            if (dataFrame.PayloadData == null)
            {
                dataFrame.PayloadData = new ByteBlock();
            }
            dataFrame.PayloadData.Write(buffer, offset, length);
            return dataFrame;
        }

        /// <summary>
        /// 构建请求数据（含Make）
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="byteBlock"></param>
        /// <returns></returns>
        public static bool BuildRequest(this WSDataFrame dataFrame, ByteBlock byteBlock)
        {
            dataFrame.FIN = true;
            dataFrame.Mask = true;
            if (dataFrame.MaskingKey == null)
            {
                dataFrame.SetMaskString("RRQM");
            }
            return dataFrame.Build(byteBlock, true);
        }

        /// <summary>
        /// 构建请求数据（含Make）
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        public static byte[] BuildRequestToBytes(this WSDataFrame dataFrame)
        {
            dataFrame.FIN = true;
            dataFrame.Mask = true;
            if (dataFrame.MaskingKey == null)
            {
                dataFrame.SetMaskString("RRQM");
            }
            using (ByteBlock byteBlock = new ByteBlock())
            {
                dataFrame.Build(byteBlock, true);
                byte[] data = byteBlock.ToArray();
                return data;
            }
        }

        /// <summary>
        /// 构建响应数据（无Make）
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="byteBlock"></param>
        /// <returns></returns>
        public static bool BuildResponse(this WSDataFrame dataFrame, ByteBlock byteBlock)
        {
            dataFrame.FIN = true;

            return dataFrame.Build(byteBlock, false);
        }

        /// <summary>
        /// 构建响应数据（无Make）
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        public static byte[] BuildResponseToBytes(this WSDataFrame dataFrame)
        {
            dataFrame.FIN = true;

            using (ByteBlock byteBlock = new ByteBlock())
            {
                dataFrame.Build(byteBlock, false);
                byte[] data = byteBlock.ToArray();
                return data;
            }
        }

        /// <summary>
        /// 设置Mask。
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static WSDataFrame SetMaskString(this WSDataFrame dataFrame, string mask)
        {
            byte[] masks = Encoding.UTF8.GetBytes(mask);
            if (masks.Length != 4)
            {
                throw new OverlengthException("Mask只能为ASCII，且只能为四位。");
            }
            dataFrame.MaskingKey = masks;
            return dataFrame;
        }

        /// <summary>
        /// 当<see cref="WSDataType.Text"/>时，转换为Text消息。
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToText(this WSDataFrame dataFrame, Encoding encoding = default)
        {
            return (encoding == default ? Encoding.UTF8 : encoding).GetString(dataFrame.PayloadData.Buffer, 0, dataFrame.PayloadLength);
        }
    }
}