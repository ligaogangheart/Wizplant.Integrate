if exists (select 1
            from  sysobjects
           where  id = object_id('PRW_Inte_Csgii_YXQXJL')
            and   type = 'U')
   drop table PRW_Inte_Csgii_YXQXJL
go

/*==============================================================*/
/* Table: PRW_Inte_Csgii_YXQXJL                                 */
/*==============================================================*/
create table PRW_Inte_Csgii_YXQXJL (
   Id                   uniqueidentifier     not null,
   DeviceId             varchar(50)          not null,
   QXBH                 varchar(32)          not null,
   QXLY                 nvarchar(200)        null,
   QXDJ                 nvarchar(200)        not null,
   QXBX                 nvarchar(200)        null,
   QXBW                 nvarchar(200)        null,
   QXLX                 nvarchar(200)        null,
   QXYY                 nvarchar(200)        null,
   QXZT                 nvarchar(200)        null,
   FXSJ                 datetime             not null,
   XQSJ                 datetime             null,
   constraint PK_PRW_INTE_CSGII_YXQXJL primary key (Id)
)
go
