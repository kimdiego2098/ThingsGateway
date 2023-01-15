using System.Collections.Concurrent;
using System.Reflection;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// DynamicMethodMemberAccessor
    /// </summary>
    public class DynamicMethodMemberAccessor : IMemberAccessor
    {
        private static ConcurrentDictionary<Type, IMemberAccessor> classAccessors = new ConcurrentDictionary<Type, IMemberAccessor>();

        /// <summary>
        /// 获取属性
        /// </summary>
        public Func<Type, PropertyInfo[]> OnGetProperties { get; set; }

        /// <summary>
        /// 获取字段
        /// </summary>
        public Func<Type, FieldInfo[]> OnGetFieldInfes { get; set; }

        /// <inheritdoc/>
        public object GetValue(object instance, string memberName)
        {
            return FindClassAccessor(instance).GetValue(instance, memberName);
        }

        /// <inheritdoc/>
        public void SetValue(object instance, string memberName, object newValue)
        {
            FindClassAccessor(instance).SetValue(instance, memberName, newValue);
        }

        private IMemberAccessor FindClassAccessor(object instance)
        {
            var typekey = instance.GetType();
            if (!classAccessors.TryGetValue(typekey, out IMemberAccessor classAccessor))
            {
                MemberAccessor memberAccessor = new MemberAccessor(instance.GetType());
                if (this.OnGetFieldInfes != null)
                {
                    memberAccessor.OnGetFieldInfes = this.OnGetFieldInfes;
                }

                if (this.OnGetProperties != null)
                {
                    memberAccessor.OnGetProperties = this.OnGetProperties;
                }
                memberAccessor.Build();
                classAccessor = memberAccessor;
                classAccessors.TryAdd(typekey, classAccessor);
            }
            return classAccessor;
        }
    }
}
