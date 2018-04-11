if exists (select 1
            from  sysobjects
           where  id = object_id('PRW_Inte_Csgii_CSBG')
            and   type = 'U')
   drop table PRW_Inte_Csgii_CSBG
go

/*==============================================================*/
/* Table: PRW_Inte_Csgii_CSBG                                   */
/*==============================================================*/
create table PRW_Inte_Csgii_CSBG (
   Id                   uniqueidentifier     not null,
   DeviceId             varchar(50)          not null,
   CSBGMC               varchar(32)          not null,
   RQ                   datetime        not null,
   ZY                   nvarchar(200)        null,
   XZ                   nvarchar(200)        null,
   JL                   nvarchar(200)        null,
   constraint PK_PRW_INTE_CSGII_CSBG primary key (Id)
)
go
