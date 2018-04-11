

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
) 
) ON [PRIMARY]

GO




CREATE NONCLUSTERED INDEX [IX_PRW_Inte_SCADA_EFileData1] ON [dbo].[PRW_Inte_SCADA_EFileData] 
(
	[CreateDate] ASC
)
GO



CREATE NONCLUSTERED INDEX [IX_PRW_Inte_SCADA_EFileData2] ON [dbo].[PRW_Inte_SCADA_EFileData] 
(
	[CreateTime] ASC
)
GO




CREATE NONCLUSTERED INDEX [IX_PRW_Inte_SCADA_EFileData3] ON [dbo].[PRW_Inte_SCADA_EFileData] 
(
	[MeasurePointId] ASC
)
GO



