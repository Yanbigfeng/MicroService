using System;
using Polly;

namespace PollyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("程序开始!");

            //Downgrade();//降级

            //Retry();//重试机制

            //Fusing();//熔断机制

            RetryDowngrade();//重试+降级

            Console.ReadLine();
        }


        #region 降级
        public static void Downgrade()
        {
            //降级处理程序
            ISyncPolicy policy = Policy.Handle<ArgumentException>()
            .Fallback(() =>
            {
                Console.WriteLine("降级成功");
            });
            //运行程序
            policy.Execute(() =>
            {
                Console.WriteLine("开始任务");

                throw new ArgumentException("");

                Console.WriteLine("结束任务");
            });
        }
        #endregion

        #region 重试机制
        public static void Retry()
        {
            //配置重试次数
            ISyncPolicy policy = Policy.Handle<Exception>().Retry(3);

            try
            {
                policy.Execute(() =>
                {
                    Console.WriteLine("任务开始");

                    throw new Exception("出错了");

                    Console.WriteLine("任务结束");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常结果 : " + ex.Message);
            }
        }
        #endregion

        #region 熔断机制
        public static void Fusing()
        {
           
            Action<Exception, TimeSpan> onBreak = (exception, timespan) =>
            {
                Console.WriteLine("1");
            };
            Action onReset = () =>
            {
                Console.WriteLine("2");
            };
            ISyncPolicy policy = Policy.Handle<Exception>().CircuitBreaker(3, TimeSpan.FromSeconds(20), onBreak, onReset);
            while (true)
            {
                try
                {

                    policy.Execute(() =>
                        {
                            Console.WriteLine("任务开始");

                            throw new Exception("出错了");

                            Console.WriteLine("任务结束");
                        });

                }
                catch (Exception ex)
                {
                    Console.WriteLine("---------------异常结果-------------- : " + ex.Message + "时间:" + DateTime.Now);
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
        #endregion

        #region 重试+降级
        public static void RetryDowngrade()
        {
            try
            {
                //降级处理程序
                ISyncPolicy policy = Policy.Handle<Exception>()
                .Fallback(() =>
                {
                    Console.WriteLine("降级成功");
                });
                //配置重试次数
                ISyncPolicy policy2 = Policy.Handle<Exception>().Retry(3, (exception, retryCount, context) =>
                             {
                                 Console.WriteLine(retryCount);

                             });
                //合并
                ISyncPolicy mainPolicy = Policy.Wrap(policy, policy2);
                mainPolicy.Execute(() =>
                {
                    Console.WriteLine("任务开始");

                    throw new Exception("出错了");

                    Console.WriteLine("任务结束");
                });
            }
            catch (Exception ex)
            {

                Console.WriteLine("异常结果 : " + ex.Message);
            }
        }
        #endregion

    }
}
