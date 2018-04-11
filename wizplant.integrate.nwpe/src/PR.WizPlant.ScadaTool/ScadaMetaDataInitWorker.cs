using Common.Logging;
using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada;
using PR.WizPlant.Integrate.Scada.Entities;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class ScadaMetaDataInitWorker:IWorker
    {
        #region Service
        /// <summary>
        /// Scada服务
        /// </summary>
        MongDbScadaServiceBase<ScadaObjectMap> scadaService = null;
        #endregion

        string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        ILog logger = LogManager.GetLogger<TcpGetMetaDataWorker>();

        public ScadaMetaDataInitWorker()
        {
            scadaService = new MongDbScadaMapService();
        }


        public void Run()
        {
            initAll();
        }

        void initSites(Dictionary<string, string> sites)
        {
            String selectSql = @"SELECT 
    [SiteName]
      ,[TagNo]
      ,[TagName]
      ,[TagType]
      ,[TagDesc]
      ,[Name]
      ,[Name2]
      ,[Context]
      ,[Revision]
      ,[MessureType]
      ,[ObjectId] FROM [PRW_Inte_SCADA_Map] where Context in @pContexts";

            

            List<ScadaObjectMap> list = new List<ScadaObjectMap>();
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, selectSql, new SqlParameter("pContexts", sites.Values.ToArray())))
            {
                ScadaObjectMap item = null;
                while (reader.Read())
                {
                    item = new ScadaObjectMap();
                    list.Add(item);
                    item.SiteName = reader.GetString(0);
                    item.TagNo = reader.GetString(1);
                    item.TagName = reader.GetString(2);
                    item.Type = reader.GetString(3) == "YX" ? ScadaDataType.YX : ScadaDataType.YC;
                    item.TagDesc = reader.GetString(4);
                    item.Name = reader.GetString(5);
                    item.Name2 = reader.GetString(6);
                    item.Context = reader.GetString(7);
                    item.Revision = reader.GetString(8);
                    item.MessureType = reader.GetInt32(9);
                    item.ObjectId = reader.IsDBNull(10) ? null : reader.GetGuid(10).ToString();
                }
            }


            if (list.Count > 0)
            {
                var table = scadaService.GetTable();
                var filter = Builders<ScadaObjectMap>.Filter.In(p => p.SiteName, sites.Keys.ToArray());
                table.DeleteManyAsync(filter).Wait();
                table.InsertManyAsync(list).Wait();
            }
        }


        void initAll()
        {
            String selectSql = @"SELECT 
    [SiteName]
      ,[TagNo]
      ,[TagName]
      ,[TagType]
      ,[TagDesc]
      ,[Name]
      ,[Name2]
      ,[Context]
      ,[Revision]
      ,[MessureType]
      ,[ObjectId] FROM [PRW_Inte_SCADA_Map]";


            List<ScadaObjectMap> list = new List<ScadaObjectMap>();
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, selectSql, null))
            {
                ScadaObjectMap item = null;
                while (reader.Read())
                {
                    item = new ScadaObjectMap();
                    list.Add(item);
                    item.SiteName = reader.GetString(0);
                    item.TagNo = reader.GetString(1);
                    item.TagName = reader.GetString(2);
                    item.Type = reader.GetString(3) == "YX" ? ScadaDataType.YX : ScadaDataType.YC;
                    item.TagDesc = reader.IsDBNull(4) ? null : reader.GetString(4);
                    item.Name = reader.IsDBNull(5) ? null : reader.GetString(5);
                    item.Name2 = reader.IsDBNull(6) ? null : reader.GetString(6);
                    item.Context = reader.IsDBNull(7) ? null : reader.GetString(7);
                    item.Revision = reader.IsDBNull(8) ? null : reader.GetString(8);
                    item.MessureType = reader.GetInt32(9);
                    item.ObjectId = reader.IsDBNull(10) ? null : reader.GetGuid(10).ToString();
                }
            }


            if (list.Count > 0)
            {
                var table = scadaService.GetTable();
                var filter = Builders<ScadaObjectMap>.Filter.Empty;
                var deleted = table.DeleteManyAsync(filter).Result.DeletedCount;
                Console.WriteLine("删除了{0}条旧数据", deleted);
                logger.DebugFormat("从ScadaObjectMap中删除了{0}条数据", deleted);
                table.InsertManyAsync(list).Wait();
                Console.WriteLine("新增了{0}条数据", list.Count);
                logger.DebugFormat("新增ScadaObjectMap中{0}条数据", list.Count);
            }
        }

        
    }
}
