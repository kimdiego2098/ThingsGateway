namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 一个成员访问接口
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// 获取指定成员的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        object GetValue(object instance, string memberName);

        /// <summary>
        ///设置指定成员的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        void SetValue(object instance, string memberName, object newValue);
    }
}
