using MongoDB.Driver;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbYXScadaService : MongDbScadaService<YXData>
    {
        UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };
        protected override string Name
        {
            get { return "YXData"; }
        }

        public override void Update(YXData data)
        {
            table.ReplaceOne(p => p.TagNo == data.TagNo, data, updateOptions);
        }

        public override async Task UpdateAsync(YXData data)
        {
            await table.ReplaceOneAsync(p => p.TagNo == data.TagNo, data, updateOptions);
        }
    }
}
