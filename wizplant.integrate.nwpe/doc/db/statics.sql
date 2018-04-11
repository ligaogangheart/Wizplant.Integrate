if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_ValueStatistics')
            and   name  = 'idx_003'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_ValueStatistics.idx_003
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_ValueStatistics')
            and   name  = 'idx_002'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_ValueStatistics.idx_002
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_ValueStatistics')
            and   name  = 'idx_001'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_ValueStatistics.idx_001
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PRW_Inte_SCADA_ValueStatistics')
            and   type = 'U')
   drop table PRW_Inte_SCADA_ValueStatistics
go

/*==============================================================*/
/* Table: PRW_Inte_SCADA_ValueStatistics                        */
/*==============================================================*/
create table PRW_Inte_SCADA_ValueStatistics (
   Id                   uniqueidentifier     not null,
   SiteName             varchar(50)          not null,
   ReportDate           int                  not null,
   TagNo                varchar(50)          not null,
   TagDesc              varchar(500)         not null,
   MaxValue             real                 not null,
   MaxTime              datetime             not null,
   MinValue             real                 not null,
   MinTime              datetime             not null,
   TotalCount           int                  not null,
   AvgValue             real                 not null,
   CreateTime           datetime             not null,
   constraint PK_PRW_INTE_SCADA_VALUESTATIST primary key (Id)
)
go

/*==============================================================*/
/* Index: idx_001                                               */
/*==============================================================*/
create index idx_001 on PRW_Inte_SCADA_ValueStatistics (
SiteName ASC
)
go

/*==============================================================*/
/* Index: idx_002                                               */
/*==============================================================*/
create index idx_002 on PRW_Inte_SCADA_ValueStatistics (
ReportDate ASC
)
go

/*==============================================================*/
/* Index: idx_003                                               */
/*==============================================================*/
create index idx_003 on PRW_Inte_SCADA_ValueStatistics (
TagNo ASC
)
go



if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_SkipCount')
            and   name  = 'idx_004'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_SkipCount.idx_004
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_SkipCount')
            and   name  = 'idx_003'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_SkipCount.idx_003
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_SkipCount')
            and   name  = 'idx_002'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_SkipCount.idx_002
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PRW_Inte_SCADA_SkipCount')
            and   name  = 'idx_001'
            and   indid > 0
            and   indid < 255)
   drop index PRW_Inte_SCADA_SkipCount.idx_001
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PRW_Inte_SCADA_SkipCount')
            and   type = 'U')
   drop table PRW_Inte_SCADA_SkipCount
go

/*==============================================================*/
/* Table: PRW_Inte_SCADA_SkipCount                              */
/*==============================================================*/
create table PRW_Inte_SCADA_SkipCount (
   Id                   uniqueidentifier     not null,
   Month                int                  not null,
   SiteName             varchar(50)          not null,
   TagNo                varchar(50)          not null,
   TagDesc              varchar(500)         not null,
   MesureType          int          not null,
   SkipCount            int                  not null,
   CreateTime           datetime             not null,
   constraint PK_PRW_INTE_SCADA_SKIPCOUNT primary key (Id)
)
go

/*==============================================================*/
/* Index: idx_001                                               */
/*==============================================================*/
create index idx_001 on PRW_Inte_SCADA_SkipCount (
Month ASC
)
go

/*==============================================================*/
/* Index: idx_002                                               */
/*==============================================================*/
create index idx_002 on PRW_Inte_SCADA_SkipCount (
SiteName ASC
)
go

/*==============================================================*/
/* Index: idx_003                                               */
/*==============================================================*/
create index idx_003 on PRW_Inte_SCADA_SkipCount (
MeasureType ASC
)
go

/*==============================================================*/
/* Index: idx_004                                               */
/*==============================================================*/
create index idx_004 on PRW_Inte_SCADA_SkipCount (
TagNo ASC
)
go
