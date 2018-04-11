using Common.Logging;
using Newtonsoft.Json.Linq;
using PR.WizPlant.Integrate.AnyShare;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PR.WizPlant.Integrate.AnyShareTool
{
    public class CreatWorker
    {
        static string AnyShareFileService = System.Configuration.ConfigurationManager.AppSettings["anysharefileservice"];//云文档文件服务
        static string docid = System.Configuration.ConfigurationManager.AppSettings["DocId"];//开始创建目录的根目录docid
        static string ProjectIdList = System.Configuration.ConfigurationManager.AppSettings["ProjectIdList"];//需创建目录的项目Id集合
        static string ClassId = System.Configuration.ConfigurationManager.AppSettings["ClassId"];//目录需创建到的层级Id，创建至此层级则停止继续创建子目录
        static string SpecialClassId = System.Configuration.ConfigurationManager.AppSettings["SpecialClassId"];//需特殊处理的目录，创建到的层级Id，创建至此层级则停止继续创建子目录
        static string StopClassId = string.Empty;//当前停止创建子目录的层级（类别Id）
        static string SpecialNameList = System.Configuration.ConfigurationManager.AppSettings["SpecialNameList"];//需特殊处理的目录名称（对象的Name）集合
        static string AssociationId = System.Configuration.ConfigurationManager.AppSettings["AssociationId"];//关联关系，下级对象
        static string strConn = System.Configuration.ConfigurationManager.AppSettings["strConn"];//集成数据库连接字符串
        static string wizplantConn = System.Configuration.ConfigurationManager.AppSettings["wizplantConn"];//wizplant数据库连接字符串
        
        static ILog logger = LogManager.GetLogger<CreatWorker>();


        static Dictionary<string, string> equipmentClassCache = null;
        /// <summary>
        /// 设备类型集合（除设备类型本身，仍有其他类别属于设备类型）
        /// </summary>
        public static Dictionary<string, string> EquipmentClassCache
        {
            get
            {
                if (equipmentClassCache == null)
                {
                    equipmentClassCache = new Dictionary<string, string>();
                    string selectSql = "SELECT [Id],[Name] FROM [WizPlant_XMH].[dbo].[PRW_MetaClass] WHERE [ParentId] ='021BD91A-74F6-4BE9-BE2F-A4EA012DDDF5'";
                    using (var reader = SqlHelper.ExecuteDataReader(SqlHelper.DefaultConnectionString, CommandType.Text, selectSql))
                    {
                        while (reader.Read())
                        {
                            equipmentClassCache[reader.GetGuid(0).ToString().ToLower()] = reader.GetString(1);
                        }
                    }
                    equipmentClassCache.Add("021bd91a-74f6-4be9-be2f-a4ea012dddf5", "Equipment");//添加设备类别本身
                }
                return equipmentClassCache;
            }
        }


        static Dictionary<string, string> specialObjCache = null;
        /// <summary>
        /// 需特殊处理的对象（根据需处理父对象找到所有对象）
        /// </summary>
        public static Dictionary<string, string> SpecialObjCache
        {
            get
            {
                if (specialObjCache == null)
                {
                    specialObjCache = new Dictionary<string, string>();
                    Dictionary<string, string> specialObjIdDic = new Dictionary<string, string>();//保存特殊需处理的对象Id，项目ProjectId
                    List<string> specialNameList = SpecialNameList.Split(',').ToList();
                    string specialNameListStr = string.Empty;
                    if (specialNameList.Count > 0) 
                    {
                        specialNameListStr = "'" + string.Join("','", specialNameList) + "'";
                    }
                    string selectSql = string.Format(@"select distinct(Id),ProjectId from PRW_Object(nolock) where Name in ({0}) and Status = 0 ", specialNameListStr);
                    using (var reader = SqlHelper.ExecuteDataReader(wizplantConn, selectSql, null))
                    {
                        while (reader.Read())
                        {
                            specialObjIdDic.Add(reader["Id"].ToString(), reader["ProjectId"].ToString());
                        }
                    }

                    foreach (var specialObjId in specialObjIdDic)
                    {
                        AddSpecialObj(specialObjId.Key, specialObjId.Value);
                    }
                }
                return specialObjCache;
            }
        }
        
        public void Run()
        {
            //initCache();
            List<string> projectIdList = ProjectIdList.Split(',').ToList();
            writeInfo("创建目录开始......");
            foreach (var projectId in projectIdList)
            {
                try 
                {
                    writeInfo(string.Format("开始创建目录项目projectId:{0}", projectId));
                    createObject(projectId,"ADC0A54C-5A1A-43A5-981B-A4EA012DDE2F", docid, projectId);
                }
                catch (Exception ex)
                {
                    writeInfo(ex.Message.ToString());
                }
            }
            writeInfo("创建目录结束");
        }

        void writeInfo(string msg)
        {
           Console.WriteLine("{0:HH:mm:ss.ttt} [{1}] {2}",DateTime.Now, "INFO", msg);
           logger.Info(msg);
        }


        /// <summary>
        /// 创建对象目录及子目录（递归）
        /// </summary>
        /// <param name="objId">当前创建目录的对象Id</param>
        /// <param name="classId">当前创建目录的对象类别</param>
        /// <param name="docid">父目录gns路径，在此目录下创建当前目录</param>
        /// <param name="projectId">项目Id</param>
        void createObject(string objId, string classId, string docid, string projectId)
        {
            //创建对象目录
            string gns = createDir(objId, docid);
            if (gns == null)
            {
                writeInfo("gns路径为null，此项目目录创建已跳出");
                return;
            }   
            //if (objId == "FB58010E-0FFC-4D26-B3BB-1ABACFD74685")
            //{
            //    //父级
            //    string a = "";
            //}
            //if (objId == "938BBC81-FB3B-432A-9EAD-432A4F1707FE")
            //{
            //    //子级
            //    string a = "";
            //}
            string stopClassId = ClassId;//创建目录时需创建到的层级Id
            if (SpecialObjCache.Where(p => p.Value == projectId.ToLower().ToString()).ToDictionary(p => p.Key,p=>p.Value).Keys.Contains(objId.ToLower().ToString()))
            {
                stopClassId = SpecialClassId;//需特殊处理目录，创建到此层级Id，停止继续创建子目录
            }
            if (stopClassId.ToUpper() =="021BD91A-74F6-4BE9-BE2F-A4EA012DDDF5")//若为设备类型，则需判断设备下的所有类型
            {
                if (EquipmentClassCache.Keys.Contains(classId.ToLower()))
                {
                    return;//创建到设备层级，停止继续创建子目录
                }
            }
            else if (classId.ToLower() == stopClassId.ToLower())
            {
                return;//创建到配置层级，停止继续创建子目录
            }
            
            //创建子对象目录
            string selectSql = string.Format(@"select b.* from PRW_Association_Object(nolock) a inner join PRW_Object(nolock) b on a.SourceObjectId=b.Id
 where a.TargetObjectId='{0}' and a.AssociationId='{1}'
 and b.Status = 0 and ProjectId = '{2}'", objId, AssociationId, projectId);
            using (var reader = SqlHelper.ExecuteDataReader(wizplantConn, selectSql, null))
            {
                while (reader.Read())
                {
                    objId = reader["Id"].ToString();
                    classId = reader["ClassId"].ToString();
                    createObject(objId, classId, gns, projectId);
                }
            }

            //Dictionary<string, string> idNames = new Dictionary<string, string>();
            //foreach(var item in idNames)
            //{
            //    createObject(item.Key, item.Value);
            //}

        }

        /// <summary>
        /// 创建对象目录
        /// </summary>
        /// <param name="objId">当前对象Id</param>
        /// <param name="docid">父目录的gns路径，在此目录下创建当前目录</param>
        string createDir(string objId, string docid)
        {
            if (!AnyShareProxy.MapCache.ContainsKey(objId.ToLower()))//若未创建则继续创建
            {
                if (AnyShareProxy.Ticket.IsAuth)//已经登录则创建目录
                {
                    string sql = "select Id,Name,Context,Revision from PRW_Object(nolock) where Id =@Id";
                    SqlParameter[] parms = new SqlParameter[] { new SqlParameter("@Id", objId) };
                    string Name = string.Empty;
                    string Context = string.Empty;
                    string Revision = string.Empty;
                    using (var reader = SqlHelper.ExecuteDataReader(wizplantConn, sql, parms))
                    {
                        while (reader.Read())
                        {
                            reader["Id"].ToString();
                            Name = reader["Name"].ToString();
                            Context = reader["Context"].ToString();
                            Revision = reader["Revision"].ToString();
                        }
                    }

                    //创建云平台目录
                    string url = string.Format("{0}/dir?method=create&userid={1}&tokenid={2}", AnyShareFileService, AnyShareProxy.Ticket.UserId, AnyShareProxy.Ticket.TokenId);
                    string json = "{\"docid\":\"" + docid + "\", \"name\":\"" + Name + "\", \"ondup\": \"1\"}";
                    try
                    {
                        string res = AnyShareProxy.HttpPost(url, json);
                        JObject obj = JObject.Parse(res);
                        if (obj["docid"] != null)//判断是否为正确返回值
                        {
                            string insertSql = string.Format("insert into PRW_Inte_AnyShare_DirMap values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", Guid.NewGuid(), Name, Context, Revision, obj["docid"].ToString(), Name, objId);
                            int count = SqlHelper.ExecuteNonQuery(strConn, insertSql, null);

                            Console.WriteLine("{0:HH:mm:ss.ttt} [{1}] {2}", DateTime.Now, "INFO", string.Format("目录名：{0}，已创建", Name));
                            AnyShareProxy.MapCache[objId.ToLower()] = obj["docid"].ToString();//已创建添加到缓存，避免数据库有重复数据报错
                            return obj["docid"].ToString();
                        }
                        else
                        {
                            if (obj["errmsg"] != null)
                            {
                                writeInfo(obj["errmsg"].ToString());
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("创建云平台目录报错：url:" + url + ",json:" + json);
                    }
                    
                }
                // create
                // insertmap db
            }
            else//已创建返回gns路径
            {
                return AnyShareProxy.MapCache[objId.ToLower()].ToString();
            }
            return null;
        }

        /// <summary>
        /// 添加特殊处理对象
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="projectId"></param>
        static void AddSpecialObj(string objId, string projectId)
        {
            specialObjCache[objId.ToLower().ToString()] = projectId.ToLower().ToString();
            //查找子对象
            string selectSql = string.Format(@"select b.* from PRW_Association_Object(nolock) a inner join PRW_Object(nolock) b on a.SourceObjectId=b.Id
 where a.TargetObjectId='{0}' and a.AssociationId='{1}'
 and b.status = 0 and ProjectId = '{2}'", objId, AssociationId, projectId);
            using (var reader = SqlHelper.ExecuteDataReader(wizplantConn, selectSql, null))
            {
                while (reader.Read())
                {
                    objId = reader["Id"].ToString();
                    if (!specialObjCache.ContainsKey(objId.ToLower()))
                    {
                        AddSpecialObj(objId, projectId);
                    }
                }
            }
        }
        //void initCache()
        //{
        //    string selectSql = "Select ObjectId,DirId from PRW_Inte_AnyShare_DirMap where objectId is not null";
        //    using(var reader = SqlHelper.ExecuteDataReader(strConn,selectSql,null))
        //    {
        //        while(reader.Read())
        //        {
        //            mapCache[reader[0].ToString().ToLower()] = reader.GetString(1);
        //        }
        //    }
        //}
    }
}
