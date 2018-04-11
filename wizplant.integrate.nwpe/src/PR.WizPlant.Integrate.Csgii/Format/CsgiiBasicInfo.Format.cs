using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public partial class CsgiiBasicInfo
    {
        public string ToJsonString()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendFormat("\"Id\":\"{0}\"", this.Id);
            sb.AppendFormat(",\"DeviceId\":\"{0}\"", this.DeviceId);
            sb.AppendFormat(",\"DeviceCode\":\"{0}\"", this.DeviceCode);
            sb.AppendFormat(",\"DeviceName\":\"{0}\"", this.DeviceName);
            sb.AppendFormat(",\"ClassifyId\":\"{0}\"", this.ClassifyId);
            sb.AppendFormat(",\"ClassifyName\":\"{0}\"", this.ClassifyName);
            sb.AppendFormat(",\"IsCapitalAssets\":\"{0}\"", this.IsCapitalAssets);
            sb.AppendFormat(",\"IsCapitalAssetsStr\":\"{0}\"", this.IsCapitalAssetsStr);
            sb.AppendFormat(",\"ProprietorCompanyOname\":\"{0}\"", this.ProprietorCompanyOname);
            sb.AppendFormat(",\"ProprietorCompanyOid\":\"{0}\"", this.ProprietorCompanyOid);
            sb.AppendFormat(",\"BaseVoltageId\":\"{0}\"", this.BaseVoltageId);
            sb.AppendFormat(",\"VoltagePageShow\":\"{0}\"", this.VoltagePageShow);
            sb.AppendFormat(",\"IsVirtualDevice\":\"{0}\"", this.IsVirtualDevice);
            sb.AppendFormat(",\"IsVirtualDeviceStr\":\"{0}\"", this.IsVirtualDeviceStr);
            sb.AppendFormat(",\"Amount\":\"{0}\"", this.Amount);
            sb.AppendFormat(",\"ManufacturerId\":\"{0}\"", this.ManufacturerId);
            sb.AppendFormat(",\"Manufacturer\":\"{0}\"", this.Manufacturer);
            sb.AppendFormat(",\"LatestManufacturer\":\"{0}\"", this.LatestManufacturer);
            sb.AppendFormat(",\"Vendor\":\"{0}\"", this.Vendor);
            sb.AppendFormat(",\"VendorId\":\"{0}\"", this.VendorId);
            sb.AppendFormat(",\"SeviceModel\":\"{0}\"", this.SeviceModel);
            sb.AppendFormat(",\"LeaveFactoryNo\":\"{0}\"", this.LeaveFactoryNo);
            sb.AppendFormat(",\"LeaveFactoryDate\":\"{0}\"", this.LeaveFactoryDate);
            sb.AppendFormat(",\"WarrantyPeriod\":\"{0}\"", this.WarrantyPeriod);
            sb.AppendFormat(",\"PlantTransferDate\":\"{0}\"", this.PlantTransferDate);
            sb.AppendFormat(",\"AssetState\":\"{0}\"", this.AssetState);
            sb.AppendFormat(",\"AssetStateStr\":\"{0}\"", this.AssetStateStr);
            sb.AppendFormat(",\"StatusDate\":\"{0}\"", this.StatusDate);
            sb.AppendFormat(",\"Latitude\":\"{0}\"", this.Latitude);
            sb.AppendFormat(",\"Longitude\":\"{0}\"", this.Longitude);
            sb.AppendFormat(",\"LatitudeStr\":\"{0}\"", encodeJson(this.LatitudeStr));
            sb.AppendFormat(",\"LongitudeStr\":\"{0}\"", encodeJson(this.LongitudeStr));
            sb.AppendFormat(",\"Altitude\":\"{0}\"", this.Altitude);
            sb.AppendFormat(",\"Topography\":\"{0}\"", this.Topography);
            sb.AppendFormat(",\"BureauUnitsOid\":\"{0}\"", this.BureauUnitsOid);
            sb.AppendFormat(",\"BureauUnitsOname\":\"{0}\"", this.BureauUnitsOname);
            sb.AppendFormat(",\"DispatchLevel\":\"{0}\"", this.DispatchLevel);
            sb.AppendFormat(",\"DispatchLevelStr\":\"{0}\"", this.DispatchLevelStr);
            sb.AppendFormat(",\"RunmanageOid\":\"{0}\"", this.RunmanageOid);
            sb.AppendFormat(",\"RunmanageOName\":\"{0}\"", this.RunmanageOName);
            sb.AppendFormat(",\"VindicateOid\":\"{0}\"", this.VindicateOid);
            sb.AppendFormat(",\"VindicateOName\":\"{0}\"", this.VindicateOName);
            sb.AppendFormat(",\"FlName\":\"{0}\"", this.FlName);
            sb.AppendFormat(",\"Remark\":\"{0}\"", this.Remark);
            sb.AppendFormat(",\"PowerGridFlag\":\"{0}\"", this.PowerGridFlag);
            sb.AppendFormat(",\"IsLabel\":\"{0}\"", this.IsLabel);
            sb.AppendFormat(",\"IsAssambly\":\"{0}\"", this.IsAssambly);
            sb.AppendFormat(",\"IsShareDevice\":\"{0}\"", this.IsShareDevice);
            sb.AppendFormat(",\"DataSource\":\"{0}\"", this.DataSource);
            sb.AppendFormat(",\"ProprietorOwner\":\"{0}\"", this.ProprietorOwner);
            sb.AppendFormat(",\"UseLife\":\"{0}\"", this.UseLife);
            sb.AppendFormat(",\"DeviceModelId\":\"{0}\"", this.DeviceModelId);
            sb.AppendFormat(",\"ClassifyCode\":\"{0}\"", this.ClassifyCode);
            sb.AppendFormat(",\"ClassifyFullPath\":\"{0}\"", this.ClassifyFullPath);
            sb.AppendFormat(",\"NominalVoltage\":\"{0}\"", this.NominalVoltage);
            sb.AppendLine("}");

            return sb.ToString();
        }

        string encodeJson(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            else
            {
                var result = str.Replace("'", "\\'");
                return result;
            }
        }

        public static string ToJsonString(List<CsgiiBasicInfo> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    sb.AppendFormat("{0},",item.ToJsonString());
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}
