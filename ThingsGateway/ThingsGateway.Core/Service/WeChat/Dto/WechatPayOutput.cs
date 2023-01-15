namespace ThingsGateway.Core.Service;

public class WechatPayOutput
{
    /// <summary>
    /// OpenId
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// 订单金额
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    public string Attachment { get; set; }

    /// <summary>
    /// 优惠标记
    /// </summary>
    public string GoodsTag { get; set; }
}