using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;

namespace ClientConsul
{
    class Program
    {
        static List<string> Urls = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("开始输出当前所有服务地址");
            Catalog_Nodes().GetAwaiter().GetResult();
            //Console.WriteLine(HelloConsul().GetAwaiter().GetResult());
            Console.WriteLine("开始随机请求一个地址服务地址");
            int index = new Random().Next(Urls.Count);
            string url = Urls[index];
            string param = "";//这里是开始位置
            param += "{";
            param += "\"" + "id" + "\":\"" + 5 + "\",";
            param = param.TrimEnd(',');
            param += "}";
            Console.WriteLine("请求的随机地址：" + url);
            string result = HttpClientHelpClass.PostResponse(url, param, out string statusCode);
            Console.WriteLine("返回状态：" + statusCode);
            Console.WriteLine("返回结果：" + result);
            Console.ReadLine();
        }
        public static async Task Catalog_Nodes()
        {
            var client = new ConsulClient();
            var nodeList = await client.Agent.Services();
            var url = nodeList.Response.Values;

            foreach (var item in url)
            {
                string Address = item.Address;
                int port = item.Port;
                string name = item.Service;
                Console.WriteLine($"地址：{Address}:{port},name：{name}");
                Urls.Add($"http://{Address}:{port}/api/Test");
            }
        }
    }
}
