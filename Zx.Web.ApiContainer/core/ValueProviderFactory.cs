using Nancy;
using Nancy.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Zx.Web.ApiContainer.ErrorHandler;

namespace Zx.Web.ApiContainer.core
{
    public abstract class ValueProviderFactory
    {
        public abstract IValueProvider GetValueProvider(NancyContext context);
    }

    public class ValueProviderFactoryCollection : Collection<ValueProviderFactory>
    {
        public ValueProviderFactoryCollection(IList<ValueProviderFactory> list)
            : base(list)
        {
        }

        public IValueProvider GetValueProvider(NancyContext context)
        {
            var valueProviders = from factory in this
                                 let valueProvider = factory.GetValueProvider(context)
                                 where valueProvider != null
                                 select valueProvider;
            return new ValueProviderCollection(valueProviders.ToList());
        }
    }

    #region providerfactory implement

    public class FormValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(NancyContext context)
        {
            var req = context.Request;
            var query = ((IDictionary<string, object>)req.Form).ToDictionary(d => d.Key, d => d.Value.ToString(),
                //不用字符串则无法转型
                StringComparer.OrdinalIgnoreCase);
            return new DictionaryValueProvider<string>(query, CultureInfo.CurrentCulture);
        }
    }

    public class JsonValueProviderFactory : ValueProviderFactory
    {
        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
        {
            var d = value as IDictionary<string, object>;
            if (d != null)
            {
                foreach (var entry in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
                }
                return;
            }
            var l = value as IList;
            if (l != null)
            {
                for (var i = 0; i < l.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                }
                return;
            }
            // primitive
            backingStore.Add(prefix, value);
        }

        private static object GetDeserializedObject(Request request)
        {
            if (!request.Headers.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return null; // not JSON request            
            var reader = new StreamReader(request.Body);
            var bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText)) return null; // no JSON data
            var serializer = new JavaScriptSerializer(); //Nancy.Json.JavaScriptSerializer
            return serializer.DeserializeObject(bodyText);
        }

        public override IValueProvider GetValueProvider(NancyContext context)
        {
            var request = context.Request;
            object jsonData;
            try
            {
                jsonData = GetDeserializedObject(request);
            }
            catch (Exception e)
            {
                throw new ParameterFormatException(e);
            }
            if (jsonData == null) return null;
            var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var backingStoreWrapper = new EntryLimitedDictionary(backingStore);
            AddToBackingStore(backingStoreWrapper, String.Empty, jsonData);
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        public IValueProvider GetValueProviderFake(string bodyText)
        {
            var serializer = new JavaScriptSerializer(); //Nancy.Json.JavaScriptSerializer
            var jsonData = serializer.DeserializeObject(bodyText);
            if (jsonData == null) return null;
            var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var backingStoreWrapper = new EntryLimitedDictionary(backingStore);
            AddToBackingStore(backingStoreWrapper, String.Empty, jsonData);
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }

        private class EntryLimitedDictionary
        {
            private static int _maximumDepth = GetMaximumDepth();
            private readonly IDictionary<string, object> _innerDictionary;
            private int _itemCount = 0;

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                _innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                if (++_itemCount > _maximumDepth)
                    throw new InvalidOperationException("The JSON request was too large to be deserialized.");
                _innerDictionary.Add(key, value);
            }

            private static int GetMaximumDepth()
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                {
                    var valueArray = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                    if (valueArray != null && valueArray.Length > 0)
                    {
                        int result;
                        if (Int32.TryParse(valueArray[0], out result)) return result;
                    }
                }
                return 1000; // Fallback default
            }
        }
    }

    public class RouteValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(NancyContext context)
        {
            var routeParameters = context.Parameters;
            var query = ((IDictionary<string, object>)routeParameters).ToDictionary(d => d.Key, d => d.Value.ToString(),
                //不用字符串则无法转型
                StringComparer.OrdinalIgnoreCase);
            return new DictionaryValueProvider<string>(query, CultureInfo.CurrentCulture);
        }
    }

    public class QueryValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(NancyContext context)
        {
            var req = context.Request;
            var query = ((IDictionary<string, object>)req.Query).ToDictionary(d => d.Key, d => d.Value.ToString(),
                //不用字符串则无法转型
                StringComparer.OrdinalIgnoreCase);
            return new DictionaryValueProvider<string>(query, CultureInfo.CurrentCulture);
        }
    }

    #endregion

}