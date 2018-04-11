using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.SBTZSOA
{
    public partial class DeviceGisDTO
    {
        public string ToJsonString()
        {
            //return base.ToString();
            
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendLine("\"baseInfo\":");
            sb.AppendLine("{");
            sb.AppendFormat("\"deviceCode\":\"{0}\"", this.deviceCode);
            sb.AppendFormat(",\"deviceName\":\"{0}\"", this.deviceName);
            sb.AppendFormat(",\"classifyId\":\"{0}\"", this.classifyId);
            sb.AppendFormat(",\"classifyName\":\"{0}\"", this.classifyName);           
            sb.AppendFormat(",\"isCapitalAssets\":\"{0}\"", this.isCapitalAssets);
            sb.AppendFormat(",\"isCapitalAssetsStr\":\"{0}\"", this.isCapitalAssetsStr);
            sb.AppendFormat(",\"proprietorCompanyOname\":\"{0}\"", this.proprietorCompanyOname);
            sb.AppendFormat(",\"proprietorCompanyOid\":\"{0}\"", this.proprietorCompanyOid);
            sb.AppendFormat(",\"baseVoltageId\":\"{0}\"", this.baseVoltageId);
            sb.AppendFormat(",\"baseVoltageName\":\"{0}\"", this.baseVoltageName);
            sb.AppendFormat(",\"voltagePageShow\":\"{0}\"", this.voltagePageShow);
            sb.AppendFormat(",\"isVirtualDevice\":\"{0}\"", this.isVirtualDevice);
            sb.AppendFormat(",\"isVirtualDeviceStr\":\"{0}\"", this.isVirtualDeviceStr);
            sb.AppendFormat(",\"measurementUnit\":\"{0}\"", this.measurementUnit);
            sb.AppendFormat(",\"amount\":\"{0}\"", this.amount);
            sb.AppendFormat(",\"manufacturer\":\"{0}\"", this.manufacturer);
            sb.AppendFormat(",\"manufacturerId\":\"{0}\"", this.manufacturerId);
            sb.AppendFormat(",\"latestManufacturer\":\"{0}\"", this.latestManufacturer);
            sb.AppendFormat(",\"vendor\":\"{0}\"", this.vendor);
            sb.AppendFormat(",\"vendorId\":\"{0}\"", this.vendorId);
            sb.AppendFormat(",\"deviceModel\":\"{0}\"", this.deviceModel);
            sb.AppendFormat(",\"leaveFactoryNo\":\"{0}\"", this.leaveFactoryNo);
           // sb.AppendFormat(",\"leaveFactoryDate\":\"{0}\"", this.leaveFactoryDate);
            sb.AppendFormat(",\"leaveFactoryDateStr\":\"{0}\"", this.leaveFactoryDateStr);
            sb.AppendFormat(",\"warrantyPeriod\":\"{0}\"", this.warrantyPeriod);
           // sb.AppendFormat(",\"plantTransferDate\":\"{0}\"", this.plantTransferDate);
            sb.AppendFormat(",\"plantTransferDateStr\":\"{0}\"", this.plantTransferDateStr);
            sb.AppendFormat(",\"assetState\":\"{0}\"", this.assetState);
            sb.AppendFormat(",\"assetStateStr\":\"{0}\"", this.assetStateStr);
            //sb.AppendFormat(",\"statusDate\":\"{0}\"", this.statusDate);
            sb.AppendFormat(",\"statusDateStr\":\"{0}\"", this.statusDateStr);
            sb.AppendFormat(",\"runningCode\":\"{0}\"", this.runningCode);
            sb.AppendFormat(",\"latitude\":\"{0}\"", this.latitude);
            sb.AppendFormat(",\"longitude\":\"{0}\"", this.longitude);
            sb.AppendFormat(",\"latitudeStr\":\"{0}\"", this.latitudeStr);
            sb.AppendFormat(",\"longitudeStr\":\"{0}\"", this.longitudeStr);
            sb.AppendFormat(",\"altitude\":\"{0}\"", this.altitude);
            sb.AppendFormat(",\"topography\":\"{0}\"", this.topography);
            sb.AppendFormat(",\"topographyStr\":\"{0}\"", this.topographyStr);
            sb.AppendFormat(",\"bureauUnitsOid\":\"{0}\"", this.bureauUnitsOid);
            sb.AppendFormat(",\"bureauUnitsOname\":\"{0}\"", this.bureauUnitsOname);
            sb.AppendFormat(",\"dispatchLevel\":\"{0}\"", this.dispatchLevel);
            sb.AppendFormat(",\"dispatchLevelStr\":\"{0}\"", this.dispatchLevelStr);
            sb.AppendFormat(",\"runmanageOid\":\"{0}\"", this.runmanageOid);
            sb.AppendFormat(",\"runmanageOName\":\"{0}\"", this.runmanageOName);
            sb.AppendFormat(",\"vindicateOid\":\"{0}\"", this.vindicateOid);
            sb.AppendFormat(",\"vindicateOName\":\"{0}\"", this.vindicateOName);
            sb.AppendFormat(",\"placeCode\":\"{0}\"", this.placeCode);
            sb.AppendFormat(",\"flName\":\"{0}\"", this.flName);
            sb.AppendFormat(",\"flCode\":\"{0}\"", this.flCode);
            sb.AppendFormat(",\"installCode\":\"{0}\"", this.installCode);
            sb.AppendFormat(",\"remark\":\"{0}\"", this.remark);
            sb.AppendFormat(",\"powerGridFlag\":\"{0}\"", this.powerGridFlag);
            sb.AppendFormat(",\"powerGridFlagStr\":\"{0}\"", this.powerGridFlagStr);
            sb.AppendFormat(",\"ownerStationOid\":\"{0}\"", this.ownerStationOid);
            sb.AppendFormat(",\"ownerStationOName\":\"{0}\"", this.ownerStationOName);
            sb.AppendFormat(",\"countyCode\":\"{0}\"", this.countyCode);
            sb.AppendFormat(",\"oldClassifyFullPath\":\"{0}\"", this.oldClassifyFullPath);
            sb.AppendFormat(",\"oldAliasName\":\"{0}\"", this.oldAliasName);
            //sb.AppendFormat(",\"retireDate\":\"{0}\"", this.retireDate);
            sb.AppendFormat(",\"isLabel\":\"{0}\"", this.isLabel);
            sb.AppendFormat(",\"isLabelStr\":\"{0}\"", this.isLabelStr);
            sb.AppendFormat(",\"isAssambly\":\"{0}\"", this.isAssambly);
            sb.AppendFormat(",\"isAssamblyStr\":\"{0}\"", this.isAssamblyStr);
            sb.AppendFormat(",\"isShareDevice\":\"{0}\"", this.isShareDevice);
            sb.AppendFormat(",\"isShareDeviceStr\":\"{0}\"", this.isShareDeviceStr);
            sb.AppendFormat(",\"dataSource\":\"{0}\"", this.dataSource);
            sb.AppendFormat(",\"licensePlateNumber\":\"{0}\"", this.licensePlateNumber);
            sb.AppendFormat(",\"vindicatorUnitOname\":\"{0}\"", this.vindicatorUnitOname);
            sb.AppendFormat(",\"vindicatorUnitOid\":\"{0}\"", this.vindicatorUnitOid);
            sb.AppendFormat(",\"capitalAssetsCode\":\"{0}\"", this.capitalAssetsCode);
            sb.AppendFormat(",\"installationLocation\":\"{0}\"", this.installationLocation);
            sb.AppendFormat(",\"proprietorOwner\":\"{0}\"", this.proprietorOwner);
            sb.AppendFormat(",\"proprietorOwnerStr\":\"{0}\"", this.proprietorOwnerStr);
            sb.AppendFormat(",\"useLife\":\"{0}\"", this.useLife);
            sb.AppendFormat(",\"deviceModelId\":\"{0}\"", this.deviceModelId);
            sb.AppendFormat(",\"deviceId\":\"{0}\"", this.deviceId);
            sb.AppendFormat(",\"retireDateStr\":\"{0}\"", this.retireDateStr);
            sb.AppendFormat(",\"functionLocationId\":\"{0}\"", this.functionLocationId);
            sb.AppendFormat(",\"classifyCode\":\"{0}\"", this.classifyCode);
            sb.AppendFormat(",\"classifyFullPath\":\"{0}\"", this.classifyFullPath);
            sb.AppendFormat(",\"nominalVoltage\":\"{0}\"", this.nominalVoltage);
            sb.AppendFormat(",\"parentFLId\":\"{0}\"", this.parentFLId);
            sb.AppendFormat(",\"parentFLName\":\"{0}\"", this.parentFLName);
            sb.AppendFormat(",\"bayId\":\"{0}\"", this.bayId);
            sb.AppendFormat(",\"bayName\":\"{0}\"", this.bayName);
            sb.AppendFormat(",\"siteId\":\"{0}\"", this.siteId);
            sb.AppendFormat(",\"siteName\":\"{0}\"", this.siteName);
            sb.AppendFormat(",\"bureauName\":\"{0}\"", this.bureauName);
            sb.AppendFormat(",\"fullPath\":\"{0}\"", this.fullPath);
            sb.AppendFormat(",\"centerSubstationId\":\"{0}\"", this.centerSubstationId);
            sb.AppendFormat(",\"centerSubstationName\":\"{0}\"", this.centerSubstationName);
            sb.AppendFormat(",\"subsIdOfDisDevice\":\"{0}\"", this.subsIdOfDisDevice);
            sb.AppendFormat(",\"subsNameOfDisDevice\":\"{0}\"", this.subsNameOfDisDevice);
            sb.AppendFormat(",\"inventoryTaskNo\":\"{0}\"", this.inventoryTaskNo);
            sb.AppendFormat(",\"assetDeviceId\":\"{0}\"", this.assetDeviceId);
            sb.AppendLine("}");
            sb.AppendLine(",\"techParams\":");
            sb.AppendLine("[");
            int i = 0;
            if (this.lstTechParamList != null)
            {
                foreach (var item in this.lstTechParamList)
                {

                    if (++i == 1)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"techParamName\":\"{0}\",\"techParamValue\":\"{1}\"", item.techParamName, item.techParamValue);
                    }
                    else
                    {
                        sb.Append(",{");
                        sb.AppendFormat("\"techParamName\":\"{0}\",\"techParamValue\":\"{1}\"", item.techParamName, item.techParamValue);
                    }
                    sb.AppendLine("}");
                }
            }
            sb.AppendLine("]");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
