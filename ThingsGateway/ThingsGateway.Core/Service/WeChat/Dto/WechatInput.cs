namespace ThingsGateway.Core.Service;

public class GenAuthUrlInput
{
    /// <summary>
    /// RedirectUrl
    /// </summary>
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Scope
    /// </summary>
    public string Scope { get; set; }
}

public class WechatOAuth2Input
{
    /// <summary>
    /// Code
    /// </summary>
    [Required(ErrorMessage = "Code串不能为空"), MinLength(10, ErrorMessage = "Code串长度不对")]
    public string Code { get; set; }
}

public class WechatUserLogin
{
    /// <summary>
    /// OpenId
    /// </summary>
    [Required(ErrorMessage = "微信标识不能为空"), MinLength(10, ErrorMessage = "微信标识长度不对")]
    public string OpenId { get; set; }
}

public class SignatureInput
{
    /// <summary>
    /// Url
    /// </summary>
    public string Url { get; set; }
}