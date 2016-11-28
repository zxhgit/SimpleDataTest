using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zx.Web.ApiContainer.ErrorHandler;
using Zx.Web.ApiContainer.external;

namespace Zx.Web.ApiContainer.core
{
    public class DefaultModelBinder
    {
        internal void BindComplexElementalModel(ModelBindingContext bindingContext, object model)
        {
            var newBindingContext = new ModelBindingContext
            {
                ModelName = bindingContext.ModelName,
                ModelType = bindingContext.ModelType,
                Model = model,
                ValueProvider = bindingContext.ValueProvider
            };
            BindProperties(newBindingContext);
        }

        internal object BindComplexModel(ModelBindingContext bindingContext)
        {
            var model = bindingContext.Model;
            var modelType = bindingContext.ModelType;
            if (model == null && modelType.IsArray)
            {
                var elementType = modelType.GetElementType();
                var listType = typeof(List<>).MakeGenericType(elementType);
                var collection = CreateModel(listType);
                var arrayBindingContext = new ModelBindingContext
                {
                    ModelName = bindingContext.ModelName,
                    ModelType = listType,
                    Model = collection,
                    ValueProvider = bindingContext.ValueProvider
                };
                var list = (IList)UpdateCollection(arrayBindingContext, elementType);
                if (list == null) return null;
                var array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            if (model == null) model = CreateModel(modelType);
            var dictionaryType = TypeHelpers.ExtractGenericInterface(modelType, typeof(IDictionary<,>));
            if (dictionaryType != null)
            {
                var genericArguments = dictionaryType.GetGenericArguments();
                var keyType = genericArguments[0];
                var valueType = genericArguments[1];
                var dictionaryBindingContext = new ModelBindingContext
                {
                    ModelName = bindingContext.ModelName,
                    ModelType = modelType,
                    Model = model,
                    ValueProvider = bindingContext.ValueProvider
                };
                return UpdateDictionary(dictionaryBindingContext, keyType, valueType);
            }
            var enumerableType = TypeHelpers.ExtractGenericInterface(modelType, typeof(IEnumerable<>));
            if (enumerableType != null)
            {
                var elementType = enumerableType.GetGenericArguments()[0];
                var collectionType = typeof(ICollection<>).MakeGenericType(elementType);
                if (collectionType.IsInstanceOfType(model))
                {
                    var collectionBindingContext = new ModelBindingContext
                    {
                        ModelName = bindingContext.ModelName,
                        ModelType = modelType,
                        Model = model,
                        ValueProvider = bindingContext.ValueProvider
                    };
                    return UpdateCollection(collectionBindingContext, elementType);
                }
            }
            BindComplexElementalModel(bindingContext, model);
            return model;
        }

        internal object BindSimpleModel(ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
        {
            if (bindingContext.ModelType.IsInstanceOfType(valueProviderResult.RawValue))
                return valueProviderResult.RawValue;
            if (bindingContext.ModelType != typeof(string))
            {
                if (bindingContext.ModelType.IsArray)//array
                    return ConvertProviderResult(valueProviderResult, bindingContext.ModelType);
                var enumerableType = TypeHelpers.ExtractGenericInterface(bindingContext.ModelType, typeof(IEnumerable<>));
                if (enumerableType != null)//IEnumerable
                {
                    var modelCollection = CreateModel(bindingContext.ModelType);
                    var elementType = enumerableType.GetGenericArguments()[0];
                    var arrayType = elementType.MakeArrayType();
                    var modelArray = ConvertProviderResult(valueProviderResult, arrayType);
                    var collectionType = typeof(ICollection<>).MakeGenericType(elementType);
                    if (collectionType.IsInstanceOfType(modelCollection))
                        CollectionHelpers.ReplaceCollection(elementType, modelCollection, modelArray);
                    return modelCollection;
                }
            }
            return ConvertProviderResult(valueProviderResult, bindingContext.ModelType);
        }

        public virtual object BindModel(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");
            var performedFallback = false;
            if (!String.IsNullOrEmpty(bindingContext.ModelName) && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                //顶级元素为复杂对象，在http中无参数名
                bindingContext = new ModelBindingContext
                {
                    ModelType = bindingContext.ModelType,
                    ValueProvider = bindingContext.ValueProvider
                };
                performedFallback = true;
            }
            if (!performedFallback)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult != null) return BindSimpleModel(bindingContext, valueProviderResult);
            }
            return !bindingContext.IsComplexType ? null : BindComplexModel(bindingContext);
        }

        private void BindProperties(ModelBindingContext bindingContext)
        {
            var properties = GetFilteredModelProperties(bindingContext);
            foreach (var property in properties)
                BindProperty(bindingContext, property);
        }

        protected virtual void BindProperty(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var fullPropertyKey = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
            if (!bindingContext.ValueProvider.ContainsPrefix(fullPropertyKey)) return;
            var propertyBinder = new DefaultModelBinder();
            var innerBindingContext = new ModelBindingContext()
            {
                ModelName = fullPropertyKey,
                ModelType = propertyDescriptor.PropertyType,//
                ValueProvider = bindingContext.ValueProvider
            };
            var newPropertyValue = GetPropertyValue(innerBindingContext, propertyDescriptor, propertyBinder);
            innerBindingContext.Model = newPropertyValue;
            SetProperty(bindingContext, propertyDescriptor, newPropertyValue);
        }

