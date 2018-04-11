update [PRW_Inte_AnyShare_DirMap] set objectId=null;

update [PRW_Inte_AnyShare_DirMap] set objectId=b.Id from [PRW_Inte_AnyShare_DirMap] a inner join
WizPlant_XMH.dbo.PRW_Object b on a.Name=b.Name and a.Context=b.Context and a.Revision=b.Revision
where b.Status=0;
