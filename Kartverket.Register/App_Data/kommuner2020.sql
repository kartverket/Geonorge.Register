-- Set on cascade update

ALTER TABLE RegisterItems
   DROP CONSTRAINT [FK_dbo.RegisterItems_dbo.Registers_registerId] 

ALTER TABLE RegisterItems
   ADD CONSTRAINT [FK_dbo.RegisterItems_dbo.Registers_registerId]
   FOREIGN KEY (registerId) REFERENCES Registers(systemId) 
   ON DELETE CASCADE
   ON UPDATE CASCADE

ALTER TABLE RegisterTranslations
   DROP CONSTRAINT [FK_dbo.RegisterTranslations_dbo.Registers_RegisterId]

ALTER TABLE RegisterTranslations
   ADD CONSTRAINT [FK_dbo.RegisterTranslations_dbo.Registers_RegisterId]
   FOREIGN KEY (registerId) REFERENCES Registers(systemId) 
   ON DELETE CASCADE
   ON UPDATE CASCADE


--Fylkesnummer
update Registers
set Registers.systemId = 'cd692fdc-31e0-4836-8ac7-a05491bc2d48'
where Registers.systemId = 'cd692fdc-31e0-4836-8ac7-a05491bc2d47'

update Registers
set Registers.systemId = 'cd692fdc-31e0-4836-8ac7-a05491bc2d47'
where Registers.systemId = 'd96bcbd6-d6ce-4fd4-a999-102051685aec'

update Registers
set Registers.systemId = 'd96bcbd6-d6ce-4fd4-a999-102051685aec'
where Registers.systemId = 'cd692fdc-31e0-4836-8ac7-a05491bc2d48'

update Registers
set Registers.name = 'Fylkesnummer', seoname='fylkesnummer'
where Registers.systemId = 'D96BCBD6-D6CE-4FD4-A999-102051685AEC'

update Registers
set Registers.name = 'Fylkesnummer 2019', seoname='fylkesnummer-2019', description = 'Fylkesnummer 2019'
where Registers.systemId = 'CD692FDC-31E0-4836-8AC7-A05491BC2D47'

update RegisterItems
set statusId = 'Retired'
where registerId = 'CD692FDC-31E0-4836-8AC7-A05491BC2D47'


update RegisterItems
set statusId = 'Valid'
where registerId = 'D96BCBD6-D6CE-4FD4-A999-102051685AEC'

-- Kommunenummer

update Registers
set Registers.systemId = '46F8F5DB-9731-422F-A04A-542E891F4E2B'
where Registers.systemId = '46F8F5DB-9731-422F-A04A-542E891F4E2A'

update Registers
set Registers.systemId = '46F8F5DB-9731-422F-A04A-542E891F4E2A'
where Registers.systemId = '85D75E07-7CAA-4876-B0B1-27513CE57670'

update Registers
set Registers.systemId = '85D75E07-7CAA-4876-B0B1-27513CE57670'
where Registers.systemId = '46F8F5DB-9731-422F-A04A-542E891F4E2B'

update Registers
set Registers.name = 'Kommunenummer', seoname='kommunenummer', description = 'Nummer til kommuner i Norge gjeldene fra 01.01.2020. Merknad: Det presiseres at kommunenummer alltid skal ha 4 sifre, dvs. eventuelt med ledende null. Kommunenummer benyttes for kopling mot en rekke andre registre som også benytter 4 sifre.'
where Registers.systemId = '46F8F5DB-9731-422F-A04A-542E891F4E2A'

update Registers
set Registers.name = 'Kommunenummer 2019', seoname='kommunenummer-2019', description = 'Kommunenummer 2019'
where Registers.systemId = '85D75E07-7CAA-4876-B0B1-27513CE57670'

update RegisterItems
set statusId = 'Retired'
where registerId = '85D75E07-7CAA-4876-B0B1-27513CE57670'

update RegisterItems
set statusId = 'Valid'
where registerId = '46F8F5DB-9731-422F-A04A-542E891F4E2A'




