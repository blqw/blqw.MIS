using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.DataModification
{
    /// <summary>
    /// 溢出检查特性 <para/>
    /// 当数值超过上下限时修正为最大值或最小值
    /// </summary>
    public class CheckedAttribute : DataModificationAttribute
    {
        /// <summary>
        /// 允许的最大值
        /// </summary>
        private readonly IComparable _max;

        /// <summary>
        /// 允许的最小值
        /// </summary>
        private readonly IComparable _min;

        private readonly Type _type;

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(double min, double max)
        {
            _min = min;
            _max = max;
            _type = typeof(double);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(long min, long max)
        {
            _min = min;
            _max = max;
            _type = typeof(long);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(uint min, uint max)
        {
            _min = min;
            _max = max;
            _type = typeof(uint);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(ulong min, ulong max)
        {
            _min = min;
            _max = max;
            _type = typeof(ulong);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(float min, float max)
        {
            _min = min;
            _max = max;
            _type = typeof(float);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(decimal min, decimal max)
        {
            _min = min;
            _max = max;
            _type = typeof(decimal);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(byte min, byte max)
        {
            _min = min;
            _max = max;
            _type = typeof(byte);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(sbyte min, sbyte max)
        {
            _min = min;
            _max = max;
            _type = typeof(sbyte);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(short min, short max)
        {
            _min = min;
            _max = max;
            _type = typeof(short);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(ushort min, ushort max)
        {
            _min = min;
            _max = max;
            _type = typeof(ushort);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CheckedAttribute(int min, int max)
        {
            _min = min;
            _max = max;
            _type = typeof(int);
        }

        /// <summary>
        ///     设置溢出检查的最大值与最小值
        /// </summary>
        /// <param name="min">最小时间(包含)</param>
        /// <param name="max">最大时间(包含)</param>
        public CheckedAttribute(string min, string max)
        {
            if (DateTime.TryParse(min, out var time) == false)
                throw new FormatException(min + " 不是有效的时间值");
            _min = time;
            if (DateTime.TryParse(max, out time) == false)
                throw new FormatException(max + " 不是有效的时间值");
            _max = time;
            _type = typeof(DateTime);
        }


        /// <summary>
        ///     如果值溢出，则改为有效的值
        /// </summary>
        /// <param name="arg">待修改的值</param>
        /// <returns></returns>
        public override void Modifies(ref object arg)
        {
            var value = arg as IComparable;
            if (value == null || arg == null || arg.GetType() != _type) return;
            if (value.CompareTo(_min) < 0) arg = _min;
            else if (value.CompareTo(_max) > 0) arg = _max;
        }

        /// <summary>
        /// 当前特性的允许类型
        /// </summary>
        protected override IEnumerable<Type> AllowTypes { get; }
            = new[]
            {
                typeof(double),
                typeof(long),
                typeof(uint),
                typeof(ulong),
                typeof(float),
                typeof(decimal),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(DateTime),
            };
    }
}
