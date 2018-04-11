update PRW_Inte_SCADA_Map set ObjectId =b.Id 
from PRW_Inte_SCADA_Map a 
inner join WizPlant_XMH.dbo.PRW_Object b on a.Name=b.Name and a.Context=b.Context and a.Revision=b.Revision
where b.Status=0

