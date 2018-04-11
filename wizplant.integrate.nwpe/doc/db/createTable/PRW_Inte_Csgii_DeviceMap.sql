

CREATE TABLE [dbo].[PRW_Inte_Csgii_DeviceMap](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Context] [nvarchar](500) NOT NULL,
	[Revision] [nvarchar](100) NOT NULL,
	[DeviceId] [varchar](32) NOT NULL,
	[DeviceName] [nvarchar](100) NOT NULL,
	[ObjectId] [uniqueidentifier] Null,
 CONSTRAINT [PK_PRW_Inte_Csgii_DeviceMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]

GO


CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_DeviceMap] ON [dbo].[PRW_Inte_Csgii_DeviceMap] 
(
	[Name] ASC
)
INCLUDE(
[Context] ,
[Revision] 
)
GO


CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_DeviceMap1] ON [dbo].[PRW_Inte_Csgii_DeviceMap] 
(
	[ObjectId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_DeviceMap2] ON [dbo].[PRW_Inte_Csgii_DeviceMap] 
(
	[DeviceId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_DeviceMap3] ON [dbo].[PRW_Inte_Csgii_DeviceMap] 
(
	[DeviceName] ASC
)
GO






