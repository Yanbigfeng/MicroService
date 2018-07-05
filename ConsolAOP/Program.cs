using AspectCore.DynamicProxy;
using System;

namespace ConsolAOP
{
  public  class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始程序");
            //User user = new User();
            //user.AddAsync();
            ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            using (IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build())
            {
                User p = proxyGenerator.CreateClassProxy<User>();
                p.AddAsync();
                //p.Error1();
            }

            Console.ReadLine();
        }
    }
}
