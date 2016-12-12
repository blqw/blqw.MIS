using System;
using System.Collections.Generic;
using System.Reflection;

namespace blqw.UIF.Validation
{
    /// <summary>
    ///     范围判断
    /// </summary>
    public class RangeAttribute : DataValidationAttribute
    {
        /// <summary>
        ///     允许的最大值
        /// </summary>
        private readonly double _max;

        /// <summary>
        ///     允许的最小值
        /// </summary>
        private readonly double _min;

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
        ///     判断 DateTime 类型参数是否超出范围
        /// </summary>
        /// <param name="min">最小时间(包含)</param>
        /// <param name="max">最大时间(包含)</param>
        public RangeAttribute(string min, string max)
            : base(-102, $"参数 {{name}} 超过允许范围( {min}~{max} )")
        {
            if (DateTime.TryParse(min, out var time) == false)
                throw new FormatException(min + " 不是有效的时间值");
            _min = time.Ticks;
            if (DateTime.TryParse(max, out time) == false)
                throw new FormatException(max + " 不是有效的时间值");
            _max = time.Ticks;
        }


        public override string GetDescription(Type type) => $"值允许范围( {_min}～{_max} )";

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var d = ConvertToDouble(value);
            if (double.IsNaN(d)) return true;
            return d >= _min && d <= _max;
        }


        private static double ConvertToDouble(object value)
        {
            if (value is IComparable == false) return double.NaN;
            if (value is ushort) return (double)(ushort)value;
            if (value is decimal) return (double)(decimal)value;
            if (value is decimal) return (double)(decimal)value;
            if (value is double) return (double)value;
            if (value is int) return (double)(int)value;
            if (value is float) return (double)(float)value;
            if (value is ulong) return (double)(ulong)value;
            if (value is long) return (double)(long)value;
            if (value is sbyte) return (double)(sbyte)value;
            if (value is byte) return (double)(byte)value;
            if (value is char) return (double)(char)value;
            if (value is short) return (double)(short)value;
            if (value is uint) return (double)(uint)value;
            if (value is DateTime) return (double)((DateTime)value).Ticks;
            return double.NaN;
        }
    }
}