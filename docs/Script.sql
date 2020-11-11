USE TestDB -- Test Database..  dominos bunyesinde baska bir DB varsa o da.
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name='GeoCoordinateRecords')
BEGIN
	DROP TABLE GeoCoordinateRecords	
END
GO

CREATE TABLE GeoCoordinateRecords(
	 gcId int IDENTITY(1,1) NOT NULL PRIMARY KEY --gcGUID UNIQUEIDENTIFIER PRIMARY KEY default NEWID() 
	,gcSource_latitude decimal(8,6) NOT NULL DEFAULT(9999)  --> decimal(8,6) cunku koordinat bilgisi xx.xxxxxx formatinda
	,gcSource_longitude decimal(8,6) NOT NULL DEFAULT(9999)  --> decimal(8,6) cunku koordinat bilgisi xx.xxxxxx formatinda
	,gcDestination_latitude decimal(8,6) NOT NULL DEFAULT(9999)  --> decimal(8,6) cunku koordinat bilgisi xx.xxxxxx formatinda
	,gcDestination_longitude decimal(8,6) NOT NULL DEFAULT(9999)  --> decimal(8,6) cunku koordinat bilgisi xx.xxxxxx formatinda
	,gcInsertDate date NOT NULL DEFAULT('19000101')
	)
GO

-- Ihtiyac halinde Index tanýmý yapýlabilir:
/*
IF (EXISTS(SELECT 1 FROM sys.indexes  WHERE name='GeoCoordinateRecords_NCUI1'))
BEGIN	
	PRINT 'Index silindi OK'
	DROP INDEX GeoCoordinateRecords_NCUI1 ON GeoCoordinateRecords
END
GO
-- Performasn amacli Index: simdilik basit bir tablo. Client sorgu tiplerine gore indexleme detaylandirilabilir.
	CREATE UNIQUE INDEX GeoCoordinateRecords_NCUI1 on GeoCoordinateRecords(gcSource_latitude,gcSource_longitude,gcDestination_latitude,gcDestination_longitude) -- gunluk lokasyon bazli Kayitlar unique dusunuldu
	PRINT 'Index yaratýldý..OK'
GO
*/


-- Ihtiyac halinde user/role yetkileri verilebilir :
--GRANT SELECT,UPDATE,DELETE,INSERT ON GeoCoordinateRecords to userXXX
