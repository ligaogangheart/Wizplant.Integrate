

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
)) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_PRW_Inte_Csgii_TechInfo] ON [dbo].[PRW_Inte_Csgii_TechInfo] 
(
	[DeviceId] ASC
)
GO





