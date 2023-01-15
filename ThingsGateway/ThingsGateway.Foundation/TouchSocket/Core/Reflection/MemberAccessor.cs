using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 动态成员访问器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemberAccessor<T> : MemberAccessor
    {
        /// <summary>
        /// 动态成员访问器
        /// </summary>
        public MemberAccessor() : base(typeof(T))
        {

        }
    }

    /// <summary>
    /// 动态成员访问器
    /// </summary>
    public class MemberAccessor : IMemberAccessor
    {
        Func<object, string, object> GetValueDelegate;

        Action<object, string, object> SetValueDelegate;

        /// <summary>
        /// 动态成员访问器
        /// </summary>
        /// <param name="type"></param>
        public MemberAccessor(Type type)
        {
            Type = type;
            this.OnGetFieldInfes = (t) => { return t.GetFields(); };
            this.OnGetProperties = (t) => { return t.GetProperties(); };
        }

        /// <summary>
        /// 构建
        /// </summary>
        public void Build()
        {
            GetValueDelegate = GenerateGetValue();
            SetValueDelegate = GenerateSetValue();
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        public Func<Type, PropertyInfo[]> OnGetProperties { get; set; }

        /// <summary>
        /// 获取字段
        /// </summary>
        public Func<Type, FieldInfo[]> OnGetFieldInfes { get; set; }

        /// <summary>
        /// 所属类型
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc/>
        public object GetValue(object instance, string memberName)
        {
            return GetValueDelegate(instance, memberName);
        }

        /// <inheritdoc/>
        public void SetValue(object instance, string memberName, object newValue)
        {
            SetValueDelegate(instance, memberName, newValue);
        }

        private Func<object, string, object> GenerateGetValue()
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in this.OnGetFieldInfes.Invoke(Type))
            {
                var property = Expression.Field(Expression.Convert(instance, Type), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in this.OnGetProperties.Invoke(Type))
            {
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            if (cases.Count == 0)
            {
                return (a, b) => default;
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Func<object, string, object>>(methodBody, instance, memberName).Compile();
        }

        private Action<object, string, object> GenerateSetValue()
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in this.OnGetFieldInfes.Invoke(Type))
            {
                var property = Expression.Field(Expression.Convert(instance, Type), propertyInfo.Name);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.FieldType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in this.OnGetProperties(Type))
            {
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo.Name);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            if (cases.Count == 0)
            {
                return (a, b, c) => { };
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Action<object, string, object>>(methodBody, instance, memberName, newValue).Compile();
        }
    }
}