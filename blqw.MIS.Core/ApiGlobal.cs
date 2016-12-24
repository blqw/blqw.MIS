using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.DataModification;
using blqw.MIS.Filters;
using blqw.MIS.Validation;

namespace blqw.MIS
{
    /// <summary>
    /// API全局操作基类
    /// </summary>
    public abstract partial class ApiGlobal
    {
        public abstract void Initialization();

        public ICollection<ApiFilterAttribute> Filters { get; } = new List<ApiFilterAttribute>();

        public ICollection<DataValidationAttribute> Validations { get; } = new List<DataValidationAttribute>();

        public ICollection<DataModificationAttribute> Modifications { get; } = new List<DataModificationAttribute>();

    }
}
