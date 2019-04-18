using Newtonsoft.Json;
using Service.Core;
using Service.DataManage.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DataManage.Function
{
    /// <summary>
    /// 红包数据操作类。Redis中 list标识：‘1’为用户链表；
    /// </summary>
    public class RedDataManage
    {
        private RedisHelper helper = new RedisHelper();

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="value"></param>
        public void SaveUser(string value)
        {
            var userList = helper.GetList<UserInfo>("1");
            var info = JsonConvert.DeserializeObject<UserInfo>(value);

            if (userList.Where(x => x.Id == info.Id).Count() == 0)
            {
                info.startTime = DateTime.Now;
                helper.AddEntityToList("1", info);
            }
        }

        /// <summary>
        /// 更新中奖信息
        /// </summary>
        /// <param name="id"></param>
        public void UpdateLuck(string id)
        {
            var userList = helper.GetList<UserInfo>("1");
            var info = userList.Where(x => x.Id == id).FirstOrDefault();

            var index = userList.ToList().FindIndex(x => x.Id == id);
            if (info != null)
            {
                helper.RemoveEntityFromList<UserInfo>("1", index);
                info.ISLuck = 1;
                info.LuckTime = DateTime.Now;
                helper.AddEntityToList("1", info);
            }
        }

        /// <summary>
        /// 根据抽奖时间展示人员信息
        /// </summary>
        /// <returns></returns>
        public string GetLuck()
        {
            var userList = helper.GetList<UserInfo>("1");

            var res = userList.OrderByDescending(x => x.LuckTime).ToList();
            return JsonConvert.SerializeObject(res);
        }

        /// <summary>
        /// 判断是否有投票机会
        /// 开发环境暂时取消判断。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string HasJoined(string id)
        {
            var userList = helper.GetList<UserInfo>("1");
            var info = userList.Where(x => x.Id == id).FirstOrDefault();
            string res = "0";
            if (true)//info.LuckTime.AddDays(1) < DateTime.Now
            {
                res = "1";//未投过

                #region 重置投票时间

                var index = userList.ToList().FindIndex(x => x.Id == id);
                if (info != null)
                {
                    helper.RemoveEntityFromList<UserInfo>("1", index);
                    info.LuckTime = DateTime.Now;
                    helper.AddEntityToList("1", info);
                }

                #endregion 重置投票时间
            }
            return JsonConvert.SerializeObject(res);
        }
    }
}