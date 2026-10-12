IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_Member') AND name = 'Name')
BEGIN
    ALTER TABLE tbl_Member ADD [Name] NVARCHAR(250) NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_Member') AND name = 'ContactNo')
BEGIN
    ALTER TABLE tbl_Member ADD [ContactNo] NVARCHAR(50) NULL
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tbl_Member') AND name = 'BarAssociation')
BEGIN
    ALTER TABLE tbl_Member ADD [BarAssociation] NVARCHAR(500) NULL
END
