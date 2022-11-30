-- Set valid if validFromdate
update RegisterItems
SET RegisterItems.statusId = 'Valid'
FROM     Registers INNER JOIN
                  RegisterItems ON Registers.systemId = RegisterItems.registerId
WHERE  (Registers.containedItemClass = 'CodelistValue') AND (Registers.statusId = 'Valid')
AND GETDATE() > = ValidFromDate and ValidToDate IS NULL AND RegisterItems.statusId <>'Valid'

-- Set retired if not ValidToDate
update RegisterItems
SET RegisterItems.statusId = 'Retired'
FROM     Registers INNER JOIN
                  RegisterItems ON Registers.systemId = RegisterItems.registerId
WHERE  (Registers.containedItemClass = 'CodelistValue') AND (Registers.statusId = 'Valid')
AND GETDATE() > ValidToDate and RegisterItems.statusId <> 'Retired'

-- Set valid if date between ValidFromDate and ValidToDate
update RegisterItems
SET RegisterItems.statusId = 'Valid'
FROM     Registers INNER JOIN
                  RegisterItems ON Registers.systemId = RegisterItems.registerId
WHERE  (Registers.containedItemClass = 'CodelistValue') AND (Registers.statusId = 'Valid')
AND GETDATE() between ValidFromDate and ValidToDate AND RegisterItems.statusId <>'Valid' 

-- Set valid if ValidToDate
update RegisterItems
SET RegisterItems.statusId = 'Valid'
FROM     Registers INNER JOIN
                  RegisterItems ON Registers.systemId = RegisterItems.registerId
WHERE  (Registers.containedItemClass = 'CodelistValue') AND (Registers.statusId = 'Valid')
AND GETDATE() < = ValidToDate and ValidFromDate IS NULL AND RegisterItems.statusId <>'Valid'
