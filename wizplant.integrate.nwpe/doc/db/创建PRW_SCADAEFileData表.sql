

CREATE TABLE [dbo].[PRW_Inte_SCADA_EFile1](
	[Id] [uniqueidentifier] NOT NULL,
	[CreateDate] int not null,
	[CreateTime] char(6) not null,
	[SiteName] [nvarchar](100) NOT NULL,
	[MeasurePointId] [varchar](50) NOT NULL,
	[MeasurePointName] [nvarchar](100) NOT NULL,
	[MeasureValue] [varchar](50) NULL,
	[Status] [bit] NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PRW_Inte_SCADA_EFile1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]



