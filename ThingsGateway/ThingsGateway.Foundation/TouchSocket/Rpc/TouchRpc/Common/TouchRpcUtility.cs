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

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// TouchRpcUtility
    /// </summary>
    public partial class TouchRpcUtility
    {
        /// <summary>
        /// TouchRpc字符串
        /// </summary>
        public const string TouchRpc = "TOUCHRPC";

        /// <summary>
        /// TouchRpc
        /// </summary>
        public static Protocol TouchRpcProtocol { get; private set; } = new Protocol(TouchRpc);

        /// <summary>
        /// 传输分包
        /// </summary>
        public const int TransferPackage = 1024 * 512;

        #region 基本协议，0-99；

        /// <summary>
        /// 握手
        /// </summary>
        public const short P_0_Handshake_Request = 0;

        /// <summary>
        /// 重置ID
        /// </summary>
        public const short P_1_ResetID_Request = -1;

        /// <summary>
        /// 握手响应
        /// </summary>
        public const short P_1000_Handshake_Response = -1000;

        /// <summary>
        /// 重置ID响应
        /// </summary>
        public const short P_1001_ResetID_Response = -1001;

        /// <summary>
        /// Ping回应
        /// </summary>
        public const short P_1002_Ping_Response = -1002;

        /// <summary>
        /// Ping
        /// </summary>
        public const short P_2_Ping_Request = -2;

        #endregion 基本协议，0-99；

        #region 通道协议，100-199；

        /// <summary>
        /// 创建一个面向对方的通道
        /// </summary>
        public const short P_100_CreateChannel_Request = -100;

        /// <summary>
        /// 创建通道回应
        /// </summary>
        public const short P_1100_CreateChannel_Response = -1100;

        /// <summary>
        /// 通道数据
        /// </summary>
        public const short P_101_ChannelPackage = -101;

        #endregion 通道协议，100-199；

        #region Rpc协议，200-299；

        /// <summary>
        /// 调用响应
        /// </summary>
        public const short P_1200_Invoke_Response = -1200;

        ///// <summary>
        ///// 调用ID客户端响应
        ///// </summary>
        //public const short P_1201_Invoke2C_Response = -1201;

        /// <summary>
        /// 调用
        /// </summary>
        public const short P_200_Invoke_Request = -200;

        ///// <summary>
        ///// 调用ID客户端
        ///// </summary>
        //public const short P_201_Invoke2C_Request = -201;

        /// <summary>
        /// 取消调用
        /// </summary>
        public const short P_204_CancelInvoke = -204;

        ///// <summary>
        ///// 取消调用
        ///// </summary>
        //public const short P_205_CancelInvoke2C = -205;

        #endregion Rpc协议，200-299；

        #region Stream协议 400-499

        /// <summary>
        /// 向服务器发送流响应
        /// </summary>
        public const short P_1400_SendStreamToSocketClient_Response = -1400;

        /// <summary>
        /// 向服务器发送流
        /// </summary>
        public const short P_400_SendStreamToSocketClient_Request = -400;

        /// <summary>
        /// 向客户端发送流
        /// </summary>
        public const short P_401_SendStreamToClient = -401;

        #endregion Stream协议 400-499

        #region FileTransfer协议 500-599

        /// <summary>
        /// 拉取文件响应
        /// </summary>
        public const short P_1500_PullFile_Response = -1500;

        /// <summary>
        /// 开始拉取文件响应
        /// </summary>
        public const short P_1501_BeginPullFile_Response = -1501;

        /// <summary>
        /// 推送文件响应
        /// </summary>
        public const short P_1502_PushFile_Response = -1502;

        ///// <summary>
        ///// 从客户端拉取文件响应
        ///// </summary>
        //public const short P_1503_PullFile2C_Response = -1503;

        ///// <summary>
        ///// 客户端拉取文件转发响应
        ///// </summary>
        //public const short P_1504_PullFileFC_Response = -1504;

        ///// <summary>
        ///// 开始从客户端拉取文件响应
        ///// </summary>
        //public const short P_1505_BeginPullFile2C_Response = -1505;

        ///// <summary>
        ///// 开始从客户端拉取文件转发响应
        ///// </summary>
        //public const short P_1506_BeginPullFileFC_Response = -1506;

        ///// <summary>
        ///// 推送到客户端响应
        ///// </summary>
        //public const short P_1507_PushFile2C_Response = -1507;

        ///// <summary>
        ///// 推送到客户端转发响应
        ///// </summary>
        //public const short P_1508_PushFileFC_Response = -1508;

        /// <summary>
        /// 拉取文件
        /// </summary>
        public const short P_500_PullFile_Request = -500;

        /// <summary>
        /// 开始拉取文件
        /// </summary>
        public const short P_501_BeginPullFile_Request = -501;

        /// <summary>
        /// 推送文件
        /// </summary>
        public const short P_502_PushFile_Request = -502;

        ///// <summary>
        ///// 从客户端拉取文件
        ///// </summary>
        //public const short P_503_PullFile2C_Request = -503;

        ///// <summary>
        ///// 客户端拉取文件转发
        ///// </summary>
        //public const short P_504_PullFileFC_Request = -504;

        ///// <summary>
        ///// 开始从客户端拉取文件
        ///// </summary>
        //public const short P_505_BeginPullFile2C_Request = -505;

        ///// <summary>
        ///// 开始从客户端拉取文件转发
        ///// </summary>
        //public const short P_506_BeginPullFileFC_Request = -506;

        ///// <summary>
        ///// 推送到客户端
        ///// </summary>
        //public const short P_507_PushFile2C_Request = -507;

        ///// <summary>
        ///// 推送到客户端转发
        ///// </summary>
        //public const short P_508_PushFileFC_Request = -508;

        /// <summary>
        /// 推送文件状态确认
        /// </summary>
        public const short P_509_PushFileAck_Request = -509;

        ///// <summary>
        ///// 推送文件到客户端状态确认
        ///// </summary>
        //public const short P_511_PushFileAck2C_Request = -511;

        /// <summary>
        /// 拉取小文件请求
        /// </summary>
        public const short P_517_PullSmallFile_Request = -517;

        /// <summary>
        /// 拉取确认小文件响应
        /// </summary>
        public const short P_1517_PullSmallFile_Response = -1517;

        /// <summary>
        /// 推送小文件请求
        /// </summary>
        public const short P_518_PushSmallFile_Request = -518;

        /// <summary>
        /// 推送确认小文件响应
        /// </summary>
        public const short P_1518_PushSmallFile_Response = -1518;

        #endregion FileTransfer协议 500-599

        #region Redis 600-699

        /// <summary>
        /// Redis
        /// </summary>
        public const short P_600_Redis_Request = -600;

        /// <summary>
        /// Redis回应
        /// </summary>
        public const short P_1600_Redis_Response = -1600;

        #endregion Redis 600-699
    }
}