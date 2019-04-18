using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core.Helper
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LogHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Info(object msg)
        {
            log.Info(msg);
        }

        public static void Debug(object msg)
        {
            log.Debug(msg);
        }

        public static void Debug(object msg, Exception e)
        {
            log.Debug(msg, e);
        }

        public static void Error(object msg, Exception e)
        {
            log.Error(msg, e);
        }
    }
}