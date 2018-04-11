using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Humiture.Models
{
    /// <summary>
    /// 接口基础数据
    /// </summary>
public    class ObjectData<T>
    {
        /// <summary>
        /// 0       获取成功 10001   密码错误 10002   用户名无效--获取令牌服务
        /// 0       获取成功 10001   无数据 ,null数据  10002   无权限  10003   token过期 10004   无效的token id--获取温湿度数据服务
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 描述说明
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }
    }

    public class SensorObjectData: ObjectData<List<HumitureData>>
    {
        public int count { get; set; }
        public double max_page { get; set; }
        public int current_page { get; set; }

    }
}
