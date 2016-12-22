using System;
using System.Collections.Generic;

namespace blqw.MIS.Validation
{
    /// <summary> 
    /// 验证字符串长度是否在指定的范围之内
    /// </summary>
    /// <remarks>peng,li 2015年7月3日</remarks>
    public class StringLengthAttribute : DataValidationAttribute
    {
        /// <summary> 最大长度
        /// </summary>
        private readonly int _maxLength;
        /// <summary> 最小长度
        /// </summary>
        private readonly int _minLength;

        /// <summary> 
        /// 指定字符串的最小和最大长度,初始化特性
        /// </summary>
        /// <param name="minLength">字符串最小长度(包含)</param>
        /// <param name="maxLength">字符串最大长度(包含)</param>
        public StringLengthAttribute(int minLength, int maxLength = 2000)
        {
            if (minLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength), "不能小于0");
            }
            if (maxLength < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), $"不能小于 {nameof(minLength)}");
            }
            _maxLength = maxLength;
            _minLength = minLength;
        }
        
        public override string GetDescription(Type type)
        {
            if (_maxLength == int.MaxValue)
            {
                return $"文本长度不能小于 {_minLength}";
            }
            if (_maxLength == _minLength)
            {
                return $"文本长度必须等于 {_minLength}";
            }
            if (_minLength == 0)
            {
                return $"文本长度不能大于 {_maxLength}";
            }
            return $"文本长度不能小于 {_minLength} 或大于 {_maxLength} ";
        }

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var str = value as string;
            if (str == null)
            {
                return true;
            }
            return str.Length >= _minLength && str.Length <= _maxLength ;
        }
        protected override IEnumerable<Type> AllowTypes { get; } = new[] { typeof(string) };
    }
}