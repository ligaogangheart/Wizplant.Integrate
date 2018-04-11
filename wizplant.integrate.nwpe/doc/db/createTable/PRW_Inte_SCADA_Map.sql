

CREATE TABLE [dbo].[PRW_Inte_SCADA_Map](
	[Id] [uniqueidentifier] NOT NULL,
	[TagId] [varchar](50) NOT NULL,
	[TagName] [nvarchar](100) NOT NULL,
	[TagType] nvarchar(100) NOT null,
	[Name] [nvarchar](500) NULL,
	[Name2] [nvarchar](500) NULL,
	[Context] [nvarchar](255) NOT NULL,
	[Revision] [varchar](50) NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[ObjectId] [uniqueidentifier] null,
 CONSTRAINT [PK_PRW_Inte_SCADA_Map] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_SCADA_Map1] ON [dbo].[PRW_Inte_SCADA_Map] 
(
	[TagId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_SCADA_Map2] ON [dbo].[PRW_Inte_SCADA_Map] 
(
	[Name] ASC	
)
INCLUDE ( [Context],
[Revision]
)
GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_SCADA_Map3] ON [dbo].[PRW_Inte_SCADA_Map] 
(
	[TagName] ASC
)
GO


