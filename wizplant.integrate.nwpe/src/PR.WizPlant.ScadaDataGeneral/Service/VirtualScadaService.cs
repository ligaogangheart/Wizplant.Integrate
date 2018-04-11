using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using LinqToDB;

namespace PR.WizPlant.ScadaDataGeneral.Service
{
    [ServiceContract]
    public class VirtualScadaService
    {
        [OperationContract]
        [WebGet(ResponseFormat=WebMessageFormat.Json)]
        public async Task<List<ScadaData>> getAllData()
        {
            return await Task.Factory.StartNew(() => {
                List<ScadaData> list = new List<ScadaData>();
                using (var db = new DataModels.TagValueDB())
                {
                    var query = from c in db.ProjectTagValue
                                select c;
                    foreach (var tagValue in query)
                    {
                        var scadaData = new ScadaData();
                        scadaData.TagNo = tagValue.TagNo;
                        scadaData.MessureType = tagValue.MessureType;
                        scadaData.Name = tagValue.Name;
                        scadaData.SiteName = tagValue.SiteName;
                        scadaData.TimeStamp = tagValue.TimeStamp;
                        scadaData.Type = (int)tagValue.Type == 1 ? ScadaDataType.YX : ScadaDataType.YC;
                        scadaData.Value = tagValue.Value;
                        list.Add(scadaData);
                    }
                }
                return list;
            });
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public async Task<List<ScadaData>> getLastData(int millsecond)
        {
            return await Task.Factory.StartNew(() => {
                List<ScadaData> list = new List<ScadaData>();

                using (var db = new DataModels.TagValueDB())
                {
                    DateTime timespan = DateTime.Now.AddMilliseconds(-millsecond);
                    var query = from c in db.ProjectTagValue
                                where c.TimeStamp >= timespan
                                select c;
                    foreach (var tagValue in query)
                    {
                        var scadaData = new ScadaData();
                        scadaData.TagNo = tagValue.TagNo;
                        scadaData.MessureType = tagValue.MessureType;
                        scadaData.Name = tagValue.Name;
                        scadaData.SiteName = tagValue.SiteName;
                        scadaData.TimeStamp = tagValue.TimeStamp;
                        scadaData.Type = (int)tagValue.Type == 1 ? ScadaDataType.YX : ScadaDataType.YC;
                        scadaData.Value = tagValue.Value;
                        list.Add(scadaData);
                    }
                }

                return list;
            });
        }
    }
}
