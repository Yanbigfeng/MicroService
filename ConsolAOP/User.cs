using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsolAOP
{
    public class User
    {
        [ErrorCommand("Error1")]
        public virtual async Task<int> AddAsync()
        {
            Console.WriteLine("开始方法：AddAsync");
            throw new Exception("出错了");
            return 0;
        }
        [ErrorCommand("Error2")]
        public virtual async Task<int> Error1()
        {
            Console.WriteLine("开始方法：Error1");
            throw new Exception("Error1:出错了");
            return 1;
        }
        public async Task<int> Error2()
        {
            Console.WriteLine("开始方法：Error2");
            return 1;
        }
    }
}
