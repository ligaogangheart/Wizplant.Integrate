
-- WizPlantӦ��Id
declare @appId varchar(36)
set @appId = 'c74dcaf0-93de-4c77-9f80-a0170cbdaf7f'

-- Ҫ��Ȩ��Ӧ�ý�ɫId
declare @appRoleId varchar(36)
-- ��ͨ�û�Id
set @appRoleId = '9b840a90-ef6a-4ba7-b1cc-44682a32413b'

-- ����ԱId
declare @userId varchar(36)
set @userId = 'bc4ba096-ac22-4f68-b43d-9499679ede7a'

-- ��ǰʱ��
declare @currentTime datetime
set @currentTime = GETDATE()

-- �����⹦��Id
declare @cjkFunId  varchar(36)
set @cjkFunId = '3CF31D64-516D-40FF-B3B9-7B0B45D5BBBA'

-- ����Ѳ�ӹ���Id
declare @xsmyFunId  varchar(36)
set @xsmyFunId = '4E079F53-1CDF-4236-B6B8-8A4701140704'

-- ������ѵ����Id
declare @fzpxFunId  varchar(36)
set @fzpxFunId = 'CFFE757B-38DA-4010-8641-CDA2DC38D7F9'

-- Ҫ��Ȩ�Ĳ���ֵ
declare @actionValue  bigint
-- ��ʾ���в���
set @actionValue = 8589934591



-- ���볡������
insert into SeAppFunction(ID,ApplicationId,ParentId,Name,IsEnd,Description,IsActive,SeqNo,IsOpenNew,Creater,ModifyPerson,CreateDate,ModifyDate)
values
(@cjkFunId,@appId,null,'������',0,'������',1,2,0,@userId,@userId,@currentTime,@currentTime),
(@xsmyFunId,@appId,@cjkFunId,'����Ѳ��',1,'������->����Ѳ��',1,1,0,@userId,@userId,@currentTime,@currentTime),
(@fzpxFunId,@appId,@cjkFunId,'������ѵ',1,'������->������ѵ',1,2,0,@userId,@userId,@currentTime,@currentTime)

--������Ȩ
insert into SeAppRoleFunction(Id,ApplicationId,AppRoleId,FunctionId,ActionValue)
values
(NEWID(),@appId,@appRoleId,@cjkFunId,@actionValue),
(NEWID(),@appId,@appRoleId,@xsmyFunId,@actionValue),
(NEWID(),@appId,@appRoleId,@fzpxFunId,@actionValue)

