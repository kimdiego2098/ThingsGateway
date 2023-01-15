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
using System.Collections.Generic;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// Rpc传输类
    /// </summary>
    public sealed class TouchRpcPackage : WaitRouterPackage
    {
        /// <summary>
        /// 反馈类型
        /// </summary>
        public FeedbackType Feedback { get; private set; }

        /// <summary>
        /// 参数是否包含引用类型
        /// </summary>
        public bool IsByRef { get; internal set; }

        /// <summary>
        /// 函数名
        /// </summary>
        public string MethodName { get; internal set; }

        /// <summary>
        /// 参数数据
        /// </summary>
        public List<byte[]> ParametersBytes { get; internal set; }

        /// <summary>
        /// 返回参数数据
        /// </summary>
        public byte[] ReturnParameterBytes { get; internal set; }

        /// <summary>
        /// 序列化类型
        /// </summary>
        public SerializationType SerializationType { get; private set; }

        /// <inheritdoc/>
        public override void PackageBody(ByteBlock byteBlock)
        {
            base.PackageBody(byteBlock);
            byteBlock.Write((byte)SerializationType);
            byteBlock.Write((byte)Feedback);
            byteBlock.Write(IsByRef);
            byteBlock.Write(MethodName);
            byteBlock.WriteBytesPackage(ReturnParameterBytes);

            if (ParametersBytes != null && ParametersBytes.Count > 0)
            {
                byteBlock.Write((byte)ParametersBytes.Count);
                foreach (byte[] item in ParametersBytes)
                {
                    byteBlock.WriteBytesPackage(item);
                }
            }
            else
            {
                byteBlock.Write((byte)0);
            }
        }

        /// <inheritdoc/>
        public override void UnpackageBody(ByteBlock byteBlock)
        {
            base.UnpackageBody(byteBlock);
            SerializationType = (SerializationType)byteBlock.ReadByte();
            Feedback = (FeedbackType)byteBlock.ReadByte();
            IsByRef = byteBlock.ReadBoolean();
            MethodName = byteBlock.ReadString();
            ReturnParameterBytes = byteBlock.ReadBytesPackage();

            byte countPar = (byte)byteBlock.ReadByte();
            ParametersBytes = new List<byte[]>();
            for (int i = 0; i < countPar; i++)
            {
                ParametersBytes.Add(byteBlock.ReadBytesPackage());
            }
        }

        internal void LoadInvokeOption(IInvokeOption option)
        {
            InvokeOption invokeOption = (InvokeOption)option;
            Feedback = invokeOption.FeedbackType;
            SerializationType = invokeOption.SerializationType;
        }

        internal void ThrowStatus()
        {
            if (Status == 1)
            {
                return;
            }
            switch (Status.ToStatus())
            {
                case TouchSocketStatus.ClientNotFind:
                    {
                        throw new ClientNotFindException(TouchSocketStatus.ClientNotFind.GetDescription(TargetId));
                    }
                case TouchSocketStatus.UnknownError:
                case TouchSocketStatus.RpcMethodNotFind:
                case TouchSocketStatus.RpcMethodDisable:
                case TouchSocketStatus.RemoteRefuse:
                case TouchSocketStatus.RpcInvokeException:
                case TouchSocketStatus.RoutingNotAllowed:
                case TouchSocketStatus.Exception:
                default:
                    {
                        throw new RpcException(Status.ToStatus().GetDescription(Message));
                    }
            }
        }
    }
}