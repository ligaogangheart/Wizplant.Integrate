using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PR.WizPlant.Integrate.AnyShare
{
    /// <summary>
    /// 云平台票据
    /// </summary>
    public class AnyShareTicket
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户令牌
        /// </summary>
        public string TokenId { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        private DateTime loginTime = DateTime.MinValue;

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime
        {
            get { return loginTime; }
            set { loginTime = value; }
        }
        /// <summary>
        /// 超时时间,单位为秒
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth
        {
            get
            {
                return loginTime.AddSeconds(Timeout) > DateTime.Now;
            }
        }
    }
}