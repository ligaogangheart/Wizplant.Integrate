using Common.Logging;
using PR.WizPlant.Integrate.Csgii.SBTZSOA;
using PR.WizPlant.Integrate.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PR.WizPlant.CsgiiTool
{
    public class ScanWorker
    {
        SOAServicePort service = new SOAServicePortClient();
        queryDeviceInfoByIdRequest1 request = null;
        queryDeviceInfoByIdResponse1 response = null;
        string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        SqlConnection conn = null;

        DataTable dtBaseInfo = null;
        DataTable dtTech = null;

        public void Run()
        {

            conn = new SqlConnection(connectionString);
           

            OutputLog("启动扫描......");
            request = new  queryDeviceInfoByIdRequest1(new QueryDeviceInfoByIdRequest());

            var startDeviceId = ConfigurationManager.AppSettings["startDeviceId"];
            if (!string.IsNullOrEmpty(startDeviceId))
            {
                OutputLog(string.Format("从设备{0}开始扫描",startDeviceId));
            }
            Stopwatch sw = new Stopwatch();

            string val = System.Configuration.ConfigurationManager.AppSettings["needReset"];
            if (val == "true")
            {
                string truncateSql = "truncate table PRW_Inte_Csgii_TechInfo;truncate table PRW_Inte_Csgii_BasicInfo;";
                OutputLog("正在清除已有数据...");
                try
                {
                    SqlHelper.ExecuteNonQuery(connectionString, truncateSql, null);
                }
                catch (Exception ex)
                {
                    OutputLog(string.Format("清除已有数据失败,错误：{0}", ex.Message), ex);
                }
                OutputLog("完成清除已有数据...");
            }

            var todoList = getDeviceIdList();
            if (todoList.Count == 0)
            {
                OutputLog("没有要处理的设备");
                return;
            }

            dtBaseInfo = SqlHelper.ExecuteEmptyDataTable(connectionString, "PRW_Inte_Csgii_BasicInfo");
            dtBaseInfo.TableName = "PRW_Inte_Csgii_BasicInfo";
            dtTech = SqlHelper.ExecuteEmptyDataTable(connectionString, "PRW_Inte_Csgii_TechInfo");
            dtTech.TableName = "PRW_Inte_Csgii_TechInfo";


            val = System.Configuration.ConfigurationManager.AppSettings["testNums"];

            int len = todoList.Count;
            if (!string.IsNullOrEmpty(val))
            {
                int iNums;
                if (int.TryParse(val,out iNums) && iNums > 0 && iNums < len)
                {
                    len = iNums;
                }
            }

            sw.Restart();
            int defaultMaxlength = 10000;
            int maxLength = defaultMaxlength;
            val = System.Configuration.ConfigurationManager.AppSettings["batchNums"];
            if (!string.IsNullOrEmpty(val))
            {
                if (int.TryParse(val, out maxLength))
                {
                    if (maxLength < 1)
                    {
                        maxLength = defaultMaxlength;
                    }
                }
                else
                {
                    maxLength = defaultMaxlength;
                }
            }
            int succNums = 0;
            // 插入步骤标记
            int stepFlag = 0;

            for (int i = 0; i < len;  )
            {
                var deviceId = todoList[i];
                
                try
                {
                    if (invoke(deviceId))
                    {
                        succNums++;
                    }
                }
                catch (Exception ex)
                {
                    OutputLog(string.Format("获取设备[{0}]的台账信息失败,错误：{1}", deviceId, ex.Message), ex,true);
                }
                finally
                {
                    if ((++i % 100) == 0)
                    {
                        OutputLog(string.Format("已获取{0}个设备的台账信息,其中失败{1}个,总耗时：{2:0.000}秒", i,i-succNums, sw.Elapsed.TotalSeconds));
                    }
                    //OutputLog(string.Format("完成设备[{0}]的台账信息获取,耗时：{1}ms", deviceId, sw.ElapsedMilliseconds));
                }

                if (succNums>0 && (succNums % maxLength) == 0)
                {
                    stepFlag = 0;
                    try
                    {
                        OutputLog(string.Format("保存设备台帐信息,本次保存的设备数:{0}", dtBaseInfo.Rows.Count));
                        SqlHelper.BulkInsert(connectionString, dtBaseInfo);
                        stepFlag = 1;
                        SqlHelper.BulkInsert(connectionString, dtTech);
                    }
                    catch (SqlException sqlEx)
                    {
                        OutputLog(string.Format("保存台账信息时失败,stepFlag[{1}],错误：{0}", sqlEx.Message, stepFlag), sqlEx);
                        if (stepFlag == 0)
                        {
                            dealWithSqlException(dtBaseInfo, sqlEx);
                        }
                        else
                        {
                            dealWithSqlException(dtTech, sqlEx);
                        }
                    }
                    catch (Exception ex)
                    {
                        OutputLog(string.Format("保存台账信息时失败,stepFlag[{1}],错误：{0}", ex.Message, stepFlag), ex);
                    }
                    finally
                    {
                        dtBaseInfo.Clear();
                        dtTech.Clear();
                    }
                }

                
            }

            if (dtBaseInfo.Rows.Count > 0)
            {
                stepFlag = 0;
                try
                {
                    OutputLog(string.Format("保存设备台帐信息,本次保存的设备数:{0}", dtBaseInfo.Rows.Count));
                    SqlHelper.BulkInsert(connectionString, dtBaseInfo);
                    stepFlag = 1;
                    SqlHelper.BulkInsert(connectionString, dtTech);
                }
                catch (SqlException sqlEx)
                {
                    OutputLog(string.Format("保存台账信息时失败,stepFlag[{1}],错误：{0}", sqlEx.Message, stepFlag), sqlEx);
                    if (stepFlag == 0)
                    {
                        dealWithSqlException(dtBaseInfo, sqlEx);
                    }
                    else
                    {
                        dealWithSqlException(dtTech, sqlEx);
                    }
                }
                catch (Exception ex)
                {
                    OutputLog(string.Format("保存台账信息时失败,stepFlag[{1}],错误：{0}", ex.Message, stepFlag), ex);
                }
            }
            OutputLog("扫描结束");
        }

        void dealWithSqlException(DataTable dt, SqlException sqlEx)
        {
            if (sqlEx.Message.IndexOf("无效的列长度") != -1)
            {
                DataColumnCollection dcs = SqlHelper.ExecuteDataTableSchema(connectionString, dt.TableName).Columns; ;
                DataRowCollection drs = dt.Rows;                

                StringBuilder sb = new StringBuilder();
                object obj = null;
                Type strType = typeof(string);
                foreach (DataRow dr in drs)
                {
                    bool hasError = false;
                    foreach (DataColumn dc in dcs)
                    {
                        if (dc.DataType == strType)
                        {
                            if (dc.ColumnName == "Id")
                            {
                                continue;
                            }
                            obj = dr[dc.ColumnName];

                            if (compareLength(dc.MaxLength, obj))
                            {
                                hasError = true;
                                sb.AppendFormat("{0}[{2}]:{1},", dc.ColumnName, obj, dc.MaxLength);
                            }
                        }
                    }
                    if (hasError)
                    {
                        sb.AppendLine();
                    }
                }
                OutputLog(sb.ToString());
            }
        }

        bool compareLength(int maxLength, object obj)
        {
            if (Convert.IsDBNull(obj))
            {
                return false;
            }
            else
            {
                return obj.ToString().Length > maxLength;
            }
        }
  
        List<string> getDeviceIdList()
        {
            string selectSql = "select a.[DeviceId] from [PRW_Inte_Csgii_Device] a where a.DeviceId not in ( select DeviceId from PRW_Inte_Csgii_BasicInfo)";
            List<string> result = new List<string>();
            using (var reader = SqlHelper.ExecuteDataReader(conn, CommandType.Text, selectSql,null))
            {
                while (reader.Read())
                {
                    result.Add(reader[0].ToString());
                }
            }
            return result;
            
        }
       

        void bulkInsertDT(DataTable dt)
        {
            SqlHelper.BulkInsert(connectionString, dt);
        }


        bool invoke(string deviceId)
        {
            request.queryDeviceInfoByIdRequest.deviceId = deviceId;
            bool succ = true;

            try
            {
                response = service.queryDeviceInfoById(request);
            }
            catch (System.ServiceModel.FaultException fe)
            {
                succ = false;
                OutputLog(string.Format("获取设备[{0}]的台账信息失败,服务端信息：{1}", deviceId, fe.Message), fe, true);
            }
            catch (Exception ex)
            {
                succ = false;
                OutputLog(string.Format("获取设备[{0}]的台账信息失败,Error：{1}", deviceId, ex.Message), ex, true);
            }

            //response = new queryDeviceInfoByIdResponse1(new queryDeviceInfoByIdResponse() { deviceGisDTO = new queryDeviceInfoByIdResponseDeviceGisDTO(), replyCode = "OK" });

            if (succ)
            {
                switch (response.queryDeviceInfoByIdResponse.replyCode)
                {
                    case "OK":
                        var dto = response.queryDeviceInfoByIdResponse.deviceGisDTO;
                        dto.deviceId = deviceId;
                        success2(dto);
                        break;
                    default:
                        succ = false;
                        OutputLog(string.Format("获取设备[{0}]的台账信息失败,replyCode：{1}", deviceId, response.queryDeviceInfoByIdResponse.replyCode), null, true);
                        break;

                }
            }
            return succ;
        }

        void success2(DeviceGisDTO dto)
        {
            var dr = dtBaseInfo.NewRow();
            dtBaseInfo.Rows.Add(dr);

            dr["Id"] = Guid.NewGuid();
            dr["DeviceId"] = dto.deviceId;
            dr["deviceCode"] = dto.deviceCode;
            dr["deviceName"] = dto.deviceName;
            dr["classifyId"] = dto.classifyId;
            dr["classifyName"] = dto.classifyName;
            setBoolValue(dr["isCapitalAssets"], dto.isCapitalAssets);    
            dr["isCapitalAssetsStr"] = dto.isCapitalAssetsStr;
            dr["proprietorCompanyOname"] = dto.proprietorCompanyOname;
            dr["proprietorCompanyOid"] = dto.proprietorCompanyOid;
            dr["baseVoltageId"] = dto.baseVoltageId;
            dr["voltagePageShow"] = dto.voltagePageShow;
            setBoolValue(dr["isVirtualDevice"], dto.isVirtualDevice);
            
            dr["isVirtualDeviceStr"] = dto.isVirtualDeviceStr;
            setInt32Value(dr["amount"], dto.amount);
            dr["manufacturerId"] = dto.manufacturerId;
            dr["manufacturer"] = dto.manufacturer;
            dr["latestManufacturer"] = dto.latestManufacturer;
            dr["vendor"] = dto.vendor;
            dr["vendorId"] = dto.vendorId;
            dr["deviceModel"] = dto.deviceModel;
            dr["leaveFactoryNo"] = dto.leaveFactoryNo;
            SetDateTimeValue(dr["leaveFactoryDate"], dto.leaveFactoryDateStr);
            
            setInt32Value(dr["warrantyPeriod"], dto.warrantyPeriod);
            SetDateTimeValue(dr["plantTransferDate"], dto.plantTransferDateStr);

            setInt32Value(dr["assetState"], dto.assetState);
            dr["assetStateStr"] = dto.assetStateStr;
            SetDateTimeValue(dr["statusDate"], dto.statusDateStr);
           
            dr["latitude"] = dto.latitude;
            dr["longitude"] = dto.longitude;
            dr["latitudeStr"] = dto.latitudeStr;
            dr["longitudeStr"] = dto.longitudeStr;
            setInt32Value(dr["altitude"], dto.altitude);
            setInt32Value(dr["topography"], dto.topography);
            dr["bureauUnitsOid"] = dto.bureauUnitsOid;
            dr["bureauUnitsOname"] = dto.bureauUnitsOname;
            setInt32Value(dr["dispatchLevel"], dto.dispatchLevel);
            dr["dispatchLevelStr"] = dto.dispatchLevelStr;
            dr["runmanageOid"] = dto.runmanageOid;
            dr["runmanageOName"] = dto.runmanageOName;
            dr["vindicateOid"] = dto.vindicateOid;
            dr["vindicateOName"] = dto.vindicateOName;
            dr["flName"] = dto.flName;
            dr["remark"] = dto.remark;
            setInt32Value(dr["powerGridFlag"], dto.powerGridFlag);
            setBoolValue(dr["isLabel"], dto.isLabel);
            setBoolValue(dr["isAssambly"], dto.isAssambly);
            setBoolValue(dr["isShareDevice"], dto.isShareDevice);
            dr["dataSource"] = dto.dataSource;
            setInt32Value(dr["proprietorOwner"], dto.proprietorOwner);
            setInt32Value(dr["useLife"], dto.useLife);
            dr["deviceModelId"] = dto.deviceModelId;
            dr["classifyCode"] = dto.classifyCode;
            dr["classifyFullPath"] = dto.classifyFullPath;
            setInt32Value(dr["nominalVoltage"], dto.nominalVoltage);


            if (dto.lstTechParamList != null)
            {
                foreach (var item in dto.lstTechParamList)
                {
                    dr = dtTech.NewRow();
                    dtTech.Rows.Add(dr);
                    dr["Id"] = Guid.NewGuid();
                    dr["DeviceId"] = dto.deviceId;
                    dr["classifyId"] = item.classifyId;
                    dr["techParamId"] = item.techParamId;
                    dr["columnName"] = item.columnName;
                    dr["techParamName"] = item.techParamName;
                    dr["techParamValue"] = item.techParamValue;
                    dr["units"] = item.units;
                    setBoolValue(dr["isMandatory"], item.isMandatory);
                    setBoolValue(dr["isShow"], item.isShow);
                    setInt32Value(dr["sortNo"], item.sortNo);
                    dr["dataType"] = item.dataType;
                    setBoolValue(dr["isVendorFill"], item.isVendorFill);
                }
            }
        }

        void setInt32Value(object dr, string value)
        {
            int temp;
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out temp))
            {
                dr = DBNull.Value;
            }
            else
            {
                dr = temp;
            }
        }

        void setDoubleValue(object dr, string value)
        {
            double temp;
            if (string.IsNullOrEmpty(value) || !double.TryParse(value, out temp))
            {
                dr = DBNull.Value;
            }
            else
            {
                dr = temp;
            }
        }

        void SetDateTimeValue(object dr, string value)
        {
            DateTime temp;
            if (string.IsNullOrEmpty(value) || !DateTime.TryParse(value, out temp))
            {
                dr = DBNull.Value;
            }
            else
            {
                dr = temp;
            }
        }

        void setBoolValue(object dr, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int iVal;
                if (int.TryParse(value, out iVal))
                {
                    dr = iVal != 0;
                }
                else
                {
                    bool bVal;
                    if (bool.TryParse(value, out bVal))
                    {
                        dr = bVal;
                    }
                    else
                    {
                        dr = false;
                    }
                }
            }
            else
            {
                dr = DBNull.Value;
            }
        }

        void success(DeviceGisDTO dto)
        {
            var insertSql = @"INSERT INTO [PRW_Inte_Csgii_BasicInfo]
           ([Id]
           ,[DeviceId]
           ,[deviceCode]
           ,[deviceName]
           ,[classifyId]
           ,[classifyName]
           ,[isCapitalAssets]
           ,[isCapitalAssetsStr]
           ,[proprietorCompanyOname]
           ,[proprietorCompanyOid]
           ,[baseVoltageId]
           ,[voltagePageShow]
           ,[isVirtualDevice]
           ,[isVirtualDeviceStr]
           ,[amount]
           ,[manufacturerId]
           ,[manufacturer]
           ,[latestManufacturer]
           ,[vendor]
           ,[vendorId]
           ,[deviceModel]
           ,[leaveFactoryNo]
           ,[leaveFactoryDate]
           ,[warrantyPeriod]
           ,[plantTransferDate]
           ,[assetState]
           ,[assetStateStr]
           ,[statusDate]
           ,[latitude]
           ,[longitude]
           ,[latitudeStr]
           ,[longitudeStr]
           ,[altitude]
           ,[topography]
           ,[bureauUnitsOid]
           ,[bureauUnitsOname]
           ,[dispatchLevel]
           ,[dispatchLevelStr]
           ,[runmanageOid]
           ,[runmanageOName]
           ,[vindicateOid]
           ,[vindicateOName]
           ,[flName]
           ,[remark]
           ,[powerGridFlag]
           ,[isLabel]
           ,[isAssambly]
           ,[isShareDevice]
           ,[dataSource]
           ,[proprietorOwner]
           ,[useLife]
           ,[deviceModelId]
           ,[classifyCode]
           ,[classifyFullPath]
           ,[nominalVoltage])
     VALUES
           (newid()
,@pDeviceId
,@pdeviceCode
,@pdeviceName
,@pclassifyId
,@pclassifyName
,@pisCapitalAssets
,@pisCapitalAssetsStr
,@pproprietorCompanyOname
,@pproprietorCompanyOid
,@pbaseVoltageId
,@pvoltagePageShow
,@pisVirtualDevice
,@pisVirtualDeviceStr
,@pamount
,@pmanufacturerId
,@pmanufacturer
,@platestManufacturer
,@pvendor
,@pvendorId
,@pdeviceModel
,@pleaveFactoryNo
,@pleaveFactoryDate
,@pwarrantyPeriod
,@pplantTransferDate
,@passetState
,@passetStateStr
,@pstatusDate
,@platitude
,@plongitude
,@platitudeStr
,@plongitudeStr
,@paltitude
,@ptopography
,@pbureauUnitsOid
,@pbureauUnitsOname
,@pdispatchLevel
,@pdispatchLevelStr
,@prunmanageOid
,@prunmanageOName
,@pvindicateOid
,@pvindicateOName
,@pflName
,@premark
,@ppowerGridFlag
,@pisLabel
,@pisAssambly
,@pisShareDevice
,@pdataSource
,@pproprietorOwner
,@puseLife
,@pdeviceModelId
,@pclassifyCode
,@pclassifyFullPath
,@pnominalVoltage)
";

            var paras = new List<SqlParameter>();

            paras.Add(new SqlParameter("@pDeviceId", dto.deviceId));
            paras.Add(new SqlParameter("@pdeviceCode", dto.deviceCode));
            paras.Add(new SqlParameter("@pdeviceName", dto.deviceName));
            paras.Add(new SqlParameter("@pclassifyId", dto.classifyId));
            paras.Add(new SqlParameter("@pclassifyName", dto.classifyName));
            paras.Add(new SqlParameter("@pisCapitalAssets", dto.isCapitalAssets));
            paras.Add(new SqlParameter("@pisCapitalAssetsStr", dto.isCapitalAssetsStr));
            paras.Add(new SqlParameter("@pproprietorCompanyOname", dto.proprietorCompanyOname));
            paras.Add(new SqlParameter("@pproprietorCompanyOid", dto.proprietorCompanyOid));
            paras.Add(new SqlParameter("@pbaseVoltageId", dto.baseVoltageId));
            paras.Add(new SqlParameter("@pvoltagePageShow", dto.voltagePageShow));
            paras.Add(new SqlParameter("@pisVirtualDevice", dto.isVirtualDevice));
            paras.Add(new SqlParameter("@pisVirtualDeviceStr", dto.isVirtualDeviceStr));
            paras.Add(new SqlParameter("@pamount", dto.amount));
            paras.Add(new SqlParameter("@pmanufacturerId", dto.manufacturerId));
            paras.Add(new SqlParameter("@pmanufacturer", dto.manufacturer));
            paras.Add(new SqlParameter("@platestManufacturer", dto.latestManufacturer));
            paras.Add(new SqlParameter("@pvendor", dto.vendor));
            paras.Add(new SqlParameter("@pvendorId", dto.vendorId));
            paras.Add(new SqlParameter("@pdeviceModel", dto.deviceModel));
            paras.Add(new SqlParameter("@pleaveFactoryNo", dto.leaveFactoryNo));
            DateTime temp ;
            DateTime sqlMinDate = new DateTime(1753, 1, 1);
            DateTime sqlMaxDate = new DateTime(9999, 12, 31);
            
            if (string.IsNullOrEmpty(dto.leaveFactoryDateStr) || !DateTime.TryParse(dto.leaveFactoryDateStr, out temp))
            {
                paras.Add(new SqlParameter("@pleaveFactoryDate", DBNull.Value));
            }
            else
            {
                if (temp > sqlMinDate && temp < sqlMinDate)
                {
                    paras.Add(new SqlParameter("@pleaveFactoryDate", temp));
                }
                else
                {
                    paras.Add(new SqlParameter("@pleaveFactoryDate", DBNull.Value));
                }
            }
            paras.Add(new SqlParameter("@pwarrantyPeriod", dto.warrantyPeriod));

            if (string.IsNullOrEmpty(dto.plantTransferDateStr) || !DateTime.TryParse(dto.plantTransferDateStr, out temp))
            {
                paras.Add(new SqlParameter("@pplantTransferDate", DBNull.Value));
            }
            else
            {
                if (temp > sqlMinDate && temp < sqlMinDate)
                {
                    paras.Add(new SqlParameter("@pplantTransferDate", temp));
                }
                else
                {
                    paras.Add(new SqlParameter("@pplantTransferDate", DBNull.Value));
                }
            }
            paras.Add(new SqlParameter("@passetState", dto.assetState));
            paras.Add(new SqlParameter("@passetStateStr", dto.assetStateStr));
            if (string.IsNullOrEmpty(dto.statusDateStr) || !DateTime.TryParse(dto.statusDateStr, out temp))
            {
                paras.Add(new SqlParameter("@pstatusDate", DBNull.Value));
            }
            else
            {
                if (temp > sqlMinDate && temp < sqlMinDate)
                {
                    paras.Add(new SqlParameter("@pstatusDate", temp));
                }
                else
                {
                    paras.Add(new SqlParameter("@pstatusDate", DBNull.Value));
                }
            }
            paras.Add(new SqlParameter("@platitude", dto.latitude));
            paras.Add(new SqlParameter("@plongitude", dto.longitude));
            paras.Add(new SqlParameter("@platitudeStr", dto.latitudeStr));
            paras.Add(new SqlParameter("@plongitudeStr", dto.longitudeStr));
            paras.Add(new SqlParameter("@paltitude", dto.altitude));
            paras.Add(new SqlParameter("@ptopography", dto.topography));
            paras.Add(new SqlParameter("@pbureauUnitsOid", dto.bureauUnitsOid));
            paras.Add(new SqlParameter("@pbureauUnitsOname", dto.bureauUnitsOname));
            paras.Add(new SqlParameter("@pdispatchLevel", dto.dispatchLevel));
            paras.Add(new SqlParameter("@pdispatchLevelStr", dto.dispatchLevelStr));
            paras.Add(new SqlParameter("@prunmanageOid", dto.runmanageOid));
            paras.Add(new SqlParameter("@prunmanageOName", dto.runmanageOName));
            paras.Add(new SqlParameter("@pvindicateOid", dto.vindicateOid));
            paras.Add(new SqlParameter("@pvindicateOName", dto.vindicateOName));
            paras.Add(new SqlParameter("@pflName", dto.flName));
            paras.Add(new SqlParameter("@premark", dto.remark));
            paras.Add(new SqlParameter("@ppowerGridFlag", dto.powerGridFlag));
            paras.Add(new SqlParameter("@pisLabel", dto.isLabel));
            paras.Add(new SqlParameter("@pisAssambly", dto.isAssambly));
            paras.Add(new SqlParameter("@pisShareDevice", dto.isShareDevice));
            paras.Add(new SqlParameter("@pdataSource", dto.dataSource));
            paras.Add(new SqlParameter("@pproprietorOwner", dto.proprietorOwner));
            paras.Add(new SqlParameter("@puseLife", dto.useLife));
            paras.Add(new SqlParameter("@pdeviceModelId", dto.deviceModelId));
            paras.Add(new SqlParameter("@pclassifyCode", dto.classifyCode));
            paras.Add(new SqlParameter("@pclassifyFullPath", dto.classifyFullPath));
            paras.Add(new SqlParameter("@pnominalVoltage", dto.nominalVoltage));



            string templ = "insert into PRW_Inte_Csgii_TechInfo(Id,DeviceId,classifyId,techParamId,columnName,techParamName,techParamValue,units,isMandatory,isShow,sortNo,dataType,isVendorFill) values(newid(),'{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},{9},{10},{11});";
            StringBuilder sb = new StringBuilder();

            if (dto.lstTechParamList != null)
            {
                foreach (var paraItem in dto.lstTechParamList)
                {
                    sb.AppendFormat(templ, dto.deviceId, paraItem.classifyId, paraItem.techParamId, paraItem.columnName, paraItem.techParamName, paraItem.techParamValue, paraItem.units, paraItem.isMandatory, paraItem.isShow, paraItem.sortNo, paraItem.dataType, paraItem.isVendorFill);
                }
            }
            
            SqlTransaction bt = null;
            try
            {
                conn.Open();
                bt = conn.BeginTransaction();
                SqlHelper.ExecuteNonQuery(bt, CommandType.Text, insertSql, paras.ToArray());
                if (sb.Length > 0)
                {
                    SqlHelper.ExecuteNonQuery(bt, CommandType.Text, sb.ToString(), paras.ToArray());
                }
                bt.Commit();
            }
            catch (Exception ex)
            {
                OutputLog(string.Format("保存{0}时失败:{1}", dto.deviceId, ex.Message), ex);
                OutputLog(dto.ToJsonString());
                if (bt != null)
                {
                    bt.Rollback();
                }

            }
            finally
            {

                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }      


        #region Log
        ILog logger = LogManager.GetLogger("csgii");
        void OutputLog(string message)
        {
            OutputLog(message, false);
        }
        void OutputLog(string message,bool onlyLog)
        {
            if (!onlyLog)
            {
                Console.WriteLine("{0:HH:mm:ss} {1}", DateTime.Now, message);
            }
            logger.InfoFormat(message);
        }

        void OutputLog(string message, Exception ex)
        {
            OutputLog(message, ex,false);
        }

        void OutputLog(string message, Exception ex, bool onlyLog)
        {
            if (!onlyLog)
            {
                Console.WriteLine("{0:HH:mm:ss} Error:{1}", DateTime.Now, message);
            }
            if (ex != null)
            {
                logger.Error(message, ex);
            }
            else
            {
                logger.Error(message);
            }
        }
        #endregion

    }
}
