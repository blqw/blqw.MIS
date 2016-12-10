using System;
using System.Collections.Generic;

namespace blqw.SIF.Validation
{
    /// <summary>
    ///     范围判断
    /// </summary>
    public class RangeAttribute : DataValidationAttribute
    {
        /// <summary>
        ///     允许的最大值
        /// </summary>
        private readonly IComparable _max;

        /// <summary>
        ///     允许的最小值
        /// </summary>
        private readonly IComparable _min;

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(double min, double max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(long min, long max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(uint min, uint max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(ulong min, ulong max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(float min, float max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(decimal min, decimal max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(byte min, byte max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(sbyte min, sbyte max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(short min, short max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(ushort min, ushort max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断 int 类型参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public RangeAttribute(int min, int max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     判断 DateTime 类型参数是否超出范围
        /// </summary>
        /// <param name="min">最小时间(包含)</param>
        /// <param name="max">最大时间(包含)</param>
        public RangeAttribute(string min, string max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            DateTime time;
            if (DateTime.TryParse(min, out time) == false)
                throw new FormatException(min + " 不是有效的时间值");
            _min = time;
            if (DateTime.TryParse(max, out time) == false)
                throw new FormatException(max + " 不是有效的时间值");
            _max = time;
        }


        public override string GetDescription(Type type) => $"值允许范围( {_min}～{_max} )";

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var comparable = value as IComparable;
            if (comparable == null)
            {
                return true;
            }
            return comparable.CompareTo(_min) >= 0 && comparable.CompareTo(_max) <= 0;
        }
    }
}