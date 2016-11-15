using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Services
{
    public sealed class ServiceCollection : ILookup<Type, IService>, IServiceProvider
    {
        public IEnumerator<IGrouping<Type, IService>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(Type key)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }

        public IEnumerable<IService> this[Type key]
        {
            get { throw new NotImplementedException(); }
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