        private static bool CanUpdateReadonlyTypedReference(Type type)
        {
            if (type.IsValueType || type.IsArray || type == typeof(string))
                return false;
            return true;
        }

        private static object ConvertProviderResult(ValueProviderResult valueProviderResult, Type destinationType)
        {
            try
            {
                object convertedValue = valueProviderResult.ConvertTo(destinationType);
                return convertedValue;
            }
            catch (Exception ex)
            {
                throw new ParameterTypeException(valueProviderResult.RawName);
                //modelState.AddModelError(modelStateKey, ex);
                return null;
            }
        }

        protected virtual object CreateModel(Type modelType)
        {
            var typeToCreate = modelType;
            if (modelType.IsGenericType)
            {
                var genericTypeDefinition = modelType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IDictionary<,>))
                    typeToCreate = typeof(Dictionary<,>).MakeGenericType(modelType.GetGenericArguments());
                else if (genericTypeDefinition == typeof(IEnumerable<>) ||
                         genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IList<>))
                    typeToCreate = typeof(List<>).MakeGenericType(modelType.GetGenericArguments());
            }
            return Activator.CreateInstance(typeToCreate);
        }

        protected static string CreateSubIndexName(string prefix, string index)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
        }

        protected internal static string CreateSubPropertyName(string prefix, string propertyName)
        {
            if (String.IsNullOrEmpty(prefix)) return propertyName;
            if (String.IsNullOrEmpty(propertyName)) return prefix;
            return prefix + "." + propertyName;
        }

        private static IEnumerable<string> GetZeroBasedIndexes()
        {
            int i = 0;
            while (true)
            {
                yield return i.ToString(CultureInfo.InvariantCulture);
                i++;
            }
        }

        protected virtual object GetPropertyValue(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, DefaultModelBinder propertyBinder)
        {
            var value = propertyBinder.BindModel(bindingContext);
            if (Equals(value, String.Empty)) return null;
            return value;
        }

        protected virtual ICustomTypeDescriptor GetTypeDescriptor(ModelBindingContext bindingContext)
        {
            var type = bindingContext.ModelType;
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }

        protected IEnumerable<PropertyDescriptor> GetFilteredModelProperties(ModelBindingContext bindingContext)
        {
            PropertyDescriptorCollection properties = GetTypeDescriptor(bindingContext).GetProperties();
            return from PropertyDescriptor property in properties
                   where ShouldUpdateProperty(property)
                   select property;
        }

        private static void GetIndexes(ModelBindingContext bindingContext, out bool stopOnIndexNotFound, out IEnumerable<string> indexes)
        {
            string indexKey = CreateSubPropertyName(bindingContext.ModelName, "index");
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(indexKey);

            if (valueProviderResult != null)
            {
                string[] indexesArray = valueProviderResult.ConvertTo(typeof(string[])) as string[];
                if (indexesArray != null)
                {
                    stopOnIndexNotFound = false;
                    indexes = indexesArray;
                    return;
                }
            }

            // just use a simple zero-based system
            stopOnIndexNotFound = true;
            indexes = GetZeroBasedIndexes();
        }

        protected virtual void SetProperty(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            bool isNullValueOnNonNullableType =
                value == null &&
                !TypeHelpers.TypeAllowsNullValue(propertyDescriptor.PropertyType);

            // Try to set a value into the property unless we know it will fail (read-only
            // properties and null values with non-nullable types)
            if (!propertyDescriptor.IsReadOnly && !isNullValueOnNonNullableType)
            {
                try
                {
                    propertyDescriptor.SetValue(bindingContext.Model, value);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private static bool ShouldUpdateProperty(PropertyDescriptor property)
        {
            if (property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
            {
                return false;
            }
            // otherwise, allow
            return true;
        }

        internal object UpdateCollection(ModelBindingContext bindingContext, Type elementType)
        {
            bool stopOnIndexNotFound;
            IEnumerable<string> indexes;
            GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);
            var elementBinder = new DefaultModelBinder();
            // build up a list of items from the request
            var modelList = new List<object>();
            foreach (var currentIndex in indexes)
            {
                var subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
                if (!bindingContext.ValueProvider.ContainsPrefix(subIndexKey))
                {
                    if (stopOnIndexNotFound) break;// we ran out of elements to pull
                    continue;
                }
                var innerContext = new ModelBindingContext
                {
                    ModelName = subIndexKey,
                    ModelType = elementType,
                    Model = null,
                    ValueProvider = bindingContext.ValueProvider
                };
                var thisElement = elementBinder.BindModel(innerContext);
                modelList.Add(thisElement);
            }
            // if there weren't any elements at all in the request, just return
            if (modelList.Count == 0) return null;
            // replace the original collection
            var collection = bindingContext.Model;
            CollectionHelpers.ReplaceCollection(elementType, collection, modelList);
            return collection;
        }

        internal object UpdateDictionary(ModelBindingContext bindingContext, Type keyType, Type valueType)
        {
            bool stopOnIndexNotFound;
            IEnumerable<string> indexes;
            GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);
            var keyBinder = new DefaultModelBinder();
            var valueBinder = new DefaultModelBinder();
            var modelList = new List<KeyValuePair<object, object>>();// build up a list of items from the request
            foreach (string currentIndex in indexes)
            {
                var subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
                var keyFieldKey = CreateSubPropertyName(subIndexKey, "key");
                var valueFieldKey = CreateSubPropertyName(subIndexKey, "value");
                if (!(bindingContext.ValueProvider.ContainsPrefix(keyFieldKey) && bindingContext.ValueProvider.ContainsPrefix(valueFieldKey)))
                {
                    if (stopOnIndexNotFound) break;// we ran out of elements to pull
                    continue;
                }
                var keyBindingContext = new ModelBindingContext // bind the key
                {
                    ModelName = keyFieldKey,
                    ModelType = keyType,
                    Model = null,
                    ValueProvider = bindingContext.ValueProvider
                };
                var thisKey = keyBinder.BindModel(keyBindingContext);
                // we can't add an invalid key, so just move on
                if (!keyType.IsInstanceOfType(thisKey)) continue;
                // bind the value
                modelList.Add(CreateEntryForModel(bindingContext, valueType, valueBinder, valueFieldKey, thisKey));
            }
            // Let's try another method
            if (modelList.Count == 0)
            {
                var enumerableValueProvider = bindingContext.ValueProvider as IEnumerableValueProvider;
                if (enumerableValueProvider != null)
                {
                    var keys = enumerableValueProvider.GetKeysFromPrefix(bindingContext.ModelName);
                    foreach (var thisKey in keys)
                        modelList.Add(CreateEntryForModel(bindingContext, valueType, valueBinder,
                            thisKey.Value, thisKey.Key));
                }
            }
            // if there weren't any elements at all in the request, just return
            if (modelList.Count == 0) return null;
            // replace the original collection
            var dictionary = bindingContext.Model;
            CollectionHelpers.ReplaceDictionary(keyType, valueType, dictionary, modelList);
            return dictionary;
        }

        private static KeyValuePair<object, object> CreateEntryForModel(ModelBindingContext bindingContext, Type valueType, DefaultModelBinder valueBinder, string modelName, object modelKey)
        {
            var valueBindingContext = new ModelBindingContext
            {
                ModelName = modelName,
                ModelType = valueType,
                Model = null,
                ValueProvider = bindingContext.ValueProvider
            };
            var thisValue = valueBinder.BindModel(valueBindingContext);
            return new KeyValuePair<object, object>(modelKey, thisValue);
        }


        // This helper type is used because we're working with strongly-typed collections, but we don't know the Ts
        // ahead of time. By using the generic methods below, we can consolidate the collection-specific code in a
        // single helper type rather than having reflection-based calls spread throughout the DefaultModelBinder type.
        // There is a single point of entry to each of the methods below, so they're fairly simple to maintain.

        private static class CollectionHelpers
        {
            private static readonly MethodInfo _replaceCollectionMethod = typeof(CollectionHelpers).GetMethod("ReplaceCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
            private static readonly MethodInfo _replaceDictionaryMethod = typeof(CollectionHelpers).GetMethod("ReplaceDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ReplaceCollection(Type collectionType, object collection, object newContents)
            {
                MethodInfo targetMethod = _replaceCollectionMethod.MakeGenericMethod(collectionType);
                targetMethod.Invoke(null, new object[] { collection, newContents });
            }

            private static void ReplaceCollectionImpl<T>(ICollection<T> collection, IEnumerable newContents)
            {
                collection.Clear();
                if (newContents != null)
                {
                    foreach (object item in newContents)
                    {
                        // if the item was not a T, some conversion failed. the error message will be propagated,
                        // but in the meanwhile we need to make a placeholder element in the array.
                        T castItem = (item is T) ? (T)item : default(T);
                        collection.Add(castItem);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ReplaceDictionary(Type keyType, Type valueType, object dictionary, object newContents)
            {
                MethodInfo targetMethod = _replaceDictionaryMethod.MakeGenericMethod(keyType, valueType);
                targetMethod.Invoke(null, new object[] { dictionary, newContents });
            }

            private static void ReplaceDictionaryImpl<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<object, object>> newContents)
            {
                dictionary.Clear();
                foreach (KeyValuePair<object, object> item in newContents)
                {
                    // if the item was not a T, some conversion failed. the error message will be propagated,
                    // but in the meanwhile we need to make a placeholder element in the dictionary.
                    TKey castKey = (TKey)item.Key; // this cast shouldn't fail
                    TValue castValue = (item.Value is TValue) ? (TValue)item.Value : default(TValue);
                    dictionary[castKey] = castValue;
                }
            }
        }
    }
}