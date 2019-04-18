using Service.Core.Helper;
using Service.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatDataService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SocketManage socketManage = new SocketManage();

                socketManage.Init();

                Console.ReadKey();
            }
            catch (Exception e)
            {
                LogHelper.Error("socket启动失败", e);
            }
        }
    }
}
