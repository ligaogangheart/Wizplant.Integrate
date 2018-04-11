using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public class CsgiiBasicInfoOfflineDto
    {
        public string ObjId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string ClassifyId { get; set; }
        public string ClassifyName { get; set; }
        public bool? IsCapitalAssets { get; set; }
        public string IsCapitalAssetsStr { get; set; }
        public string ProprietorCompanyOname { get; set; }
        public string ProprietorCompanyOid { get; set; }
        public string BaseVoltageId { get; set; }
        public string VoltagePageShow { get; set; }
        public bool? IsVirtualDevice { get; set; }
        public string IsVirtualDeviceStr { get; set; }
        public int? Amount { get; set; }
        public string ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public string LatestManufacturer { get; set; }
        public string Vendor { get; set; }
        public string VendorId { get; set; }
        public string SeviceModel { get; set; }
        public string LeaveFactoryNo { get; set; }
        public DateTime? LeaveFactoryDate { get; set; }
        public int? WarrantyPeriod { get; set; }
        public DateTime? PlantTransferDate { get; set; }
        public int? AssetState { get; set; }
        public string AssetStateStr { get; set; }
        public string StatusDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LatitudeStr { get; set; }
        public string LongitudeStr { get; set; }
        public int? Altitude { get; set; }
        public int? Topography { get; set; }
        public string BureauUnitsOid { get; set; }
        public string BureauUnitsOname { get; set; }
        public int? DispatchLevel { get; set; }
        public string DispatchLevelStr { get; set; }
        public string RunmanageOid { get; set; }
        public string RunmanageOName { get; set; }
        public string VindicateOid { get; set; }
        public string VindicateOName { get; set; }
        public string FlName { get; set; }
        public string Remark { get; set; }
        public int? PowerGridFlag { get; set; }
        public bool? IsLabel { get; set; }
        public bool? IsAssambly { get; set; }
        public bool? IsShareDevice { get; set; }
        public string DataSource { get; set; }
        public string ProprietorOwner { get; set; }
        public int? UseLife { get; set; }
        public string DeviceModelId { get; set; }
        public string ClassifyCode { get; set; }
        public string ClassifyFullPath { get; set; }
        public string NominalVoltage { get; set; }
    }
}
