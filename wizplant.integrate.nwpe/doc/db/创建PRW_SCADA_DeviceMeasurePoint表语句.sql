USE [WizPlant_XMH]
GO

/****** Object:  Table [dbo].[[PRW_Inte_SCADA_Tag]]    Script Date: 04/11/2016 11:38:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_Inte_SCADA_Map](
	[Id] [uniqueidentifier] NOT NULL,		
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,	
	[Name] [nvarchar](500) NULL,
	[Name2] [nvarchar](500) NULL,
	[Context] [nvarchar](255) NOT NULL,
	[Revision] [varchar](50) NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PK_PRW_Inte_SCADA_Map] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[PRW_Inte_SCADA_Map](
	[Id] [uniqueidentifier] NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[SiteCode] [varchar](50) NOT NULL,
	[SiteNo] [int] NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
	[DataType] [nvarchar](50) NOT NULL,
	[DataTypeIdentificationNo] [int] NOT NULL,
	[DataTypeIdentificationCode] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Context] [nvarchar](255) NOT NULL,
	[Revision] [varchar](50) NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PRW_Inte_SCADA_Tag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


