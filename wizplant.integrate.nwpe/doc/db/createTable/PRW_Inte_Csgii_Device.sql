

CREATE TABLE [dbo].[PRW_Inte_Csgii_Device](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [varchar](50) NOT NULL,
	[Station] [varchar](100) NOT NULL,
	[DeviceName] [nvarchar](100) NOT NULL,
	[FullPath] [nvarchar](500) NOT NULL,
	[ClassPath] [nvarchar](500) NOT NULL,
	[LevelType] [int] NOT NULL,
	[Status] int NOT NULL,
 CONSTRAINT [PK_PRW_Inte_Csgii_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_Device] ON [dbo].[PRW_Inte_Csgii_Device] 
(
	[DeviceId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_Device1] ON [dbo].[PRW_Inte_Csgii_Device] 
(
	[DeviceName] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_Device2] ON [dbo].[PRW_Inte_Csgii_Device] 
(
	[LevelType] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_Device3] ON [dbo].[PRW_Inte_Csgii_Device] 
(
	[Status] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_Device4] ON [dbo].[PRW_Inte_Csgii_Device] 
(
	[Station] ASC
)
GO





