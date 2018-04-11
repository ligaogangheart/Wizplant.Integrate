
CREATE TABLE [dbo].[PRW_Inte_Csgii_BasicInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [varchar](50) NOT NULL,
	[deviceCode] [nvarchar](64) NOT NULL,
	[deviceName] [nvarchar](260) NOT NULL,
	[classifyId] [varchar](10) NULL,
	[classifyName] [nvarchar](32) NULL,
	[isCapitalAssets] [bit] NULL,
	[isCapitalAssetsStr] [nvarchar](2) NULL,
	[proprietorCompanyOname] [nvarchar](200) NULL,
	[proprietorCompanyOid] [varchar](32) NULL,
	[baseVoltageId] [varchar](32) NULL,
	[voltagePageShow] [varchar](32) NULL,
	[isVirtualDevice] [bit] NULL,
	[isVirtualDeviceStr] [nvarchar](2) NULL,
	[amount] [int] NULL,
	[manufacturerId] [varchar](32) NULL,
	[manufacturer] [nvarchar](100) NULL,
	[latestManufacturer] [nvarchar](100) NULL,
	[vendor] [nvarchar](100) NULL,
	[vendorId] [varchar](32) NULL,
	[deviceModel] [nvarchar](100) NULL,
	[leaveFactoryNo] [varchar](64) NULL,
	[leaveFactoryDate] [datetime] NULL,
	[warrantyPeriod] [int] NULL,
	[plantTransferDate] [datetime] NULL,
	[assetState] [int] NULL,
	[assetStateStr] [nvarchar](100) NULL,
	[statusDate] [nvarchar](100) NULL,
	[latitude] [numeric](18, 15) NULL,
	[longitude] [numeric](18, 15) NULL,
	[latitudeStr] [varchar](100) NULL,
	[longitudeStr] [varchar](100) NULL,
	[altitude] [int] NULL,
	[topography] [int] NULL,
	[bureauUnitsOid] [varchar](32) NULL,
	[bureauUnitsOname] [nvarchar](500) NULL,
	[dispatchLevel] [int] NULL,
	[dispatchLevelStr] [nvarchar](100) NULL,
	[runmanageOid] [varchar](32) NULL,
	[runmanageOName] [nvarchar](500) NULL,
	[vindicateOid] [varchar](500) NULL,
	[vindicateOName] [nvarchar](200) NULL,
	[flName] [nvarchar](200) NULL,
	[remark] [nvarchar](200) NULL,
	[powerGridFlag] [int] NULL,
	[isLabel] [bit] NULL,
	[isAssambly] [bit] NULL,
	[isShareDevice] [bit] NULL,
	[dataSource] [nvarchar](20) NULL,
	[proprietorOwner] [nvarchar](200) NULL,
	[useLife] [int] NULL,
	[deviceModelId] [varchar](32) NULL,
	[classifyCode] [varchar](32) NULL,
	[classifyFullPath] [nvarchar](500) NULL,
	[nominalVoltage] [varchar](32) NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_BasicInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_BasicInfo] ON [dbo].[PRW_Inte_Csgii_BasicInfo] 
(
	[DeviceId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_BasicInfo1] ON [dbo].[PRW_Inte_Csgii_BasicInfo] 
(
	[deviceName] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_BasicInfo2] ON [dbo].[PRW_Inte_Csgii_BasicInfo] 
(
	[voltagePageShow] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_BasicInfo3] ON [dbo].[PRW_Inte_Csgii_BasicInfo] 
(
	[manufacturer] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_BasicInfo4] ON [dbo].[PRW_Inte_Csgii_BasicInfo] 
(
	[plantTransferDate] ASC
)
GO










