USE [WizPlant_XMH]
GO

/****** Object:  Table [dbo].[PRW_SCADA_EFileDataHistory]    Script Date: 04/10/2016 14:52:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRW_SCADA_EFileDataHistory](
	[Id] [uniqueidentifier] NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[SiteName] [nvarchar](100) NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
	[MeasureValue] [varchar](50) NULL,
	[Status] [bit] NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PK_PRW_SCADA_EFileDataHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


