using Common.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada.Dao
{
    public static class MongoDbHelper
    {
        static ILog logger = LogManager.GetLogger("MongoDbHelper");

        private static IMongoDatabase scadaDB = null;
        public static IMongoDatabase ScadaDB
        {
            get
            {
                return scadaDB;
            }
        }

        static MongoDbHelper()
        {
            initDB();
        }
        static void initDB()
        {
            logger.DebugFormat("initDB()");
            string dbServer = System.Configuration.ConfigurationManager.AppSettings["mongodbServer"];//"mongodb://localhost:27017";
            logger.DebugFormat("dbServer = {0}", dbServer);
            string dbName = System.Configuration.ConfigurationManager.AppSettings["mongodbDB"];//"scadaDB";
            logger.DebugFormat("dbName = {0}", dbName);

            try
            {
                scadaDB = new MongoClient(dbServer).GetDatabase(dbName);
                logger.DebugFormat("GetDatabase {0} success", dbName);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("GetDatabase {0} Error:{1}", dbName, ex.Message, ex);
                throw;
            }


            //if (!scadaDB.Client.Settings.IsFrozen)
            //{
            //    logger.DebugFormat("设置 ReadPreference.SecondaryPreferred ");
            //    // 读写分享设置，优先从副本读取
            //    scadaDB.Settings.ReadPreference = ReadPreference.SecondaryPreferred;
            //}
           
        }

    }
}
