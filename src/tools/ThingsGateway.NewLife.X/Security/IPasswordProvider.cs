﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using NewLife.Collections;

namespace NewLife.Security;

/// <summary>密码提供者</summary>
public interface IPasswordProvider
{
    /// <summary>对密码进行散列处理，此处可以加盐，结果保存在数据库</summary>
    /// <param name="password"></param>
    /// <returns></returns>
    String Hash(String password);

    /// <summary>验证密码散列，包括加盐判断</summary>
    /// <param name="password"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    Boolean Verify(String password, String hash);
}

/// <summary>MD5密码提供者</summary>
public class MD5PasswordProvider : IPasswordProvider
{
    /// <summary>对密码进行散列处理，此处可以加盐，结果保存在数据库</summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public String Hash(String password) => password.MD5();

    /// <summary>验证密码散列，包括加盐判断</summary>
    /// <param name="password"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    public Boolean Verify(String password, String hash) => password.MD5().EqualIgnoreCase(hash);
}

/// <summary>盐值密码提供者</summary>
public class SaltPasswordProvider : IPasswordProvider
{
    private static readonly String _cs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789./";

    /// <summary>算法。支持md5/sha1/sha512</summary>
    public String Algorithm { get; set; } = "sha512";

    /// <summary>使用Unix秒作为盐值。该值为允许的最大时间差，默认0，不使用时间盐值，而是使用随机字符串</summary>
    /// <remarks>一般在传输中使用，避免临时盐值被截取作为它用，建议值30秒。不仅仅是传输耗时，还有两端时间差</remarks>
    public Int32 SaltTime { get; set; }

    /// <summary>对密码进行散列处理，此处可以加盐，结果保存在数据库</summary>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public String Hash(String password)
    {
        var salt = CreateSalt();
        var hash = Algorithm switch
        {
            "md5" => (password.MD5() + salt).MD5(),
            "sha1" => password.GetBytes().SHA1(salt.GetBytes()).ToBase64(),
            "sha512" => password.GetBytes().SHA512(salt.GetBytes()).ToBase64(),
            _ => throw new NotImplementedException(),
        };

        return $"${Algorithm}${salt}${hash}";
    }

    /// <summary>验证密码散列，包括加盐判断</summary>
    /// <param name="password"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    public Boolean Verify(String password, String hash)
    {
        var ss = hash?.Split('$');
        if (ss == null || ss.Length == 0) throw new ArgumentNullException(nameof(hash));

        // 老式MD5，password可能是密码原文，也可能是前端已经md5散列的值
        if (ss.Length == 1) return hash.EqualIgnoreCase(password, password.MD5());

        if (ss.Length != 4) throw new NotSupportedException("Unsupported password hash value");

        var salt = ss[2];
        if (SaltTime > 0)
        {
            // Unix秒作为盐值，时间差不得超过 SaltTime
            var t = DateTime.UtcNow.ToInt() - salt.ToInt();
            if (Math.Abs(t) > SaltTime) return false;
        }

        switch (ss[1])
        {
            case "md5":
                if (ss[3] == (password.MD5() + salt).MD5()) return true;
                return ss[3] == (password + salt).MD5();

            case "sha1":
                return ss[3] == password.GetBytes().SHA1(salt.GetBytes()).ToBase64();

            case "sha512":
                return ss[3] == password.GetBytes().SHA512(salt.GetBytes()).ToBase64();

            default:
                throw new NotSupportedException($"Unsupported password hash mode [{ss[1]}]");
        }
    }

    /// <summary>创建盐值</summary>
    /// <returns></returns>
    protected virtual String CreateSalt()
    {
        if (SaltTime > 0) return DateTime.UtcNow.ToInt().ToString();

        var length = 16;
        var sb = Pool.StringBuilder.Get();
        for (var i = 0; i < length; i++)
        {
            var ch = _cs[Rand.Next(0, _cs.Length)];
            sb.Append(ch);
        }

        return sb.Put(true);
    }
}
