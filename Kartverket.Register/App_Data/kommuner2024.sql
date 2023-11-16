
UPDATE [kartverket_register].[dbo].[RegisterItems]
SET statusId= 'Retired'
  where registerId='F99D6D80-32E1-4935-8464-601D90741443'
  and GETDATE() > ValidToDate and statusId = 'Valid'


UPDATE [kartverket_register].[dbo].[RegisterItems]
SET statusId= 'Valid'
  where registerId='F99D6D80-32E1-4935-8464-601D90741443'
  and GETDATE() > ValidFromDate  and statusId = 'Draft'


--For test
--update [kartverket_register].[dbo].[RegisterItems]
--set ValidFromDate = '11/16/2023'
--  where registerId='F99D6D80-32E1-4935-8464-601D90741443'
--  and ValidFromDate = '1/1/2024'


 -- update [kartverket_register].[dbo].[RegisterItems]
 -- set ValidToDate = '11/15/2023'
 -- where registerId='F99D6D80-32E1-4935-8464-601D90741443'
--  and ValidToDate = '12/31/2023'
