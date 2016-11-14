using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiAdmin.Utilities
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflectHelper
    {
        /// <summary>
        /// 锁定
        /// </summary>
        static object _LockPad = new object();

        /// <summary>
        /// 类型全名称，类实例存储词典
        /// </summary>
        static Dictionary<string, object> _ImplementorDic = new Dictionary<string, object>();

        /// <summary>
        /// key 类型全名称+"."+方法名称， value 制定名称的方法列表
        /// </summary>
        static Dictionary<string, MethodInfo[]> _TypeNameMethodListDic = new Dictionary<string, MethodInfo[]>();



        /// <summary>
        /// key 类型全名称+"."+方法名称， value 制定名称的方法列表
        /// </summary>
        static Dictionary<string, MethodInfo> _TypeNameSingleMethodListDic = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// 根据构造函数获取实例对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="constructor">构造函数</param>
        /// <returns></returns>
        public static T GetInterfaceInstanceByConstroctor<T>(ConstructorInfo constructor)
        {
            Type[] paraTypes = new Type[0];

            var t = (T)constructor.Invoke(null);

            return t;
        }

        /// <summary>
        /// 通过Type创建构造函数（没有参数的构造函数）
        /// </summary>
        /// <param name="type"></param>
        public static ConstructorInfo GetDefaultConstructorInfo(Type type)
        {
            Type[] paraTypes = new Type[0];

            ConstructorInfo info = type.GetConstructor(paraTypes);

            return info;
        }

        /// <summary>
        /// 通过Type创建构造函数（制定参数类型）
        /// </summary>
        /// <param name="classWholePath"></param>
        /// <param name="paramTypes"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(string classWholePath, Type[] paramTypes)
        {
            ConstructorInfo info = null;

            Type t = Type.GetType(classWholePath);

            if (t != null)
            {
                info = t.GetConstructor(paramTypes);
            }

            return info;
        }

        /// <summary>
        /// 根据全路径和数据集名称获取构造函数
        /// 获取无参数的构造函数
        /// </summary>
        /// <param name="classWholePath">类全路径</param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(string classWholePath)
        {

            Type t = Type.GetType(classWholePath);

            if (t != null)
            {
                return GetDefaultConstructorInfo(t);
            }

            return null;
        }

        /// <summary>
        /// 通过类全路径创建Type
        /// </summary>
        /// <param name="classWholePath">类全路径</param>
        /// <returns></returns>
        public static Type GetTypeByWholeUrl(string classWholePath)
        {
            Type t = Type.GetType(classWholePath);

            if (t == null)
            {
                string[] typeArray = classWholePath.Split(',');

                if (typeArray != null && typeArray.Length > 1)
                {
                    string asmName = typeArray[1] + ".dll";

                    Assembly asmCurrent = Assembly.GetExecutingAssembly();
                    string codeBase = asmCurrent.CodeBase;
                    int pos = codeBase.LastIndexOf(@"/");
                    string dictionary = codeBase.Substring(0, pos + 1);

                    Assembly asm = Assembly.LoadFrom(dictionary + asmName);

                    t = asm.GetType(typeArray[0]);
                }

            }

            return t;
        }

        /// <summary>
        /// 根据类型获取全路径（反射获取用）
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetTypeFullUrlWithProcessorName(Type type)
        {
            if (type == null)
            {
                throw new Exception("类型不能为空");
            }

            string typeFullPath = null;

            string fullName = type.FullName;

            string processorName = FilterAssemblyName(type.Assembly.FullName);

            typeFullPath = fullName + "," + processorName;

            return typeFullPath;
        }



        /// <summary>
        /// 通过类全路径创建实例
        /// </summary>
        /// <param name="classWholePath">类全路径</param>
        /// <returns></returns>
        public static object CreateInstanceByWholeUrl(string classWholePath)
        {
            return GetInstanceByCacheLogic(classWholePath);


        }

        /// <summary>
        /// 根据AssemblyFullName获取程序集名称
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <returns></returns>
        public static string FilterAssemblyName(string assemblyFullName)
        {
            string[] nameSplites = assemblyFullName.Split(',');

            return nameSplites[0];
        }

        /// <summary>
        /// 获取缓存的实现类
        /// </summary>
        /// <param name="classWholePath"></param>
        /// <returns></returns>
        private static object GetInstanceByCacheLogic(string classWholePath)
        {
            object obj = null;

            if (!_ImplementorDic.ContainsKey(classWholePath))
            {
                lock (_LockPad)
                {
                    if (!_ImplementorDic.ContainsKey(classWholePath))
                    {
                        Type objectType = GetTypeByWholeUrl(classWholePath);

                        if (objectType != null)
                        {
                            obj = Activator.CreateInstance(objectType, false);

                            _ImplementorDic.Add(classWholePath, obj);
                        }


                    }
                }
            }


            if (obj == null)
            {
                obj = _ImplementorDic[classWholePath];
            }


            return obj;
        }

        /// <summary>
        /// 通过Type创建实例
        /// </summary>
        /// <param name="objectType">类型</param>
        /// <returns></returns>
        public static object CreateInstanceByType(Type objectType)
        {
            if (objectType == null)
            {
                throw new Exception("Type can't be null if you want to use it");
            }

            string typeFullPath = GetTypeFullUrlWithProcessorName(objectType);

            return GetInstanceByCacheLogic(typeFullPath);
        }

        /// <summary>
        /// 获取类别制定名称+"."+方法名称获取制定的方法列表
        /// </summary>
        /// <param name="type">类型名称</param>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        public static MethodInfo GetMethodFromType(Type type, string methodName)
        {

            string typeFullPath = GetTypeFullUrlWithProcessorName(type);

            string methodFullPath = typeFullPath + "." + methodName;

            if (!_TypeNameSingleMethodListDic.ContainsKey(methodFullPath))
            {
                lock (_LockPad)
                {
                    if (!_TypeNameSingleMethodListDic.ContainsKey(methodFullPath))
                    {
                        MethodInfo[] methodLists = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                        MethodInfo me = null;

                        List<MethodInfo> filterdMethodList = new List<MethodInfo>();

                        foreach (MethodInfo method in methodLists)
                        {
                            if (method.Name == methodName)
                            {
                                me = method;

                                _TypeNameSingleMethodListDic.Add(methodFullPath, me);

                                break;
                            }
                        }
                    }
                }
            }

            if (_TypeNameSingleMethodListDic.ContainsKey(methodFullPath))
            {
                return _TypeNameSingleMethodListDic[methodFullPath];
            }

            return null;
        }



        /// <summary>
        /// 获取类别制定名称+"."+方法名称获取制定的方法列表
        /// </summary>
        /// <param name="type">类型名称</param>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethodsFromType(Type type, string methodName)
        {
            MethodInfo[] methodLists = null;

            string typeFullPath = GetTypeFullUrlWithProcessorName(type);

            string methodFullPath = typeFullPath + "." + methodName;

            if (!_TypeNameMethodListDic.ContainsKey(methodFullPath))
            {
                lock (_LockPad)
                {
                    if (!_TypeNameMethodListDic.ContainsKey(methodFullPath))
                    {
                        methodLists = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                        List<MethodInfo> filterdMethodList = new List<MethodInfo>();

                        foreach (MethodInfo method in methodLists)
                        {
                            if (method.Name == methodName)
                            {
                                filterdMethodList.Add(method);
                            }
                        }

                        _TypeNameMethodListDic.Add(methodFullPath, filterdMethodList.ToArray());

                    }
                }
            }

            if (methodLists == null)
            {
                try
                {
                    methodLists = _TypeNameMethodListDic[methodFullPath];
                }
                catch (Exception ex)
                {
                    Exception exNew = new Exception(typeFullPath, ex);

                    throw exNew;
                }

            }

            return methodLists;
        }

    }
}
