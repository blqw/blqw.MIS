using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF.DataModification;
using blqw.SIF.Filters;
using blqw.SIF.Validation;

namespace blqw.SIF
{
    /// <summary>
    /// Api全局操作基类
    /// </summary>
    public abstract class ApiGlobal
    {
        public abstract void Initialization();

        public ICollection<ApiFilterAttribute> Filters { get; } = new List<ApiFilterAttribute>();

        public ICollection<DataValidationAttribute> Validations { get; } = new List<DataValidationAttribute>();

        public ICollection<DataModificationAttribute> Modifications { get; } = new List<DataModificationAttribute>();
    }
}
