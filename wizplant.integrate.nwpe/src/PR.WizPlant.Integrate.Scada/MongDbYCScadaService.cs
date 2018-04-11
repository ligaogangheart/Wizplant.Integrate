using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace PR.WizPlant.Integrate.Scada
{
    public class MongDbYCScadaService : MongDbScadaService<YCData>
    {
        UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };

        protected override string Name
        {
            get { return "YCData"; }
        }

        public override void Update(YCData data)
        {
            table.ReplaceOne(p => p.TagNo == data.TagNo, data, updateOptions);
        }

        public override async Task UpdateAsync(YCData data)
        {
            await table.ReplaceOneAsync(p => p.TagNo == data.TagNo, data, updateOptions);
        }
    }
}
