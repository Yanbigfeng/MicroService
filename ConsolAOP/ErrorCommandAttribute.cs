
using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsolAOP
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    class ErrorCommandAttribute : AbstractInterceptorAttribute
    {
        string _mess;
        public ErrorCommandAttribute(string methon)
        {
            _mess = methon;
        }
        /// <summary>
        /// 每个被拦截的方法中执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("AddAsync方法开始前");
                await next(context); // 执行被拦截的方法
            }
            catch (Exception)
            {
                Console.WriteLine("AddAsync方法出错");
                //重复调用示例
                ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();
                using (IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build())
                {
                    var baseClass = context.Proxy;
                    foreach (var item in baseClass.GetType().GetMethods())
                    {
                        if (item.Name.ToString() == _mess)
                        {
                            MethodInfo methodinfo = baseClass.GetType().GetMethod(_mess);
                            methodinfo.Invoke(baseClass, null);
                        }
                    }
                }

                throw;
            }
            finally
            {
                Console.WriteLine("AddAsync方法结束");
            }
        }
    }
}
