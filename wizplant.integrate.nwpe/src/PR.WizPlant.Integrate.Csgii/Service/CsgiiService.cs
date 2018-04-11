using PR.WizPlant.Integrate.Csgii.Entities;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Service
{
    public class CsgiiService
    {       
        static string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        public static Dictionary<string, string> mapCache = null;

        public static Dictionary<string, string> MapCache
        {
            get
            {
                if (mapCache == null)
                {
                    mapCache = new Dictionary<string, string>();
                    string selectSql = "SELECT [ObjectId],[DeviceId] FROM [PRW_Inte_Csgii_DeviceMap] WHERE [ObjectId] IS NOT NULL";
                    using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql))
                    {
                        while (reader.Read())
                        {
                            mapCache[reader.GetGuid(0).ToString().ToLower()] = reader.GetString(1);
                        }
                    }
                }
                return mapCache;
            }
        }

        public static Dictionary<string, string> deviceObjMapCache = null;

        public static Dictionary<string, string> DeviceObjMapCache
        {
            get
            {
                if (deviceObjMapCache == null)
                {
                    deviceObjMapCache = new Dictionary<string, string>();
                    string selectSql = "SELECT [ObjectId],[DeviceId] FROM [PRW_Inte_Csgii_DeviceMap] WHERE [ObjectId] IS NOT NULL and [DeviceId] is not null";
                    using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql))
                    {
                        while (reader.Read())
                        {
                            deviceObjMapCache[reader.GetGuid(1).ToString().ToLower()] = reader.GetString(0);
                        }
                    }
                }
                return deviceObjMapCache;
            }
        }

        public static CsgiiDevice GetDevice(string deviceId)
        {
            string selectSql = @"SELECT [Id]
      ,[DeviceId]
      ,[Station]
      ,[DeviceName]
      ,[FullPath]
      ,[ClassPath]
      ,[LevelType]
      ,[Status] FROM  [PRW_Inte_Csgii_Device] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            CsgiiDevice result = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                if (reader.Read())
                {
                    result = new CsgiiDevice();
                    result.Id = reader.GetGuid(0);
                    result.DeviceId = reader.GetString(1);
                    result.Station = reader.GetString(2);
                    result.DeviceName = reader.GetString(3);
                    result.FullPath = getStringValue(reader, 4);
                    result.ClassPath = getStringValue(reader, 5);
                    result.LevelType = reader.GetInt32(6);
                    result.Status = reader.GetInt32(7);
                }
            }
            return result;
        }

        public static CsgiiBasicInfo GetBasicInfo(string deviceId)
        {
            string selectSql = @"SELECT [Id], 
[DeviceId], 
[deviceCode], 
[deviceName], 
[classifyId], 
[classifyName], 
[isCapitalAssets], 
[isCapitalAssetsStr], 
[proprietorCompanyOname], 
[proprietorCompanyOid], 
[baseVoltageId], 
[voltagePageShow], 
[isVirtualDevice], 
[isVirtualDeviceStr], 
[amount], 
[manufacturerId], 
[manufacturer], 
[latestManufacturer], 
[vendor], 
[vendorId], 
[deviceModel], 
[leaveFactoryNo], 
[leaveFactoryDate], 
[warrantyPeriod], 
[plantTransferDate], 
[assetState], 
[assetStateStr], 
[statusDate], 
[latitude], 
[longitude], 
[latitudeStr], 
[longitudeStr], 
[altitude], 
[topography], 
[bureauUnitsOid], 
[bureauUnitsOname], 
[dispatchLevel], 
[dispatchLevelStr], 
[runmanageOid], 
[runmanageOName], 
[vindicateOid], 
[vindicateOName], 
[flName], 
[remark], 
[powerGridFlag], 
[isLabel], 
[isAssambly], 
[isShareDevice], 
[dataSource], 
[proprietorOwner], 
[useLife], 
[deviceModelId], 
[classifyCode], 
[classifyFullPath], 
[nominalVoltage]
 FROM  [PRW_Inte_Csgii_BasicInfo] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            CsgiiBasicInfo result = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                if (reader.Read())
                {
                    result = new CsgiiBasicInfo();
                    result.Id = reader.GetGuid(0);
                    result.DeviceId = reader.GetString(1);
                    result.DeviceCode = reader.GetString(2);
                    result.DeviceName = reader.GetString(3);
                    result.ClassifyId = getStringValue(reader, 4);
                    result.ClassifyName = getStringValue(reader, 5);
                    result.IsCapitalAssets = getBoolValue(reader, 6);
                    result.IsCapitalAssetsStr = getStringValue(reader, 7);
                    result.ProprietorCompanyOname = getStringValue(reader, 8);
                    result.ProprietorCompanyOid = getStringValue(reader, 9);
                    result.BaseVoltageId = getStringValue(reader, 10);
                    result.VoltagePageShow = getStringValue(reader, 11);
                    result.IsVirtualDevice = getBoolValue(reader, 12);
                    result.IsVirtualDeviceStr = getStringValue(reader, 13);
                    result.Amount = getInt32Value(reader, 14);
                    result.ManufacturerId = getStringValue(reader, 15);
                    result.Manufacturer = getStringValue(reader, 16);
                    result.LatestManufacturer = getStringValue(reader, 17);
                    result.Vendor = getStringValue(reader, 18);
                    result.VendorId = getStringValue(reader, 19);
                    result.SeviceModel = getStringValue(reader, 20);
                    result.LeaveFactoryNo = getStringValue(reader, 21);
                    result.LeaveFactoryDate = getDateTimeValue(reader, 22);
                    result.WarrantyPeriod = getInt32Value(reader, 23);
                    result.PlantTransferDate = getDateTimeValue(reader, 24);
                    result.AssetState = getInt32Value(reader, 25);
                    result.AssetStateStr = getStringValue(reader, 26);
                    result.StatusDate = getStringValue(reader, 27);
                    result.Latitude = getStringValue(reader, 28);
                    result.Longitude = getStringValue(reader, 29);
                    result.LatitudeStr = getStringValue(reader, 30);
                    result.LongitudeStr = getStringValue(reader, 31);
                    result.Altitude = getInt32Value(reader, 32);
                    result.Topography = getInt32Value(reader, 33);
                    result.BureauUnitsOid = getStringValue(reader, 34);
                    result.BureauUnitsOname = getStringValue(reader, 35);
                    result.DispatchLevel = getInt32Value(reader, 36);
                    result.DispatchLevelStr = getStringValue(reader, 37);
                    result.RunmanageOid = getStringValue(reader, 38);
                    result.RunmanageOName = getStringValue(reader, 39);
                    result.VindicateOid = getStringValue(reader, 40);
                    result.VindicateOName = getStringValue(reader, 41);
                    result.FlName = getStringValue(reader, 42);
                    result.Remark = getStringValue(reader, 43);
                    result.PowerGridFlag = getInt32Value(reader, 44);
                    result.IsLabel = getBoolValue(reader, 45);
                    result.IsAssambly = getBoolValue(reader, 46);
                    result.IsShareDevice = getBoolValue(reader, 47);
                    result.DataSource = getStringValue(reader, 48);
                    result.ProprietorOwner = getStringValue(reader, 49);
                    result.UseLife = getInt32Value(reader, 50);
                    result.DeviceModelId = getStringValue(reader, 51);
                    result.ClassifyCode = getStringValue(reader, 52);
                    result.ClassifyFullPath = getStringValue(reader, 53);
                    result.NominalVoltage = getStringValue(reader, 54);
                }
            }
            return result;
        }

        public static List<CsgiiTechInfo> GetTechInfos(string deviceId)
        {
            string selectSql = @"SELECT [Id]
      ,[DeviceId]
      ,[classifyId]
      ,[techParamId]
      ,[columnName]
      ,[techParamName]
      ,[techParamValue]
      ,[units]
      ,[isMandatory]
      ,[isShow]
      ,[sortNo]
      ,[dataType]
      ,[isVendorFill]
  FROM [PRW_Inte_Csgii_TechInfo] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            var result = new List<CsgiiTechInfo>();
            CsgiiTechInfo item = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    item = new CsgiiTechInfo();
                    result.Add(item);
                    item.Id = reader.GetGuid(0);
                    item.DeviceId = reader.GetString(1);
                    item.ClassifyId = reader.GetString(2);
                    item.TechParamId = reader.GetString(3);
                    item.ColumnName = getStringValue(reader, 4);
                    item.TechParamName = getStringValue(reader, 5);
                    item.TechParamValue = reader.GetString(6);
                    item.Units = getStringValue(reader, 7);
                    item.IsMandatory = getBoolValue(reader,8);
                    item.IsShow = getBoolValue(reader, 9);
                    item.SortNo = getInt32Value(reader, 10);
                    item.DataType = reader.GetInt32(11);
                    item.IsVendorFill = getBoolValue(reader, 12);
                }
            }
            return result;
        }

        public static string GetJsonTechInfos(string deviceId)
        {
            string selectSql = @"SELECT [techParamName]
      ,[techParamValue]      
  FROM [PRW_Inte_Csgii_TechInfo] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    sb.Append("{");
                    sb.AppendFormat("\"TechParamName\":\"{0}\",\"TechParamValue\":\"{1}\"", reader.GetString(0), reader.GetString(1));
                    sb.Append("},");
                }
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string GetJsonTechInfosOfflineData(string deviceId,string objId)
        {
            string selectSql = @"SELECT [techParamName]
      ,[techParamValue]      
  FROM [PRW_Inte_Csgii_TechInfo] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{\"ObjId\":\"{0}\",\"Params\":",objId));
            sb.Append("[");
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    sb.Append("{");
                    sb.AppendFormat("\"TechParamName\":\"{0}\",\"TechParamValue\":\"{1}\"", reader.GetString(0), reader.GetString(1));
                    sb.Append("},");
                }
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        public static List<CsgiiCSBG> GetCSBGs(string deviceId)
        {
            string selectSql = "SELECT [Id] ,[DeviceId],[CSBGMC],[RQ],[ZY],[XZ],[JL] FROM  [PRW_Inte_Csgii_CSBG] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            var result = new List<CsgiiCSBG>();
            CsgiiCSBG item = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    item = new CsgiiCSBG();
                    result.Add(item);
                    item.Id = reader.GetGuid(0);
                    item.DeviceId = reader.GetString(1);
                    item.CSBGMC = getStringValue(reader, 2);
                    item.RQ = reader.GetDateTime(3);
                    item.ZY = getStringValue(reader, 4);
                    item.XZ = getStringValue(reader, 5);
                    item.JL = getStringValue(reader, 6);
                }
            }
            return result;
        }

        public static List<CsgiiYXQXJL> GetYXQXJLs(string deviceId)
        {
            string selectSql = "SELECT [Id],[DeviceId],[QXBH],[QXLY],[QXDJ],[QXBX],[QXBW],[QXLX],[QXYY],[QXZT],[FXSJ],[XQSJ] FROM [PRW_Inte_Csgii_YXQXJL] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            var result = new List<CsgiiYXQXJL>();
            CsgiiYXQXJL item = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    item = new CsgiiYXQXJL();
                    result.Add(item);
                    item.Id = reader.GetGuid(0);
                    item.DeviceId = reader.GetString(1);                   
                    item.QXBH = getStringValue(reader, 2);
                    item.QXLY = getStringValue(reader, 3);
                    item.QXDJ = getStringValue(reader, 4);
                    item.QXBX = getStringValue(reader, 5);
                    item.QXBW = getStringValue(reader, 6);

                    item.QXLX = getStringValue(reader, 7);
                    item.QXYY = getStringValue(reader, 8);
                    item.QXZT = getStringValue(reader, 9);
                    item.FXSJ = reader.GetDateTime(10);
                    item.XQSJ = getDateTimeValue(reader, 11);                    
                }
            }
            return result;
        }

        public static List<CsgiiYXQXJL> GetYXQXJLsByQuery(string deviceId,string defectLevel,string defectClass,DateTime startTime,DateTime endTime,QueryTimeEnum queryTimeEnum)
        {
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            List<SqlParameter> paraList = new List<SqlParameter>() { para };

            StringBuilder builder = new StringBuilder("SELECT [Id],[DeviceId],[QXBH],[QXLY],[QXDJ],[QXBX],[QXBW],[QXLX],[QXYY],[QXZT],[FXSJ],[XQSJ] FROM [PRW_Inte_Csgii_YXQXJL] where DeviceId=@pDeviceId ");
            if (!string.IsNullOrEmpty(defectLevel))
            {
                builder.Append(" and QXDJ=@defectLevel ");
                paraList.Add(new SqlParameter("@defectLevel", defectLevel));
            }
            if (!string.IsNullOrEmpty(defectClass))
            {
                builder.Append(" and QXLX=@defectClass ");
                paraList.Add(new SqlParameter("@defectClass", defectClass));
            }

            switch (queryTimeEnum)
            {
                case QueryTimeEnum.QueryStart:
                    builder.Append(" and convert(char(10),FXSJ,120) =@startTime ");
                    paraList.Add(new SqlParameter("@startTime", startTime));
                    break;
                case QueryTimeEnum.QueryEnd:
                    builder.Append(" and convert(char(10),XQSJ,120)=@endTime ");
                    paraList.Add(new SqlParameter("@endTime", endTime));
                    break;
                case QueryTimeEnum.QueryInterval:
                    builder.Append(" and convert(char(10),FXSJ,120)=@startTime ");
                    builder.Append(" and convert(char(10),XQSJ,120) =@endTime ");
                    paraList.Add(new SqlParameter("@startTime", startTime));
                    paraList.Add(new SqlParameter("@endTime", endTime));
                    break;
            }

            SqlParameter[] sqlParams = paraList.ToArray();
            var result = new List<CsgiiYXQXJL>();
            CsgiiYXQXJL item = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, builder.ToString(), sqlParams))
            {
                while (reader.Read())
                {
                    item = new CsgiiYXQXJL();
                    result.Add(item);
                    item.Id = reader.GetGuid(0);
                    item.DeviceId = reader.GetString(1);
                    item.QXBH = getStringValue(reader, 2);
                    item.QXLY = getStringValue(reader, 3);
                    item.QXDJ = getStringValue(reader, 4);
                    item.QXBX = getStringValue(reader, 5);
                    item.QXBW = getStringValue(reader, 6);

                    item.QXLX = getStringValue(reader, 7);
                    item.QXYY = getStringValue(reader, 8);
                    item.QXZT = getStringValue(reader, 9);
                    item.FXSJ = reader.GetDateTime(10);
                    item.XQSJ = getDateTimeValue(reader, 11);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取缺陷等级值列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDefectLevels()
        {
            List<string> result = new List<string>();

            string selectSql = "select distinct QXDJ from [PRW_Inte_Csgii_YXQXJL]";
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql))
            {
                while (reader.Read())
                {
                    string tmpValue = getStringValue(reader, 0);
                    if (!string.IsNullOrEmpty(tmpValue))
                    {
                        result.Add(tmpValue);
                    }
                }
            }

            return result;
        }

        public static List<string> GetDefectClassList()
        {
            List<string> result = new List<string>();

            string selectSql = "select distinct QXLX from [PRW_Inte_Csgii_YXQXJL]";
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql))
            {
                while (reader.Read())
                {
                    string tmpValue = getStringValue(reader, 0);
                    if (!string.IsNullOrEmpty(tmpValue))
                    {
                        result.Add(tmpValue);
                    }
                }
            }

            return result;
        }

        public static List<CsgiiZTPJ> GetZTPJs(string deviceId)
        {
            string selectSql = "SELECT [Id],[DeviceId],[PJRQ],[SBJKD],[SBZYD],[SBYWJB] FROM [PRW_Inte_Csgii_ZTPJ] where DeviceId=@pDeviceId";
            SqlParameter para = new SqlParameter("@pDeviceId", deviceId);
            var result = new List<CsgiiZTPJ>();
            CsgiiZTPJ item = null;
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    item = new CsgiiZTPJ();
                    result.Add(item);
                    item.Id = reader.GetGuid(0);
                    item.DeviceId = reader.GetString(1);
                    item.PJRQ = reader.GetDateTime(2);
                    item.SBJKD = getStringValue(reader, 3);
                    item.SBZYD = getStringValue(reader, 4);
                    item.SBYWJB = getStringValue(reader, 5);                   
                }
            }
            return result;
        }

        static string getStringValue(IDataReader reader, int idx)
        {
            if (reader.IsDBNull(idx))
            {
                return null;
            }
            return reader[idx].ToString();
        }

        static int? getInt32Value(IDataReader reader, int idx)
        {
            if (reader.IsDBNull(idx))
            {
                return null;
            }
            return reader.GetInt32(idx);
        }

        static double? getDoubleValue(IDataReader reader, int idx)
        {
            if (reader.IsDBNull(idx))
            {
                return null;
            }
            return reader.GetDouble(idx);
        }

        static DateTime? getDateTimeValue(IDataReader reader, int idx)
        {
            if (reader.IsDBNull(idx))
            {
                return null;
            }
            return reader.GetDateTime(idx);
        }

        static bool? getBoolValue(IDataReader reader, int idx)
        {
            if (reader.IsDBNull(idx))
            {
                return null;
            }
            return reader.GetBoolean(idx);
        }

    }
}
