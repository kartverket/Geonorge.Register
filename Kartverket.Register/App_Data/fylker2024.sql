
UPDATE [kartverket_register].[dbo].[RegisterItems]
SET statusId= 'Retired'
  where registerId='152EE358-CE3D-40F7-B7EE-85195B40A35A'
  and GETDATE() > ValidToDate and statusId = 'Valid'


UPDATE [kartverket_register].[dbo].[RegisterItems]
SET statusId= 'Valid'
  where registerId='152EE358-CE3D-40F7-B7EE-85195B40A35A'
  and GETDATE() > ValidFromDate  and statusId = 'Draft'


--For test
-- update [kartverket_register].[dbo].[RegisterItems]
-- set ValidFromDate = '11/16/2023'
--   where registerId='152EE358-CE3D-40F7-B7EE-85195B40A35A'
--   and ValidFromDate = '1/1/2024'


--  update [kartverket_register].[dbo].[RegisterItems]
--   set ValidToDate = '11/15/2023'
--   where registerId='152EE358-CE3D-40F7-B7EE-85195B40A35A'
--   and ValidToDate = '12/31/2023'
