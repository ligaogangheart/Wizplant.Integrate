using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace PR.WizPlant.Integrate.WcfHost.utilities
{
    public static class ScadaHelper
    {
        static string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        /// <summary>
        /// 获取设备下测点列表
        /// </summary>
        /// <param name="objId">设备对象Id</param>
        /// <returns>测点Id列表，以|隔开</returns>
        public static string GetObjectTags(string objId)
        {
            string sql = @"select TagId from PRW_Inte_SCADA_Map(nolock)
                            where [ObjectId]=@p_objId";

            string result = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, sql, new SqlParameter("p_objId", objId)))
            {
                StringBuilder sb = new StringBuilder();
                while (reader.Read())
                {
                    sb.AppendFormat("{0}|", reader.GetString(0));
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }

                sb.Insert(0, "{\"RealData\":\"").Append("\"}");

                result = sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// 获取测点集合的对应的模型值
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="tags">测点Id数组</param>
        /// <param name="isShowDetail">是否显示部件</param>
        /// <returns></returns>
        public static string GetTagModelValue(int date, string [] tags,bool isShowDetail)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }

            var ids = string.Join("','", tags);

            string sql = string.Format(@"select c.name,c.name2,a.MeasureValue from PRW_Inte_SCADA_EFileData a inner join
(select MAX(CreateTime) CreateTime from PRW_Inte_SCADA_EFileData where CreateDate={0}) b on a.CreateTime=b.CreateTime
inner join PRW_Inte_SCADA_Map c on a.TagId=c.TagId
 and a.CreateDate={0} 
 and a.TagId in ('{1}')" ,date,ids);



            StringBuilder sb = new StringBuilder();

            using (var reader = SqlHelper.ExecuteDataReader(connectionString, sql))
            {               
                string name,name1;
                while (reader.Read())
                {
                    sb.Append("{");

                    name = reader.GetString(0);

                    if (isShowDetail)
                    {
                        if (reader.IsDBNull(1))
                        {
                            name1 = reader.GetString(1);
                            if (!string.IsNullOrEmpty(name1))
                            {
                                name = name1;
                            }
                        }
                    }
                    
                    sb.AppendFormat("\"modelName\":\"{0}\",",name);
                    sb.AppendFormat("\"value\":\"{0}\"", reader.GetString(2));
                    sb.Append("},");
                }
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Insert(0, "[").Append("]");

            return sb.ToString();
        }


        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <param name="onlyLast">是否只取最新记录</param>
        /// <returns></returns>
        public static string GetTagDetailValue(int date, string[] tags, bool onlyLast)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }

            var ids = string.Join("','", tags);

            string sql = null;
            if (onlyLast)
            {
                sql = string.Format(@"select a.*,c.TagType from PRW_Inte_SCADA_EFileData a inner join
(select MAX(CreateTime) CreateTime from PRW_Inte_SCADA_EFileData where CreateDate={0}) b on a.CreateTime=b.CreateTime
inner join PRW_Inte_SCADA_Map c on a.TagId=c.TagId
 and a.CreateDate={0} 
 and a.TagId in ('{1}')
order by ( case c.TagType 
when '开关状态' then 1 
when 'A相电流' then 2
when 'B相电流' then 3
when 'C相电流' then 4
when 'A相电压' then 5
when 'B相电压' then 6
when 'C相电压' then 7
when '有功' then 8
when '无功' then 9
else 100
end
) asc", date, ids);
            }
            else
            {
                sql = string.Format(@"select a.*,c.TagType from PRW_Inte_SCADA_EFileData a 
inner join PRW_Inte_SCADA_Map c on a.TagId=c.TagId
 and a.CreateDate={0} 
 and a.TagId in ('{1}') order by a.CreateTime asc", date, ids);
            }



            StringBuilder sb = new StringBuilder();

            using (var reader = SqlHelper.ExecuteDataReader(connectionString, sql))
            {
                while (reader.Read())
                {
                    sb.Append("{");
                  
                    sb.AppendFormat("\"Id\":\"{0}\",", reader["Id"].ToString());
                    sb.AppendFormat("\"CreateDate\":\"{0}\",", reader["CreateDate"].ToString());
                    sb.AppendFormat("\"CreateTime\":\"{0}\",", reader["CreateTime"].ToString());
                    sb.AppendFormat("\"SiteName\":\"{0}\",", reader["SiteName"].ToString());
                    sb.AppendFormat("\"TagId\":\"{0}\",", reader["TagId"].ToString());
                    sb.AppendFormat("\"TagName\":\"{0}\",", reader["TagName"].ToString());
                    sb.AppendFormat("\"MeasureValue\":\"{0}\",", reader["MeasureValue"].ToString());
                    sb.AppendFormat("\"Status\":\"{0}\",", Convert.ToInt32(reader["Status"]));
                    sb.AppendFormat("\"Type\":\"{0}\",", reader["Type"].ToString());
                    sb.AppendFormat("\"TagType\":\"{0}\"", reader["TagType"].ToString());
                    sb.Append("},");                  
                }
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Insert(0, "{\"RealData\":[").Append("]}");

            return sb.ToString();
        }


        /// <summary>
        /// 获取测点集合的模型值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string GetTagListValue(int date, string[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                return "[]";
            }

            var ids = string.Join("','", tags);

            string sql = string.Format(@"select a.TagId,a.MeasureValue from PRW_Inte_SCADA_EFileData a inner join
(select MAX(CreateTime) CreateTime from PRW_Inte_SCADA_EFileData where CreateDate={0}) b on a.CreateTime=b.CreateTime
 and a.CreateDate={0}
 and a.TagId in ('{1}')", date, ids);



            StringBuilder sb = new StringBuilder();

            using (var reader = SqlHelper.ExecuteDataReader(connectionString, sql))
            {
                while (reader.Read())
                {
                    sb.Append("{");

                    sb.AppendFormat("\"TagId\":\"{0}\",", reader.GetString(0));
                    sb.AppendFormat("\"value\":\"{0}\"", reader.GetString(1));
                    sb.Append("},");
                }
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Insert(0, "[").Append("]");

            return sb.ToString();
        }

    }
}