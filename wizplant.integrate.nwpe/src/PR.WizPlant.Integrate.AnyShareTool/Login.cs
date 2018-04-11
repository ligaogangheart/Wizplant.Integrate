using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PR.WizPlant.Integrate.AnyShareTool
{
    public class Login
    {
        static string AnyShareUserName = System.Configuration.ConfigurationManager.AppSettings["AnyShareUserName"];//云平台登录用户名
        static string AnyShareUserPwd = System.Configuration.ConfigurationManager.AppSettings["AnyShareUserPwd"];//云平台账户密码
        static string AnyShareIP = System.Configuration.ConfigurationManager.AppSettings["AnyShareIP"];//云平台服务器IP地址

        /// <summary>
        /// 超时时间
        /// </summary>
        static double timeout = 3600;

        static public UserInfo CurrentUser = new UserInfo();

        /// <summary>
        /// 登录信息
        /// </summary>
        public class UserInfo
        {
            public string UserId { get; set; }
            public string TokenId { get; set; }
            public DateTime LastLogin { get; set; }
        }

        public void UserLogin(out string userid, out string tokenid)
        {
            double interval = DateTime.Now.Subtract(CurrentUser.LastLogin).TotalSeconds;

            if (interval < timeout)
            {
                userid = CurrentUser.UserId;
                tokenid = CurrentUser.TokenId;
            }
            else
            {
                string url = "http://" + AnyShareIP + ":9998/v1/auth1?method=getnew";
                string passwordBase64 = RSA.RSAEncrypt(AnyShareUserPwd);
                string postDataStr = "{\"account\":\"" + AnyShareUserName + "\", \"password\":\"" + passwordBase64 + "\"}";
                string retstring = Http.HttpPost(url, postDataStr);
                JObject obj = JObject.Parse(retstring);
                CurrentUser.UserId = obj["userid"].ToString();
                CurrentUser.TokenId = obj["tokenid"].ToString();
                CurrentUser.LastLogin = DateTime.Now;
                timeout = obj["expires"].Value<int>();

                userid = obj["userid"].ToString();
                tokenid = obj["tokenid"].ToString();
            }
        }
    }
}
