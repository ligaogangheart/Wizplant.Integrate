using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PR.WizPlant.Integrate.AnyShare
{
    /// <summary>
    /// 云平台Rest服务
    /// </summary>
    [ServiceContract]
    public interface IAnyShareRestWcfService
    {
        /// <summary>
        /// 根据对象Id获取对象关联云平台文件列表，分页
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [OperationContract(Name="GetPagedAssociationDocs")]
        string GetFileList(string objId, int pageNo, int pageSize, int totalRecords);

        /// <summary>
        /// 根据云平台目录Id获取目录下文件列表，分页
        /// </summary>
        /// <param name="dirId">目录对象Id</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [OperationContract(Name="GetPagedDocsByDirId")]
        string GetFileListByCloudDirId(string dirId, int pageNo, int pageSize, int totalRecords);

        /// <summary>
        /// 根据对象Id获取对象关联云平台文件和子目录列表
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// <returns>{"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}],"dirs":[{"locationId":"gns","dirName":"xyz","length":12345},{"locationId":"gns","dirName":"xyz","length":12345}]}</returns>
        [OperationContract(Name="GetAssociationSubList")]
        string GetSubList(string objId);

        /// <summary>
        /// 根据云平台目录Id获取目录下文件和子目录列表
        /// </summary>
        /// <param name="dirId">目录对象Id</param>       
        /// <returns>{"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}],"dirs":[{"locationId":"gns","dirName":"xyz","length":12345},{"locationId":"gns","dirName":"xyz","length":12345}]}</returns>
        [OperationContract(Name="GetSubListByDirId")]
        string GetSubListByCloudDirId(string dirId);

        /// <summary>
        /// 根据文件名关键字在指定对象的云文档目录中查找文件
        /// </summary>
        /// <param name="objId">对象Id</param>
        /// /// <param name="key">要查询的文件名，模糊匹配，不含后缀名</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [OperationContract(Name="SearchPagedFileList")]
        string SearchFileList(string objId,string key, int pageNo, int pageSize, int totalRecords);

        /// <summary>
        /// 根据文件名关键字在云文档指定目录中查找文件
        /// </summary>
        /// <param name="dirId">目录对象Id</param>
        /// <param name="key">要查询的文件名，模糊匹配，不含后缀名</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="totalRecords">总记录数，如果为-1表示需要计算总记录数，如果为大于-1的整数则表示为总记录数，则服务不再计算总记录数</param>
        /// <returns>{"total":1000,"pages":50,"docs":[{"locationId":"gns","fileName":"xyz","length":12345},{"locationId":"gns","fileName":"xyz","length":12345}]}</returns>
        [OperationContract(Name="SearchPagedFileListByCloudDirId")]
        string SearchFileListByCloudDirId(string dirId, string key, int pageNo, int pageSize, int totalRecords);
    }
}
