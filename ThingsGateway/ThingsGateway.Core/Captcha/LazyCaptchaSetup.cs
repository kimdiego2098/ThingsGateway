using Lazy.Captcha.Core;
using Lazy.Captcha.Core.Generator;
using Lazy.Captcha.Core.Storage;

using SkiaSharp;

namespace ThingsGateway.Core;

public static class LazyCaptchaSetup
{
    /// <summary>
    /// 验证码初始化
    /// </summary>
    /// <param name="services"></param>
    public static void AddLazyCaptcha(this IServiceCollection services)
    {
        services.AddCaptcha(App.Configuration);
        services.AddScoped<ICaptcha, RandomCaptcha>();
    }
}

/// <summary>
/// 随机验证码
/// </summary>
public class RandomCaptcha : DefaultCaptcha
{
    private static readonly Random random = new();
    private static readonly CaptchaType[] captchaTypes = Enum.GetValues<CaptchaType>();

    public RandomCaptcha(IOptionsSnapshot<CaptchaOptions> options, IStorage storage) : base(options, storage)
    {
    }

    /// <summary>
    /// 更新选项
    /// </summary>
    /// <param name="options"></param>
    protected override void ChangeOptions(CaptchaOptions options)
    {
        // 随机验证码类型
        options.CaptchaType = captchaTypes[10]; // captchaTypes[random.Next(0, captchaTypes.Length)];

        // 当是算数运算时，CodeLength是指运算数个数
        if (options.CaptchaType.IsArithmetic())
        {
            options.CodeLength = 2;
        }
        else
        {
            options.CodeLength = 4;
        }

        // 如果包含中文时，使用kaiti字体，否则文字乱码
        if (options.CaptchaType.ContainsChinese())
        {
            options.ImageOption.FontFamily = DefaultFontFamilys.Instance.Kaiti;
            options.ImageOption.FontSize = 24;
        }
        else
        {
            options.ImageOption.FontFamily = DefaultFontFamilys.Instance.Actionj;
        }

        options.IgnoreCase = true; // 忽略大小写

        options.ImageOption.Animation = random.Next(2) == 0; // 动静

        options.ImageOption.InterferenceLineCount = random.Next(1, 5); // 干扰线数量

        options.ImageOption.BubbleCount = random.Next(1, 5); // 气泡数量
        //options.ImageOption.BubbleMinRadius = 5; // 气泡最小半径
        //options.ImageOption.BubbleMaxRadius = 15; // 气泡最大半径
        //options.ImageOption.BubbleThickness = 1; // 气泡边沿厚度

        options.ImageOption.BackgroundColor = SKColors.White; // 背景色

        options.ImageOption.Width = 150; // 验证码宽度
        options.ImageOption.Height = 50; // 验证码高度

        options.ImageOption.FontSize = 36; // 字体大小
        //options.ImageOption.FontFamily = DefaultFontFamilys.Instance.Actionj; // 字体
    }
}