using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Zx.Web.ApiContainer.core
{
    public class ValueProviderCollection : Collection<IValueProvider>, IValueProvider
    {
        public ValueProviderCollection()
        {
        }

        public ValueProviderCollection(IList<IValueProvider> list)
            : base(list)
        {
        }

        public virtual bool ContainsPrefix(string prefix)
        {
            return this.Any(vp => vp.ContainsPrefix(prefix));
        }

        public virtual ValueProviderResult GetValue(string key)
        {
            return (from provider in this
                    let result = provider.GetValue(key)
                    where result != null
                    select result).FirstOrDefault();
        }
    }
}