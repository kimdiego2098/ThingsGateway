namespace ThingsGateway.Foundation
{
    /// <summary>
    /// 返回通知接口，继承IRequestInfo
    /// </summary>
    public interface IResult : IRequestInfo
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        ResultCode ResultCode { get; }
        /// <summary>
        /// 消息
        /// </summary>
        string Message { get; }
    }
}