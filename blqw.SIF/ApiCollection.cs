using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;

namespace blqw.SIF
{
    public class ApiCollection
    {
        public ICollection<NamespaceDescriptor> Namespaces { get; set; }
        public ICollection<TypeDescriptor> Types { get; set; }
        public ICollection<ApiDescriptor> Apis { get; set; }
    }
}
