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



namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 结果返回
    /// </summary>
    public struct Result : IResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly Result Success = new Result(ResultCode.Success, "Success");

        /// <summary>
        /// 初始状态
        /// </summary>
        public static readonly Result Default = new Result(ResultCode.Default, "Default");

        /// <summary>
        /// 未知失败
        /// </summary>
        public static readonly Result UnknownFail = new Result(ResultCode.Fail, TouchSocketStatus.UnknownError.GetDescription());

        /// <summary>
        /// 超时
        /// </summary>
        public static readonly Result Overtime = new Result(ResultCode.Overtime, TouchSocketStatus.Overtime.GetDescription());

        /// <summary>
        /// 取消
        /// </summary>
        public static readonly Result Canceled = new Result(ResultCode.Canceled, TouchSocketStatus.Canceled.GetDescription());

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="message"></param>
        public Result(ResultCode resultCode, string message)
        {
            ResultCode = resultCode;
            Message = message;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="result"></param>
        public Result(IResult result)
        {
            ResultCode = result.ResultCode;
            Message = result.Message;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exception"></param>
        public Result(Exception exception)
        {
            ResultCode = ResultCode.Exception;
            Message = exception.Message;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resultCode"></param>
        public Result(ResultCode resultCode)
        {
            ResultCode = resultCode;
            Message = resultCode.GetDescription();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ResultCode ResultCode { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 创建来自<see cref="ResultCode.Canceled"/>的<see cref="Result"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result FromCanceled(string msg)
        {
            return new Result(ResultCode.Canceled, msg);
        }

        /// <summary>
        /// 创建来自<see cref="ResultCode.Error"/>的<see cref="Result"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result FromError(string msg)
        {
            return new Result(ResultCode.Error, msg);
        }

        /// <summary>
        /// 创建来自<see cref="ResultCode.Exception"/>的<see cref="Result"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result FromException(string msg)
        {
            return new Result(ResultCode.Exception, msg);
        }

        /// <summary>
        /// 创建来自<see cref="ResultCode.Overtime"/>的<see cref="Result"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result FromFail(string msg)
        {
            return new Result(ResultCode.Fail, msg);
        }

        /// <summary>
        /// 创建来自<see cref="ResultCode.Overtime"/>的<see cref="Result"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result FromOvertime(string msg)
        {
            return new Result(ResultCode.Overtime, msg);
        }

        /// <summary>
        /// 创建来自<see cref="ResultCode.Success"/>的<see cref="Result"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result FromSuccess(string msg)
        {
            return new Result(ResultCode.Success, msg);
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"类型：{ResultCode}，信息：{Message}";
        }
    }

    /// <summary>
    /// 结果返回
    /// </summary>
    public class ResultBase : IResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="message"></param>
        public ResultBase(ResultCode resultCode, string message)
        {
            ResultCode = resultCode;
            Message = message;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resultCode"></param>
        public ResultBase(ResultCode resultCode)
        {
            ResultCode = resultCode;
            Message = resultCode.GetDescription();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="result"></param>
        public ResultBase(Result result)
        {
            ResultCode = result.ResultCode;
            Message = result.Message;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResultBase()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ResultCode ResultCode { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"类型：{ResultCode}，信息：{Message}";
        }
    }

    /// <summary>
    /// ResultExtensions
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// 是否成功。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsSuccess(this IResult result)
        {
            return result.ResultCode == ResultCode.Success;
        }

        /// <summary>
        /// 是否没有成功。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool NotSuccess(this IResult result)
        {
            return result.ResultCode != ResultCode.Success;
        }

        /// <summary>
        /// 转换为<see cref="Result"/>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Result ToResult(this IResult result)
        {
            return new Result(result);
        }
    }
}