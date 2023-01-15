//驱动通讯返回标准化，参照RESULTAPI
namespace ThingsGateway.Foundation
{
    public class OperResult<T> : OperResult
    {

        #region Public Constructors

        public OperResult()
        {

        }
        public OperResult(T value, string msg = "")
        {
            Content = value;
            Message = msg;
        }

        public OperResult(ResultCode resultCode, string msg = "") : base(resultCode, msg) { }

        public OperResult(string msg) : base(msg) { }
        public OperResult(Exception ex) : base(ex.Message + Environment.NewLine + ex.StackTrace) { }
        #endregion Public Constructors

        #region Public Properties

        public T Content { get; set; }

        #endregion Public Properties
        public OperResult<TResult> Then<TResult>(Func<T, OperResult<TResult>> func)
        {
            return !IsSuccess ? OperResult.CreateFailedResult<TResult>(this) : func(Content);
        }
        public override OperResult<T1> ChangedType<T1>()
        {
            return new OperResult<T1>()
            {
                ResultCode = ResultCode,
                Message = Message,
                Content = (T1)Convert.ChangeType(Content, typeof(T1)),
            };
        }

    }
    public class OperResult<T1, T2> : OperResult
    {
        public OperResult()
        {

        }
        public OperResult(T1 t1, T2 t2)
        {
            Content1 = t1;
            Content2 = t2;
        }
        public OperResult(ResultCode resultCode, string msg = "") : base(resultCode, msg) { }

        public OperResult(string msg) : base(msg) { }
        #region Public Properties

        public T1 Content1 { get; set; }
        public T2 Content2 { get; set; }

        #endregion Public Properties
        public override OperResult<T> ChangedType<T>()
        {
            return new OperResult<T>()
            {
                ResultCode = ResultCode,
                Message = Message,
                Content = (T)Convert.ChangeType(Content1, typeof(T)),
            };
        }
    }

    public class OperResult<T1, T2, T3> : OperResult
    {
        public OperResult()
        {

        }
        public OperResult(T1 t1, T2 t2, T3 t3)
        {
            Content1 = t1;
            Content2 = t2;
            Content3 = t3;
        }
        public OperResult(ResultCode resultCode, string msg = "") : base(resultCode, msg) { }

        public OperResult(string msg) : base(msg) { }
        #region Public Properties

        public T1 Content1 { get; set; }
        public T2 Content2 { get; set; }
        public T3 Content3 { get; set; }

        #endregion Public Properties
        public override OperResult<T> ChangedType<T>()
        {
            return new OperResult<T>()
            {
                ResultCode = ResultCode,
                Message = Message,
                Content = (T)Convert.ChangeType(Content1, typeof(T)),
            };
        }
    }

    public class OperResult : ResultBase
    {

        #region Public Constructors

        public OperResult()
        {
        }

        public OperResult(ResultCode resultCode, string msg = "")
        {
            ResultCode = resultCode;
            Message = msg;
        }
        public int Code;
        public OperResult(int code, string msg)
        {
            Code = code;
            Message = msg;
        }
        public OperResult(string msg)
        {
            Message = msg;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool IsSuccess => ResultCode.HasFlag(ResultCode.Success);

        #endregion Public Properties
        public virtual OperResult<T> ChangedType<T>()
        {
            return this.Copy<T>();
        }
        #region Public Methods

        public static OperResult CreateSuccessResult()
        {
            return new OperResult()
            {
                ResultCode = ResultCode.Success,
                Message = TouchSocketStatus.Success.GetDescription(),
            };
        }
        public static OperResult<T> CreateSuccessResult<T>(T value)
        {
            return new OperResult<T>()
            {
                ResultCode = ResultCode.Success,
                Message = TouchSocketStatus.Success.GetDescription(),
                Content = value
            };
        }
        public static OperResult<T1, T2> CreateSuccessResult<T1, T2>(T1 value1, T2 value2)
        {
            return new OperResult<T1, T2>()
            {
                ResultCode = ResultCode.Success,
                Message = TouchSocketStatus.Success.GetDescription(),
                Content1 = value1,
                Content2 = value2
            };
        }

        public static OperResult<T1, T2, T3> CreateSuccessResult<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            return new OperResult<T1, T2, T3>()
            {
                ResultCode = ResultCode.Success,
                Message = TouchSocketStatus.Success.GetDescription(),
                Content1 = value1,
                Content2 = value2,
                Content3 = value3
            };
        }

        #endregion Public Methods
        public static OperResult<T1> CreateFailedResult<T1>(OperResult result)
        {
            OperResult<T1> failedResult = new OperResult<T1>
            {
                ResultCode = result.ResultCode,
                Message = result.Message,
                Code = result.Code
            };
            return failedResult;
        }

        public static OperResult<T1, T2> CreateFailedResult<T1, T2>(OperResult result)
        {
            OperResult<T1, T2> failedResult = new OperResult<T1, T2>
            {
                ResultCode = result.ResultCode,
                Message = result.Message,
                Code = result.Code
            };
            return failedResult;
        }
        public static OperResult<T1, T2, T3> CreateFailedResult<T1, T2, T3>(OperResult result)
        {
            OperResult<T1, T2, T3> failedResult = new OperResult<T1, T2, T3>
            {
                ResultCode = result.ResultCode,
                Message = result.Message,
                Code = result.Code
            };
            return failedResult;
        }
    }
}