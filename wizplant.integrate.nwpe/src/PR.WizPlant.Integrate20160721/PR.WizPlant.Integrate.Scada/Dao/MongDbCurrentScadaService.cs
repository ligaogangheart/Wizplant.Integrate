using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbCurrentScadaService : MongDbScadaService
    {
        protected override string Name
        {
            get { return "CurrentScadaData"; }
        }

        public override void UpdateOne(ScadaData data)
        {
            table.UpdateOne(
                 Builders<ScadaData>.Filter.Eq(p => p.TagNo, data.TagNo),
                 Builders<ScadaData>.Update.Set(p => p.TimeStamp, data.TimeStamp).Set(p => p.Value, data.Value),
                 updateOptions);
        }

        public override async Task UpdateOneAsync(ScadaData data)
        {
            await table.UpdateOneAsync(
                Builders<ScadaData>.Filter.Eq(p => p.TagNo, data.TagNo), 
                Builders<ScadaData>.Update.Set(p => p.TimeStamp, data.TimeStamp).Set(p => p.Value, data.Value),
                updateOptions);
        }  


    }
}
