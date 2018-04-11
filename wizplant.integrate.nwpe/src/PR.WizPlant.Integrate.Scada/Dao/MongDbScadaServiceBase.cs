using Common.Logging;
using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada.Dao;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbScadaServiceBase<T>:IScadaService<T>
    {
        protected UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };
        /// <summary>
        /// 时标起始时间
        /// </summary>
        public static readonly DateTime BeginTime = new DateTime(1970, 1, 1);
        private static IMongoDatabase scadaDB = MongoDbHelper.ScadaDB;
        protected static IMongoDatabase ScadaDB
        {
            get
            {
               
                return scadaDB;
            }
        }

        //static void initDB()
        //{
        //    logger.DebugFormat("initDB()");
        //    string dbServer = System.Configuration.ConfigurationManager.AppSettings["mongodbServer"];//"mongodb://localhost:27017";
        //    logger.DebugFormat("dbServer = {0}", dbServer);
        //    string dbName = System.Configuration.ConfigurationManager.AppSettings["mongodbDB"];//"scadaDB";
        //    logger.DebugFormat("dbName = {0}", dbName);

        //    try
        //    {
        //        scadaDB = new MongoClient(dbServer).GetDatabase(dbName);
        //        logger.DebugFormat("GetDatabase {0} success", dbName);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.ErrorFormat("GetDatabase {0} Error:{1}", dbName, ex.Message, ex);
        //        throw;
        //    }


        //    if (!scadaDB.Client.Settings.IsFrozen)
        //    {
        //        logger.DebugFormat("设置 ReadPreference.SecondaryPreferred ");
        //        // 读写分享设置，优先从副本读取
        //        scadaDB.Settings.ReadPreference = ReadPreference.SecondaryPreferred;
        //    }
        //}

        protected IMongoCollection<T> table;

        static ILog logger = LogManager.GetLogger<MongDbScadaServiceBase<T>>();

        protected string IdFieldName = "TagNo";

        static MongDbScadaServiceBase()
        {
            //initDB();
            //string dbServer = System.Configuration.ConfigurationManager.AppSettings["mongodbServer"];//"mongodb://localhost:27017";
            //logger.DebugFormat("dbServer = {0}", dbServer);
            //string dbName = System.Configuration.ConfigurationManager.AppSettings["mongodbDB"];//"scadaDB";
            //logger.DebugFormat("dbName = {0}", dbName);

            //try
            //{
            //    scadaDB = new MongoClient(dbServer).GetDatabase(dbName);
            //    logger.DebugFormat("GetDatabase {0} success", dbName);
            //}
            //catch (Exception ex)
            //{
            //    logger.ErrorFormat("GetDatabase {0} Error:{1}", dbName, ex.Message, ex);
            //    throw;
            //}
           

            //if (!scadaDB.Client.Settings.IsFrozen)
            //{
            //    // 读写分享设置，优先从副本读取
            //    scadaDB.Settings.ReadPreference = ReadPreference.SecondaryPreferred;
            //}
        }

        public MongDbScadaServiceBase()          
        {
            table = ScadaDB.GetCollection<T>(Name);
        }

        public MongDbScadaServiceBase(string tableName)
        {
            name = tableName;
            table = ScadaDB.GetCollection<T>(name);
        }

      

        private string name = "ScadaData";

        protected virtual string Name
        {
            get { return name; }
        }

        public virtual void UpdateOne(ScadaData data)
        {
            
                        
        }

        public virtual async Task UpdateOneAsync(ScadaData data)
        {
            await Task.Delay(1);
           // await table.ReplaceOneAsync(p => p.TagNo == data.TagNo, data, updateOptions);
        }
        

        public void Insert(T data)
        {
            table.InsertOne(data);           
        }

        public async Task InsertAsync(T data)
        {
            await table.InsertOneAsync(data);
        }

      

        public void InsertList(IList<T> datas)
        {
            table.InsertMany(datas);
        }

        public async Task InsertListAsync(IList<T> datas)
        {
            await table.InsertManyAsync(datas);
        }

        public T Get(string tagNo)
        {
            var filter = Builders<T>.Filter.Eq(IdFieldName, tagNo);
            var doc = table.Find(filter).FirstOrDefaultAsync().Result;
            return doc;
        }

        public async Task<T> GetAsync(string tagNo)
        {
            var filter = Builders<T>.Filter.Eq(IdFieldName, tagNo);
            var task = table.Find(filter).FirstOrDefaultAsync();
            await task;
            return task.Result;
        }

        public IList<T> GetList(string[] tagNos)
        {
            var filter = Builders<T>.Filter.In(IdFieldName, tagNos);
            return table.Find(filter).ToListAsync().Result;
        }

        public async Task<IList<T>> GetListAsync(string[] tagNos)
        {
            var filter = Builders<T>.Filter.In(IdFieldName, tagNos);
            var task = table.Find(filter).ToListAsync();
            await task;
            return task.Result;
        }

        public virtual IMongoCollection<T> GetTable()
        {
            return table;
        }
    }
}
