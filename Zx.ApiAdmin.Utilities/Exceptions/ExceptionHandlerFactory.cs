namespace Zx.ApiAdmin.Utilities.Exceptions
{
    /// <summary>
    /// 异常工厂类
    /// </summary>
    public class ExceptionHandlerFactory
    {
        #region Static Fileds

        /// <summary>
        /// 处理器获取时的锁定
        /// </summary>
        static object _Lockpad = new object();

        /// <summary>
        /// 默认异常处理器
        /// </summary>
        static DefaultExceptionHandler _currentDefaultProcessor = null;

        /// <summary>
        /// 异常处理器
        /// </summary>
        static IExceptionHandler _ExceptionHandler = null;

        #endregion

        #region CallMethod

        /// <summary>
        /// 获取默认处理器
        /// </summary>
        /// <returns></returns>
        public static DefaultExceptionHandler GetExceptionProcessor()
        {
            return GetDefaultExceptionProcessor();
        }

        /// <summary>
        /// 通过反射获取异常处理器
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static IExceptionHandler GetCurrentExceptionHandler(string handlerType)
        {
            if (_ExceptionHandler == null)
            {
                lock (_Lockpad)
                {
                    if (_ExceptionHandler == null)
                    {
                        try
                        {
                            IExceptionHandler handler = (IExceptionHandler)ReflectHelper.CreateInstanceByWholeUrl(handlerType);

                            _ExceptionHandler = handler;

                        }
                        catch
                        {

                        }

                    }
                }
            }


            return _ExceptionHandler;
        }


        /// <summary>
        /// 获取默认处理器
        /// </summary>
        /// <returns></returns>
        public static DefaultExceptionHandler GetDefaultExceptionProcessor()
        {
            if (_currentDefaultProcessor == null)
            {
                lock (_Lockpad)
                {
                    if (_currentDefaultProcessor == null)
                    {
                        _currentDefaultProcessor = new DefaultExceptionHandler();

                    }

                }
            }

            return _currentDefaultProcessor;
        }


        #endregion
    }
}
