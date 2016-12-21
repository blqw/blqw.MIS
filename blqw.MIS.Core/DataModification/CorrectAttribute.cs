using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.DataModification
{
    public class CorrectAttribute : DataModificationAttribute
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
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(double min, double max)
        {
            _min = min;
            _max = max;
            _type = typeof(double);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(long min, long max)
        {
            _min = min;
            _max = max;
            _type = typeof(long);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(uint min, uint max)
        {
            _min = min;
            _max = max;
            _type = typeof(uint);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(ulong min, ulong max)
        {
            _min = min;
            _max = max;
            _type = typeof(ulong);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(float min, float max)
        {
            _min = min;
            _max = max;
            _type = typeof(float);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(decimal min, decimal max)
        {
            _min = min;
            _max = max;
            _type = typeof(decimal);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(byte min, byte max)
        {
            _min = min;
            _max = max;
            _type = typeof(byte);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(sbyte min, sbyte max)
        {
            _min = min;
            _max = max;
            _type = typeof(sbyte);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(short min, short max)
        {
            _min = min;
            _max = max;
            _type = typeof(short);
        }

        /// <summary>
        ///     判断参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(ushort min, ushort max)
        {
            _min = min;
            _max = max;
            _type = typeof(ushort);
        }

        /// <summary>
        ///     判断 int 类型参数是否超出范围
        /// </summary>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        public CorrectAttribute(int min, int max)
        {
            _min = min;
            _max = max;
            _type = typeof(int);
        }

        /// <summary>
        ///     判断 DateTime 类型参数是否超出范围
        /// </summary>
        /// <param name="min">最小时间(包含)</param>
        /// <param name="max">最大时间(包含)</param>
        public CorrectAttribute(string min, string max)
        {
            if (DateTime.TryParse(min, out var time) == false)
                throw new FormatException(min + " 不是有效的时间值");
            _min = time;
            if (DateTime.TryParse(max, out time) == false)
                throw new FormatException(max + " 不是有效的时间值");
            _max = time;
            _type = typeof(DateTime);
        }

        
        public override void Modifies(ref object arg)
        {
            var value = arg as IComparable;
            if (value == null || arg == null || arg.GetType() != _type) return;
            if (value.CompareTo(_min) < 0) arg = _min;
            else if (value.CompareTo(_max) > 0) arg = _max;
        }
    }
}
