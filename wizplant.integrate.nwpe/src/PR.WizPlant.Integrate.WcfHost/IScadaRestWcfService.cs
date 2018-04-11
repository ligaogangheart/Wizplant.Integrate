using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PR.WizPlant.Integrate.Scada.Entities;

namespace PR.WizPlant.Integrate.WcfHost
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IScadaRestWcfService”。
    [ServiceContract]
    public interface IScadaRestWcfService
    {
        /// <summary>
        /// 获取设备下测点列表
        /// </summary>
        /// <param name="objId">设备对象Id</param>
        /// <returns>[{"TagNo":"#TagNo"},{"TagNo":"#TagNo"}]</returns>
        [OperationContract]
        string GetObjectTags(Guid objId);

        /// <summary>
        /// 根据隔离开关id列表获取对应的测点列表
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        [OperationContract]
        string GetSwitchObjectTagsList(Guid[] objIds);

        /// <summary>
        /// 根据隔离开关id列表获取对应的测点列表
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        [OperationContract]
        string GetSwitchObjectTagsListByList(List<SegregateSwitchData> switchObjList);

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <param name="isShowDetail">是否查询部件名称</param>
        /// <returns>[{"ModelName":"","Value",""},{"ModelName":"","Value",""}]</returns>
        /// <remarks>ModelName，测点对应的模型名称</remarks>
        [OperationContract]
        string GetTagModelValue(int date, string[] tags, bool isShowDetail);

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="colName">测点集合组名称</param>
        /// <param name="isShowDetail">是否查询部件名称</param>
        /// <returns>[{"ModelName":"","Value",""},{"ModelName":"","Value",""}]</returns>
        /// <remarks>ModelName，测点对应的模型名称</remarks>
        [OperationContract]
        string GetTagModelValueByColName(int date, string colName, bool isShowDetail);

        

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns>
        /// [
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
        /// },
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
        /// }
        /// ]
        /// </returns>
        [OperationContract]
        string GetTagDetailValue(int date, string[] tags, bool onlyLast);




        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="colName">测点集合组名称</param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns>
        /// [
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
        /// },
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
        /// }
        /// ]
        /// </returns>
        [OperationContract]
        string GetTagDetailValueByColName(int date, string colName, bool onlyLast);


        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="totalSeconds">总区间时间数</param>
        /// <param name="intervalSeconds">数据点间隔秒数</param>
        /// <param name="tags"></param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns>
        /// [
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
        /// },
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
        /// }
        /// ]
        /// </returns>
        [OperationContract]
        string GetTagDetailValue2(DateTime endTime, int totalSeconds, int intervalSeconds, string[] tags, bool onlyLast);


        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="totalSeconds">总区间时间数</param>
        /// <param name="intervalSeconds">数据点间隔秒数</param>
        /// <param name="colName"></param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns>
        /// [
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":1}]
        /// },
        /// {"TagNo":"","SiteName":"","Name","","Type":"YX","MessureType":"101","Count":-1,
        ///     "TimeValue":[{"Value","","Date":"20160606","Time":"200819","B":1},{"Value","","Date":"20160606","Time":"200819","B":0}]
        /// }
        /// ]
        /// </returns>
        [OperationContract]
        string GetTagDetailValueByColName2(DateTime endTime, int totalSeconds, int intervalSeconds, string colName, bool onlyLast);


        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="tags">测点集合</param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        [OperationContract]
        string GetTagListValue(int date, string[] tags);

        /// <summary>
        /// 获取隔离开关测点集合的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="tags">测点集合</param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        [OperationContract]
        string GetSwitchTagListValue(int date, List<SegregateSwitchData> switchObjList);

        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="colName">测点集合组名称</param>
        /// <returns>[{"TagNo":"","Value",""},{"TagNo":"","Value",""}]</returns>
        [OperationContract]
        string GetTagListValueByColName(int date, string colName);
    }
}
