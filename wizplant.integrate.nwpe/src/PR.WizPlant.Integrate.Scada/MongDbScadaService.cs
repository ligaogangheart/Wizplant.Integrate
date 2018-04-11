using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public abstract class MongDbScadaService<T>:IScadaService<T>
    {
        /// <summary>
        /// 时标起始时间
        /// </summary>
        public static readonly DateTime BeginTime = new DateTime(1970, 1, 1);
        private IMongoDatabase scadaDB = null;
        private IMongoCollection<T> table;

        private string idFieldName = "DeviceNo";  

        public MongDbScadaService()
        {
            string dbServer = System.Configuration.ConfigurationManager.AppSettings["mongodbServer"];//"mongodb://localhost:27017";
            string dbName = System.Configuration.ConfigurationManager.AppSettings["mongodbDB"];//"scadaDB";
            scadaDB = new MongoClient(dbServer).GetDatabase(dbName);
            table = scadaDB.GetCollection<T>(Name);
        }       

        /// <summary>
        /// 对象表名
        /// </summary>
        protected abstract string Name { get; }       
        

        public void Save(T data)
        {
            table.InsertOne(data);           
        }

        public async Task SaveAsync(T data)
        {
            await table.InsertOneAsync(data);
        }

        public void Save(IList<T> datas)
        {
            table.InsertMany(datas);
        }

        public async Task SaveAsync(IList<T> datas)
        {
            await table.InsertManyAsync(datas);
        }

        public T Get(int deviceId)
        {
            var filter = Builders<T>.Filter.Eq(idFieldName, deviceId);
            var doc = table.Find(filter).FirstOrDefaultAsync().Result;
            return doc;
        }

        public async Task<T> GetAsync(int deviceId)
        {
            var filter = Builders<T>.Filter.Eq(idFieldName, deviceId);
            var task = table.Find(filter).FirstOrDefaultAsync();
            await task;
            return task.Result;
        }

        public IList<T> GetList(int[] deviceIds)
        {
            var filter = Builders<T>.Filter.In(idFieldName, deviceIds);
            return table.Find(filter).ToListAsync().Result;
        }

        public async Task<IList<T>> GetListAsync(int[] deviceIds)
        {
            var filter = Builders<T>.Filter.In(idFieldName, deviceIds);
            var task = table.Find(filter).ToListAsync();
            await task;
            return task.Result;
        }
    }
}
