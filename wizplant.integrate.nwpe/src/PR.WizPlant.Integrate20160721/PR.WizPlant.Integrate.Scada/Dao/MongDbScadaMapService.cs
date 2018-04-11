using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbScadaMapService : MongDbScadaServiceBase<ScadaObjectMap>
    {

        private static DateTime GetStartDate(int date)
        {
            return new DateTime(date / 10000, (date % 10000) / 100, (date % 10000) % 100);
        }

        private static DateTime GetEndDate(int date)
        {
            return new DateTime(date / 10000, (date % 10000) / 100, (date % 10000) % 100, 23, 59, 59);
        }

        const string MapTableName = "ScadaMap";

        protected override string Name
        {
            get { return MapTableName; }
        }

        public  void Update(ScadaObjectMap data)
        {
            table.ReplaceOne(p => p.TagNo == data.TagNo, data, updateOptions);
        }

        public async Task UpdateAsync(ScadaObjectMap data)
        {
           await table.ReplaceOneAsync(p => p.TagNo == data.TagNo, data, updateOptions);
        }


        #region mapCache
        static object cacheLockObject = new object();

        private static Dictionary<string, List<ScadaObjectMap>> object2TagMapCache = null;
        public static Dictionary<string, List<ScadaObjectMap>> Object2TagMapCache
        {
            get
            {
                if (object2TagMapCache == null)
                {
                    ResetMapCache();
                }
                return object2TagMapCache;
            }
        }


        private static Dictionary<string, List<ScadaObjectMap>> tag2ObjectMapCache = null;
        public static Dictionary<string, List<ScadaObjectMap>> Tag2ObjectMapCache
        {
            get
            {
                if (tag2ObjectMapCache == null)
                {
                    ResetMapCache();
                }
                return tag2ObjectMapCache;
            }
        }

        static void ResetMapCache()
        {
            lock (cacheLockObject)
            {
                var table = scadaDB.GetCollection<ScadaObjectMap>(MapTableName);
                var list = table.Find(Builders<ScadaObjectMap>.Filter.Where(p => String.IsNullOrEmpty(p.ObjectId) == false)).ToListAsync().Result;

                if (tag2ObjectMapCache == null)
                {
                    tag2ObjectMapCache = new Dictionary<string, List<ScadaObjectMap>>();
                }
                else
                {
                    tag2ObjectMapCache.Clear();
                }

                list.GroupBy(p => p.TagNo).ToList().ForEach(item =>
                {
                    tag2ObjectMapCache[item.Key] = item.ToList();
                });

                if (object2TagMapCache == null)
                {
                    object2TagMapCache = new Dictionary<string, List<ScadaObjectMap>>();
                }
                else
                {
                    object2TagMapCache.Clear();
                }

                list.GroupBy(p => p.ObjectId.ToLower()).ToList().ForEach(item =>
                {
                    object2TagMapCache[item.Key] = item.ToList();
                });
            }
        }
        #endregion

    }
}
