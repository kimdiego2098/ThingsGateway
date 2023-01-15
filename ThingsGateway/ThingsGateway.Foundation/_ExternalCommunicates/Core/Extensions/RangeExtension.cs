namespace ThingsGateway.Foundation.Extension
{
#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// RangeExtension
    /// </summary>
    public static class RangeExtension
    {
        /// <summary>
        /// 枚举扩展
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static CustomIntEnumerator GetEnumerator(this Range range)
        {
            return new CustomIntEnumerator(range);
        }
    }

    /// <summary>
    /// CustomIntEnumerator
    /// </summary>
    public ref struct CustomIntEnumerator
    {
        private int m_current;
        private readonly int m_end;

        /// <summary>
        /// CustomIntEnumerator
        /// </summary>
        /// <param name="range"></param>
        public CustomIntEnumerator(Range range)
        {
            if (range.End.IsFromEnd)
            {
                throw new NotSupportedException("不支持无限枚举。");
            }
            m_current = range.Start.Value - 1;
            m_end = range.End.Value;
        }

        /// <summary>
        /// Current
        /// </summary>
        public int Current => m_current;

        /// <summary>
        /// MoveNext
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            m_current++;
            return m_current <= m_end;
        }
    }
#endif
}