if exists (select 1
            from  sysobjects
           where  id = object_id('PRW_Inte_Csgii_ZTPJ')
            and   type = 'U')
   drop table PRW_Inte_Csgii_ZTPJ
go

/*==============================================================*/
/* Table: PRW_Inte_Csgii_ZTPJ                                   */
/*==============================================================*/
create table PRW_Inte_Csgii_ZTPJ (
   Id                   uniqueidentifier     not null,
   DeviceId             varchar(50)          not null,
   PJRQ                 datetime          not null,
   SBJKD                nvarchar(200)        null,
   SBZYD                nvarchar(200)        not null,
   SBYWJB               nvarchar(200)        null,
   constraint PK_PRW_INTE_CSGII_ZTPJ primary key (Id)
)
go
