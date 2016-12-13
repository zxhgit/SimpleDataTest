using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Zx.Web.ApiContainer.core
{
    public class ModelBindingContext
    {
        private string _modelName;
        public ModelBindingContext()
            : this(null)
        { }

        // copies certain values that won't change between parent and child objects
        public ModelBindingContext(ModelBindingContext bindingContext)
        {
            if (bindingContext != null)
                ValueProvider = bindingContext.ValueProvider;
        }

        public object Model { get; set; }

        public string ModelName
        {
            get { return _modelName ?? (_modelName = String.Empty); }
            set { _modelName = value; }
        }

        public Type ModelType { get; set; }

        public IValueProvider ValueProvider { get; set; }

        public virtual bool IsComplexType
        {
            get { return !(TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string))); }
        }

    }
}