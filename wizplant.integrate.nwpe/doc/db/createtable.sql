USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_BasicInfo]    Script Date: 04/17/2016 11:29:35 ******/
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
	[classifyId] [varchar](10) NOT NULL,
	[classifyName] [nvarchar](32) NOT NULL,
	[isCapitalAssets] [bit] NOT NULL,
	[isCapitalAssetsStr] [nvarchar](2) NOT NULL,
	[proprietorCompanyOname] [nvarchar](200) NOT NULL,
	[proprietorCompanyOid] [varchar](32) NOT NULL,
	[baseVoltageId] [varchar](32) NOT NULL,
	[voltagePageShow] [varchar](32) NOT NULL,
	[isVirtualDevice] [bit] NOT NULL,
	[isVirtualDeviceStr] [nvarchar](2) NOT NULL,
	[amount] [int] NOT NULL,
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
	[assetStateStr] [int] NULL,
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
	[vindicateOid] [varchar](32) NULL,
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
	[deviceModelId] [int] NULL,
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

USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_Device]    Script Date: 04/17/2016 11:29:36 ******/
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
	[Path] [nvarchar](500) NOT NULL,
	[LevelType] [int] NOT NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_DeviceMap]    Script Date: 04/17/2016 11:29:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_Csgii_DeviceMap](
	[Id] [uniqueidentifier] NOT NULL,
	[WIZDeviceName] [nvarchar](100) NOT NULL,
	[WIZDeviceContextId] [nvarchar](500) NOT NULL,
	[WIZDeviceRevision] [nvarchar](100) NOT NULL,
	[DeviceId] [varchar](32) NOT NULL,
	[DeviceName] [nvarchar](100) NOT NULL,
	[Path] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_DeviceMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_Csgii_TechInfo]    Script Date: 04/17/2016 11:29:36 ******/
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

USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_SCADA_EFile1]    Script Date: 04/17/2016 11:29:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_SCADA_EFile1](
	[Id] [uniqueidentifier] NOT NULL,
	[CreateDate] [int] NOT NULL,
	[CreateTime] [char](6) NOT NULL,
	[SiteName] [nvarchar](100) NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
	[MeasureValue] [varchar](50) NULL,
	[Status] [bit] NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PK_PRW_Inte_SCADA_EFile1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_SCADA_EFileData]    Script Date: 04/17/2016 11:29:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_SCADA_EFileData](
	[Id] [uniqueidentifier] NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[SiteName] [nvarchar](100) NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
	[MeasureValue] [varchar](50) NOT NULL,
	[Status] [bit] NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PK_PRW_SCADA_EFileData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [WizIntegrate_XMH]
GO

/****** Object:  Table [dbo].[PRW_Inte_SCADA_Tag]    Script Date: 04/17/2016 11:29:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_SCADA_Tag](
	[Id] [uniqueidentifier] NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_PRW_Inte_SCADA_Tag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

