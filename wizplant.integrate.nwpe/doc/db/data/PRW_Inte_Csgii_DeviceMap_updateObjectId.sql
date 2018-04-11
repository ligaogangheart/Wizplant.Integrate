update PRW_Inte_Csgii_DeviceMap set ObjectId =b.Id 
from PRW_Inte_Csgii_DeviceMap a inner join WizPlant_XMH.dbo.PRW_Object b 
on a.Name=b.Name and a.Context=b.Context and a.Revision=b.Revision
where b.Status=0


select * from PRW_Inte_Csgii_DeviceMap where ObjectId is not null


select a.* from PRW_Inte_Csgii_DeviceMap a inner join WizPlant_XMH.dbo.PRW_Object b 
on a.Name=b.Name and a.Context=b.Context and a.Revision=b.Revision
where b.Status=0


select * from PRW_Inte_Csgii_BasicInfo where DeviceId='3ee2d3d219004015999135c7b53669d3'

select * from PRW_Inte_Csgii_Device where DeviceId='3ee2d3d219004015999135c7b53669d3'
