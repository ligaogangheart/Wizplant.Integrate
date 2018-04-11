update PRW_Inte_VideoMonitor_CameraMap set objectId=b.Id from PRW_Inte_VideoMonitor_CameraMap a inner join
WizPlant_XMH.dbo.PRW_Object b on a.Name=b.Name and a.Context=b.Context and a.Revision=b.Revision
where b.Status=0
