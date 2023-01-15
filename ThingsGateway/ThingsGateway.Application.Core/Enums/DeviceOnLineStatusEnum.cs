namespace ThingsGateway.Application.Core
{
    /// <summary>
    /// 设备在线状态
    /// </summary>
    public enum DeviceOnLineStatusEnum
    {
        /// <summary>
        /// 在线
        /// </summary>
        OnLine = 1,
        /// <summary>
        /// 离线
        /// </summary>
        OffLine = 2,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 3,
        /// <summary>
        /// 在线但未完全采集成功
        /// </summary>
        OnLineButNoInitialValue = 4,
        /// <summary>
        /// 初始化
        /// </summary>
        Default = 5,
    }
}
