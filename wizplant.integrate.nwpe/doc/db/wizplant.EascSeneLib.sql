
-- WizPlant应用Id
declare @appId varchar(36)
set @appId = 'c74dcaf0-93de-4c77-9f80-a0170cbdaf7f'

-- 要授权的应用角色Id
declare @appRoleId varchar(36)
-- 普通用户Id
set @appRoleId = '9b840a90-ef6a-4ba7-b1cc-44682a32413b'

-- 管理员Id
declare @userId varchar(36)
set @userId = 'bc4ba096-ac22-4f68-b43d-9499679ede7a'

-- 当前时间
declare @currentTime datetime
set @currentTime = GETDATE()

-- 场景库功能Id
declare @cjkFunId  varchar(36)
set @cjkFunId = '3CF31D64-516D-40FF-B3B9-7B0B45D5BBBA'

-- 漫游巡视功能Id
declare @xsmyFunId  varchar(36)
set @xsmyFunId = '4E079F53-1CDF-4236-B6B8-8A4701140704'

-- 仿真培训功能Id
declare @fzpxFunId  varchar(36)
set @fzpxFunId = 'CFFE757B-38DA-4010-8641-CDA2DC38D7F9'

-- 要授权的操作值
declare @actionValue  bigint
-- 表示所有操作
set @actionValue = 8589934591



-- 插入场景功能
insert into SeAppFunction(ID,ApplicationId,ParentId,Name,IsEnd,Description,IsActive,SeqNo,IsOpenNew,Creater,ModifyPerson,CreateDate,ModifyDate)
values
(@cjkFunId,@appId,null,'场景库',0,'场景库',1,2,0,@userId,@userId,@currentTime,@currentTime),
(@xsmyFunId,@appId,@cjkFunId,'漫游巡视',1,'场景库->漫游巡视',1,1,0,@userId,@userId,@currentTime,@currentTime),
(@fzpxFunId,@appId,@cjkFunId,'仿真培训',1,'场景库->仿真培训',1,2,0,@userId,@userId,@currentTime,@currentTime)

--插入授权
insert into SeAppRoleFunction(Id,ApplicationId,AppRoleId,FunctionId,ActionValue)
values
(NEWID(),@appId,@appRoleId,@cjkFunId,@actionValue),
(NEWID(),@appId,@appRoleId,@xsmyFunId,@actionValue),
(NEWID(),@appId,@appRoleId,@fzpxFunId,@actionValue)

