USE [WizIntegrate_NW]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_BasicInfo]    Script Date: 06/08/2016 10:13:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_NW]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_Device]    Script Date: 06/08/2016 10:13:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_Csgii_Device](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [varchar](50) NOT NULL,
	[Station] [varchar](100) NOT NULL,
	[DeviceName] [nvarchar](100) NOT NULL,
	[FullPath] [nvarchar](500) NOT NULL,
	[ClassPath] [nvarchar](500) NOT NULL,
	[LevelType] [int] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_NW]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_DeviceMap]    Script Date: 06/08/2016 10:13:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_Csgii_DeviceMap](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Context] [nvarchar](500) NOT NULL,
	[Revision] [nvarchar](100) NOT NULL,
	[DeviceId] [varchar](32) NOT NULL,
	[DeviceName] [nvarchar](100) NOT NULL,
	[ObjectId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_DeviceMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_NW]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_TechInfo]    Script Date: 06/08/2016 10:13:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_Csgii_TechInfo](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [varchar](50) NOT NULL,
	[classifyId] [varchar](32) NOT NULL,
	[techParamId] [varchar](32) NOT NULL,
	[columnName] [varchar](50) NOT NULL,
	[techParamName] [nvarchar](100) NOT NULL,
	[isMandatory] [bit] NOT NULL,
	[isShow] [bit] NOT NULL,
	[sortNo] [int] NOT NULL,
	[dataType] [int] NOT NULL,
	[isVendorFill] [bit] NOT NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_TechInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_NW]
GO

/****** Object:  Table [dbo].[PRW_Inte_SCADA_EFileData]    Script Date: 06/08/2016 10:13:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_SCADA_EFileData](
	[Id] [uniqueidentifier] NOT NULL,
	[CreateDate] [int] NOT NULL,
	[CreateTime] [char](6) NOT NULL,
	[SiteName] [nvarchar](100) NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
	[MeasureValue] [varchar](50) NULL,
	[Status] [bit] NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PK_PRW_Inte_SCADA_EFileData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_NW]
GO

/****** Object:  Table [dbo].[PRW_Inte_SCADA_Map]    Script Date: 06/08/2016 10:13:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_SCADA_Map](
	[Id] [uniqueidentifier] NOT NULL,
	[TagId] [varchar](50) NOT NULL,
	[TagName] [nvarchar](100) NOT NULL,
	[TagType] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Name2] [nvarchar](500) NULL,
	[Context] [nvarchar](255) NOT NULL,
	[Revision] [varchar](50) NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[ObjectId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PRW_Inte_SCADA_Map] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

