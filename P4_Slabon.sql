USE WideWorldImporters;
GO
/*
	/* DROP THE DIM TABLES */
Drop table dbo.FactOrders
Drop table dbo.DimCities
Drop table dbo.DimCustomers
Drop table dbo.DimProducts
Drop table dbo.DimSalesPeople
Drop table dbo.DimDate
Drop table dbo.DimSuppliers
	/* DROP THE STAGING TABLES */
Drop table dbo.Customers_Stage
Drop table dbo.Products_Stage
Drop table dbo.SalesPeople_Stage
drop table dbo.Orders_stage
drop table dbo.Suppliers_Stage

Drop procedure dbo.DimDate_Load
	/* DROP THE EXTRACTS */
Drop procedure dbo.Customers_Extract
Drop procedure dbo.Products_Extract
Drop Procedure dbo.Suppliers_Extract
Drop Procedure dbo.Orders_Extract
Drop Procedure dbo.SalesPeople_Extract
	/* DROP THE PRELOAD TABLES */
Drop table dbo.Cities_Preload
Drop table dbo.Customers_Preload
Drop table dbo.Orders_Preload
Drop table dbo.Products_Preload
Drop table dbo.SalesPeople_Preload
Drop table dbo.Suppliers_Preload
	/* DROP THE SEQUENCES */
Drop sequence dbo.CityKey
Drop sequence dbo.CustomerKey
Drop sequence dbo.ProductKey
Drop sequence dbo.SalesPeopleKey
Drop sequence dbo.SuppliersKey
	/* DROP THE TRANSFORMS */
Drop procedure dbo.Customers_Transform --SCD 2
Drop procedure dbo.Cities_Transform --SCD 1
Drop procedure dbo.Orders_Transform --SCD 1
Drop procedure dbo.Products_Transform --SCD 2
Drop procedure dbo.SalesPeople_Transform -- SCD 1
Drop procedure dbo.Suppliers_Transform --SCD 1
go
*/
/* REQUIREMENT 1 */
/* Dimensional Modal Tables */

CREATE TABLE dbo.DimCities(
CityKey INT NOT NULL,
CityName NVARCHAR(50) NULL,
StateProvCode NVARCHAR(5) NULL,
StateProvName NVARCHAR(50) NULL,
CountryName NVARCHAR(60) NULL,
CountryFormalName NVARCHAR(60) NULL,
    CONSTRAINT PK_DimCities PRIMARY KEY CLUSTERED ( CityKey )
);

CREATE TABLE dbo.DimCustomers(
	CustomerKey INT NOT NULL,
	CustomerName NVARCHAR(100) NULL,
	CustomerCategoryName NVARCHAR(50) NULL,
	DeliveryCityName NVARCHAR(50) NULL,
	DeliveryStateProvCode NVARCHAR(5) NULL,
	DeliveryCountryName NVARCHAR(50) NULL,
	PostalCityName NVARCHAR(50) NULL,
	PostalStateProvCode NVARCHAR(5) NULL,
	PostalCountryName NVARCHAR(50) NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NULL,
    CONSTRAINT PK_DimCustomers PRIMARY KEY CLUSTERED ( CustomerKey )
);

CREATE TABLE dbo.DimProducts(
ProductKey INT NOT NULL,
ProductName NVARCHAR(100) NULL,
ProductColour NVARCHAR(20) NULL,
ProductBrand NVARCHAR(50) NULL,
ProductSize NVARCHAR(20) NULL,
StartDate DATE NOT NULL,
EndDate DATE NULL,
    CONSTRAINT PK_DimProducts PRIMARY KEY CLUSTERED ( ProductKey )
);

CREATE TABLE dbo.DimSalesPeople(
SalespersonKey INT NOT NULL,
FullName NVARCHAR(50) NULL,
PreferredName NVARCHAR(50) NULL,
LogonName NVARCHAR(50) NULL,
PhoneNumber NVARCHAR(20) NULL,
FaxNumber NVARCHAR(20) NULL,
EmailAddress NVARCHAR(256) NULL,
    CONSTRAINT PK_DimSalesPeople PRIMARY KEY CLUSTERED (SalespersonKey )
);

CREATE TABLE dbo.DimDate(
DateKey INT NOT NULL,
DateValue DATE NOT NULL,
Year SMALLINT NOT NULL,
Month TINYINT NOT NULL,
Day TINYINT NOT NULL,
Quarter TINYINT NOT NULL,
StartOfMonth DATE NOT NULL,
EndOfMonth DATE NOT NULL,
MonthName VARCHAR(9) NOT NULL,
DayOfWeekName VARCHAR(9) NOT NULL,
    CONSTRAINT PK_DimDate PRIMARY KEY CLUSTERED ( DateKey )
);

CREATE TABLE dbo.DimSuppliers(
SupplierKey INT NOT NULL,
	SupplierName nvarchar(100) NULL,
	PhoneNumber nvarchar(20) NULL,
	FaxNumber nvarchar(20) NULL,
	WebsiteURL nvarchar(256) NULL,
    CONSTRAINT PK_DimSuppliers PRIMARY KEY CLUSTERED ( SupplierKey )
);

CREATE TABLE dbo.FactOrders(
CustomerKey INT NOT NULL,
CityKey INT NOT NULL,
ProductKey INT NOT NULL,
SalespersonKey INT NOT NULL,
SupplierKey INT not null,
DateKey INT NOT NULL,
Quantity INT NOT NULL,
UnitPrice DECIMAL(18, 2) NOT NULL,
TaxRate DECIMAL(18, 3) NOT NULL,
TotalBeforeTax DECIMAL(18, 2) NOT NULL,
TotalAfterTax DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_FactOrders_DimCities FOREIGN KEY(CityKey) REFERENCES dbo.DimCities (CityKey),
    CONSTRAINT FK_FactOrders_DimCustomers FOREIGN KEY(CustomerKey) REFERENCES dbo.DimCustomers (CustomerKey),
    CONSTRAINT FK_FactOrders_DimDate FOREIGN KEY(DateKey) REFERENCES dbo.DimDate (DateKey),
    CONSTRAINT FK_FactOrders_DimProducts FOREIGN KEY(ProductKey) REFERENCES dbo.DimProducts (ProductKey),
	CONSTRAINT FK_FactOrders_DimSuppliers FOREIGN KEY(SupplierKey) REFERENCES dbo.DimSuppliers (SupplierKey),
    CONSTRAINT FK_FactOrders_DimSalesPeople FOREIGN KEY(SalespersonKey) REFERENCES dbo.DimSalesPeople 
(SalespersonKey)
);
GO

CREATE INDEX IX_FactOrders_CustomerKey ON dbo.FactOrders(CustomerKey);
CREATE INDEX IX_FactOrders_CityKey ON dbo.FactOrders(CityKey);
CREATE INDEX IX_FactOrders_ProductKey ON dbo.FactOrders(ProductKey);
CREATE INDEX IX_FactOrders_SalespersonKey ON dbo.FactOrders(SalespersonKey);
CREATE INDEX IX_FactOrders_DateKey ON dbo.FactOrders(DateKey);
CREATE INDEX IX_FactOrders_SupplierKey ON dbo.FactOrders(SupplierKey);

GO

/* REQUIREMENT 2 */

CREATE PROCEDURE dbo.DimDate_Load 
    @DateValue DATE
AS
BEGIN;
    INSERT INTO dbo.DimDate
    SELECT CAST( YEAR(@DateValue) * 10000 + MONTH(@DateValue) * 100 + DAY(@DateValue) AS INT),
           @DateValue,
           YEAR(@DateValue),
           MONTH(@DateValue),
           DAY(@DateValue),
           DATEPART(qq,@DateValue),
           DATEADD(DAY,1,EOMONTH(@DateValue,-1)),
           EOMONTH(@DateValue),
           DATENAME(mm,@DateValue),
           DATENAME(dw,@DateValue);
END
go

execute dbo.DimDate_Load '2013-01-01';

/* REQUIREMENT 3 */

WITH SalesPerCityRanked AS (
	Select   ct.CityName, p.ProductName as 'Product Name', f.Quantity as 'ProductQuantity', sup.SupplierName AS 'Supplier', f.TotalBeforeTax AS 'Total',
	cs.CustomerName as 'Customer Name', sp.FullName AS 'Sales Person', d.Month, d.Year

	From dbo.FactOrders f 
	join dbo.DimCustomers cs on cs.CustomerKey = f.CustomerKey
	join dbo.DimCities ct on ct.CityKey = f.CityKey
	join dbo.DimSalesPeople sp on sp.SalespersonKey = f.SalespersonKey
	join dbo.DimProducts p on p.ProductKey = f.ProductKey
	join dbo.DimSuppliers sup on sup.SupplierKey = f.SupplierKey
	join dbo.DimDate d on f.DateKey = d.DateKey

	Group By ct.CityName, f.TotalBeforeTax, f.Quantity, p.ProductName, cs.CustomerName, sp.FullName, sup.SupplierName
	, d.Month, d.Year
)
Select *,
	AVG (Total) OVER (Partition By CityName) as 'Average City Sales'

	From SalesPerCityRanked
	order by CityName, Total -- order by city, then sort by highest sales
go

/* REQUIREMENT 4 */

/* Customers Extract */
CREATE TABLE dbo.Customers_Stage (
    CustomerName NVARCHAR(100),
    CustomerCategoryName NVARCHAR(50),
    DeliveryCityName NVARCHAR(50),
    DeliveryStateProvinceCode NVARCHAR(5),
    DeliveryStateProvinceName NVARCHAR(50),
    DeliveryCountryName NVARCHAR(50),
    DeliveryFormalName NVARCHAR(60),
    PostalCityName NVARCHAR(50),
    PostalStateProvinceCode NVARCHAR(5),
    PostalStateProvinceName NVARCHAR(50),
    PostalCountryName NVARCHAR(50),
    PostalFormalName NVARCHAR(60)
);
go

CREATE PROCEDURE dbo.Customers_Extract
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @RowCt INT;
    TRUNCATE TABLE dbo.Customers_Stage;
    WITH CityDetails AS (
        SELECT ci.CityID,
               ci.CityName,
               sp.StateProvinceCode,
               sp.StateProvinceName,
               co.CountryName,
               co.FormalName
        FROM WideWorldImporters.Application.Cities ci
        LEFT JOIN WideWorldImporters.Application.StateProvinces sp
            ON ci.StateProvinceID = sp.StateProvinceID
        LEFT JOIN WideWorldImporters.Application.Countries co
            ON sp.CountryID = co.CountryID ) 
    INSERT INTO dbo.Customers_Stage (
        CustomerName,
        CustomerCategoryName,
        DeliveryCityName,
        DeliveryStateProvinceCode,
        DeliveryStateProvinceName,
        DeliveryCountryName,
        DeliveryFormalName,
        PostalCityName,
        PostalStateProvinceCode,
        PostalStateProvinceName,
        PostalCountryName,
        PostalFormalName )
    SELECT cust.CustomerName,
           cat.CustomerCategoryName,
           dc.CityName,
           dc.StateProvinceCode,
           dc.StateProvinceName,
           dc.CountryName,
           dc.FormalName,
           pc.CityName,
           pc.StateProvinceCode,
           pc.StateProvinceName,
           pc.CountryName,
           pc.FormalName
    FROM WideWorldImporters.Sales.Customers cust
    LEFT JOIN WideWorldImporters.Sales.CustomerCategories cat
        ON cust.CustomerCategoryID = cat.CustomerCategoryID
    LEFT JOIN CityDetails dc
        ON cust.DeliveryCityID = dc.CityID
    LEFT JOIN CityDetails pc
        ON cust.PostalCityID = pc.CityID;
    SET @RowCt = @@ROWCOUNT;
    IF @RowCt = 0 
    BEGIN;
        THROW 50001, 'No records found. Check with source system.', 1;
    END;
END;
go

/* Products Extract */
CREATE TABLE dbo.Products_Stage (
    StockItemName NVARCHAR(100),
    Brand NVARCHAR(50),
	Size NVARCHAR(50),
    ColorName NVARCHAR(20)
);
go

CREATE PROCEDURE dbo.Products_Extract
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @RowCt INT;
    TRUNCATE TABLE dbo.Products_Stage;
    INSERT INTO dbo.Products_Stage (
        StockItemName,
		Brand,
		Size,
		ColorName)
    SELECT si.StockItemName,
		si.Brand,
		si.Size,
		c.ColorName
    FROM WideWorldImporters.Warehouse.StockItems si
    LEFT JOIN WideWorldImporters.Warehouse.Colors c
        ON si.ColorID = c.ColorID;
    SET @RowCt = @@ROWCOUNT;
    IF @RowCt = 0 
    BEGIN;
        THROW 50001, 'No records found. Check with source system.', 1;
    END;
END;
go

/* SalesPeople Extract */
CREATE TABLE dbo.SalesPeople_Stage (
    FullName NVARCHAR(50),
	PreferredName NVARCHAR(50),
    LogonName NVARCHAR(50),
    PhoneNumber NVARCHAR(20),
    FaxNumber NVARCHAR(20),
    EmailAddress NVARCHAR(256)
);
GO

CREATE PROCEDURE dbo.SalesPeople_Extract
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @RowCount INT;
    TRUNCATE TABLE dbo.SalesPeople_Stage;
    INSERT INTO dbo.SalesPeople_Stage (
        FullName,
		PreferredName,
        LogonName,
        PhoneNumber,
        FaxNumber,
        EmailAddress
    )
    SELECT 
        p.FullName,
        p.LogonName,
		p.PreferredName,
        p.PhoneNumber,
        p.FaxNumber,
        p.EmailAddress
    FROM WideWorldImporters.Application.People p
    WHERE p.IsSalesperson = 1
    SET @RowCount = @@ROWCOUNT;
    IF @RowCount = 0 
    BEGIN;
        THROW 50001, 'No records found. Check with source system.', 1;
    END;
END;
GO
/* Orders Extract */
CREATE TABLE dbo.Orders_Stage (
    OrderDate    DATE,
    Quantity     INT,
    UnitPrice    DECIMAL(18,2),
    TaxRate      DECIMAL(18,3),
    CustomerName NVARCHAR(100),
    CityName     NVARCHAR(50),
    StateProvinceName NVARCHAR(50),
    CountryName  NVARCHAR(60),
    StockItemName NVARCHAR(100),
    LogonName    NVARCHAR(50)
)
GO

Create procedure dbo.Orders_Extract 
 @DateValue DATE
 AS
 BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @RowCount INT;
    TRUNCATE TABLE dbo.Orders_Stage;
	Insert into dbo.Orders_Stage (
		OrderDate,
        Quantity,
        UnitPrice,
        TaxRate,
        CustomerName,
        CityName,
        StateProvinceName,
        CountryName,
        StockItemName,
        LogonName
    )
    SELECT 
        o.OrderDate,
        ol.Quantity,
        ol.UnitPrice,
        ol.TaxRate,
        c.CustomerName,
        ci.CityName,
        sp.StateProvinceName,
        co.CountryName,
        si.StockItemName,
        p.LogonName
	FROM WideWorldImporters.Sales.Orders o
	left join WideWorldImporters.Sales.OrderLines ol ON o.OrderID = ol.OrderID
	left join WideWorldImporters.Sales.Customers c on c.CustomerID = o.CustomerID
	left join WideWorldImporters.Application.People p on p.PersonID = o.SalespersonPersonID
	left join WideWorldImporters.Application.Cities ci on ci.CityID = c.PostalCityID
	left join WideWorldImporters.Application.StateProvinces sp on sp.StateProvinceID = ci.StateProvinceID
	left join WideWorldImporters.Application.Countries co on co.CountryID = sp.CountryID
	left join WideWorldImporters.Warehouse.StockItems si on si.StockItemID = ol.StockItemID
	where o.OrderDate = @DateValue
	SET @RowCount = @@ROWCOUNT;
    IF @RowCount = 0 
    BEGIN;
        THROW 50001, 'No records found. Check with source system.', 1;
    END;
END
go
/* Suppliers Extract */
Create table dbo.Suppliers_Stage (
	SupplierName nvarchar(100),
	PhoneNumber nvarchar(20),
	FaxNumber nvarchar(20),
	WebsiteURL nvarchar(256),
	SupplierCategoryName nvarchar(50)
);
go

CREATE PROCEDURE dbo.Suppliers_Extract
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @RowCt INT;
    TRUNCATE TABLE dbo.Suppliers_Stage;
    INSERT INTO dbo.Suppliers_Stage(
        SupplierName,
		PhoneNumber,
		FaxNumber,
		WebsiteURL,
		SupplierCategoryName )
    SELECT s.SupplierName,
		s.PhoneNumber,
		s.FaxNumber,
		s.WebsiteURL,
		c.SupplierCategoryName
    FROM WideWorldImporters.Purchasing.Suppliers s
    LEFT JOIN WideWorldImporters.Purchasing.SupplierCategories c
        ON s.SupplierCategoryID = c.SupplierCategoryID;
    SET @RowCt = @@ROWCOUNT;
    IF @RowCt = 0
    BEGIN;
        THROW 50001, 'No records found. Check with source system.', 1;
    END;
END;
go

Execute dbo.Suppliers_Extract
go
Execute dbo.Customers_Extract
go
Execute dbo.Products_Extract
go
Execute dbo.SalesPeople_Extract
go
Execute dbo.Orders_Extract '2013-01-01'
go

Select * from dbo.Orders_Stage
Select * from dbo.Suppliers_Stage
Select * from dbo.Customers_Stage
Select * from dbo.Products_Stage
Select * from dbo.SalesPeople_Stage
go

/* REQUIREMENT 5 */

/* PRE-LOAD TABLES */
CREATE TABLE dbo.Cities_Preload (
    CityKey INT NOT NULL,
    CityName NVARCHAR(50) NULL,
    StateProvCode NVARCHAR(5) NULL,
    StateProvName NVARCHAR(50) NULL,
    CountryName NVARCHAR(60) NULL,
    CountryFormalName NVARCHAR(60) NULL,
    CONSTRAINT PK_Cities_Preload PRIMARY KEY CLUSTERED ( CityKey )
);

CREATE TABLE dbo.Customers_Preload (
CustomerKey INT NOT NULL,
CustomerName NVARCHAR(100) NULL,
CustomerCategoryName NVARCHAR(50) NULL,
DeliveryCityName NVARCHAR(50) NULL,
DeliveryStateProvCode NVARCHAR(5) NULL,
DeliveryCountryName NVARCHAR(50) NULL,
PostalCityName NVARCHAR(50) NULL,
PostalStateProvCode NVARCHAR(5) NULL,
PostalCountryName NVARCHAR(50) NULL,
StartDate DATE NOT NULL,
EndDate DATE NULL,
    CONSTRAINT PK_Customers_Preload PRIMARY KEY CLUSTERED ( CustomerKey )
);

CREATE TABLE dbo.Orders_Preload (
CustomerKey INT NOT NULL,
CityKey INT NOT NULL,
ProductKey INT NOT NULL,
SalespersonKey INT NOT NULL,
DateKey INT NOT NULL,
Quantity INT NOT NULL,
UnitPrice DECIMAL(18, 2) NOT NULL,
TaxRate DECIMAL(18, 3) NOT NULL,
TotalBeforeTax DECIMAL(18, 2) NOT NULL,
TotalAfterTax DECIMAL(18, 2) NOT NULL
);

CREATE TABLE dbo.SalesPeople_Preload (
SalespersonKey INT NOT NULL,
FullName NVARCHAR(50) NULL,
PreferredName NVARCHAR(50) NULL,
LogonName NVARCHAR(50) NULL,
PhoneNumber NVARCHAR(20) NULL,
FaxNumber NVARCHAR(20) NULL,
EmailAddress NVARCHAR(256) NULL,
    CONSTRAINT PK_SalesPeople_Preload PRIMARY KEY CLUSTERED (SalespersonKey )
);

CREATE TABLE dbo.Suppliers_Preload (
    SuppliersKey INT NOT NULL,
    SupplierName nvarchar(100),
	PhoneNumber nvarchar(20),
	FaxNumber nvarchar(20),
	WebsiteURL nvarchar(256),
	SupplierCategoryName nvarchar(50)
    CONSTRAINT PK_Suppliers_Preload PRIMARY KEY CLUSTERED ( SuppliersKey )
);

CREATE TABLE dbo.Products_Preload (
ProductKey INT NOT NULL,
ProductName NVARCHAR(100) NULL,
ProductColour NVARCHAR(20) NULL,
ProductBrand NVARCHAR(50) NULL,
ProductSize NVARCHAR(20) NULL,
StartDate DATE NOT NULL,
EndDate DATE NULL,
    CONSTRAINT PK_Products_Preload PRIMARY KEY CLUSTERED ( ProductKey )
);
go

/* TRANSFORM Procedures */
CREATE SEQUENCE dbo.CityKey START WITH 1;
CREATE SEQUENCE dbo.CustomerKey START WITH 1;
CREATE SEQUENCE dbo.ProductKey START WITH 1;
CREATE SEQUENCE dbo.SuppliersKey START WITH 1;
go

CREATE PROCEDURE dbo.Cities_Transform
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    TRUNCATE TABLE dbo.Cities_Preload;
    BEGIN TRANSACTION;
    INSERT INTO dbo.Cities_Preload /* Column list excluded for brevity */
    SELECT NEXT VALUE FOR dbo.CityKey AS CityKey,
           cu.DeliveryCityName,
           cu.DeliveryStateProvinceCode,
           cu.DeliveryStateProvinceName,
           cu.DeliveryCountryName,
           cu.DeliveryFormalName
    FROM dbo.Customers_Stage cu
    WHERE NOT EXISTS ( SELECT 1 
                       FROM dbo.DimCities ci
                       WHERE cu.DeliveryCityName = ci.CityName
                             AND cu.DeliveryStateProvinceName = ci.StateProvName
                             AND cu.DeliveryCountryName = ci.CountryName )
    INSERT INTO dbo.Cities_Preload /* Column list excluded for brevity */
    SELECT ci.CityKey,
           cu.DeliveryCityName,
           cu.DeliveryStateProvinceCode,
           cu.DeliveryStateProvinceName,
           cu.DeliveryCountryName,
           cu.DeliveryFormalName
    FROM dbo.Customers_Stage cu
    JOIN dbo.DimCities ci
        ON cu.DeliveryCityName = ci.CityName
        AND cu.DeliveryStateProvinceName = ci.StateProvName
        AND cu.DeliveryCountryName = ci.CountryName
	Group by CityKey, DeliveryCityName, DeliveryStateProvinceCode, DeliveryStateProvinceName, DeliveryCountryName, DeliveryFormalName
    COMMIT TRANSACTION;
END;
go

CREATE PROCEDURE dbo.Customers_Transform
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    TRUNCATE TABLE dbo.Customers_Preload;
    DECLARE @StartDate DATE = GETDATE();
    DECLARE @EndDate DATE = DATEADD(dd,-1,GETDATE());
    BEGIN TRANSACTION;
    -- Add updated records
    INSERT INTO dbo.Customers_Preload /* Column list excluded for brevity */
    SELECT NEXT VALUE FOR dbo.CustomerKey AS CustomerKey,
           stg.CustomerName,
           stg.CustomerCategoryName,
           stg.DeliveryCityName,
           stg.DeliveryStateProvinceCode,
           stg.DeliveryCountryName,
           stg.PostalCityName,
           stg.PostalStateProvinceCode,
           stg.PostalCountryName,
           @StartDate,
           NULL
    FROM dbo.Customers_Stage stg
    JOIN dbo.DimCustomers cu
        ON stg.CustomerName = cu.CustomerName
        AND cu.EndDate IS NULL
    WHERE stg.CustomerCategoryName <> cu.CustomerCategoryName
          OR stg.DeliveryCityName <> cu.DeliveryCityName
          OR stg.DeliveryStateProvinceCode <> cu.DeliveryStateProvCode
          OR stg.DeliveryCountryName <> cu.DeliveryCountryName
          OR stg.PostalCityName <> cu.PostalCityName
          OR stg.PostalStateProvinceCode <> cu.PostalStateProvCode
          OR stg.PostalCountryName <> cu.PostalCountryName;
    -- Add existing records, and expire as necessary
    INSERT INTO dbo.Customers_Preload /* Column list excluded for brevity */
    SELECT cu.CustomerKey,
           cu.CustomerName,
           cu.CustomerCategoryName,
           cu.DeliveryCityName,
           cu.DeliveryStateProvCode,
           cu.DeliveryCountryName,
           cu.PostalCityName,
           cu.PostalStateProvCode,
           cu.PostalCountryName,
           cu.StartDate,
           CASE 
               WHEN pl.CustomerName IS NULL THEN NULL
               ELSE @EndDate
           END AS EndDate
    FROM dbo.DimCustomers cu
    LEFT JOIN dbo.Customers_Preload pl    
        ON pl.CustomerName = cu.CustomerName
        AND cu.EndDate IS NULL;
    
    -- Create new records
    INSERT INTO dbo.Customers_Preload /* Column list excluded for brevity */
    SELECT NEXT VALUE FOR dbo.CustomerKey AS CustomerKey,
           stg.CustomerName,
           stg.CustomerCategoryName,
           stg.DeliveryCityName,
           stg.DeliveryStateProvinceCode,
           stg.DeliveryCountryName,
           stg.PostalCityName,
           stg.PostalStateProvinceCode,
           stg.PostalCountryName,
           @StartDate,
           NULL
    FROM dbo.Customers_Stage stg
    WHERE NOT EXISTS ( SELECT 1 FROM dbo.DimCustomers cu WHERE stg.CustomerName = cu.CustomerName );
    -- Expire missing records
    INSERT INTO dbo.Customers_Preload /* Column list excluded for brevity */
    SELECT cu.CustomerKey,
           cu.CustomerName,
           cu.CustomerCategoryName,
           cu.DeliveryCityName,
           cu.DeliveryStateProvCode,
           cu.DeliveryCountryName,
           cu.PostalCityName,
           cu.PostalStateProvCode,
           cu.PostalCountryName,
           cu.StartDate,
           @EndDate
    FROM dbo.DimCustomers cu
    WHERE NOT EXISTS ( SELECT 1 FROM dbo.Customers_Stage stg WHERE stg.CustomerName = cu.CustomerName )
          AND cu.EndDate IS NULL;
    COMMIT TRANSACTION;
END;
go

Create Procedure dbo.Products_Transform
AS
BEGIN;
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
	DECLARE @StartDate DATE = GETDATE();
    DECLARE @EndDate DATE = DATEADD(dd,-1,GETDATE());
	BEGIN TRANSACTION;
	-- Add updated records
    TRUNCATE TABLE dbo.Products_Preload;
	INSERT INTO dbo.Products_Preload
	Select next value for dbo.ProductKey as ProductKey,
	stg.StockItemName,
	stg.ColorName,
	stg.Brand,
	stg.Size,
	@StartDate,
	null
	FROM
	dbo.Products_Stage stg
	JOIN dbo.DimProducts dp
        ON stg.StockItemName = dp.ProductName
        AND dp.EndDate IS NULL
	WHERE stg.ColorName <> dp.ProductColour
          OR stg.Brand <> dp.ProductBrand
          OR stg.Size <> dp.ProductSize
	-- Add existing records, and expire as necessary
	INSERT INTO dbo.Products_Preload
	Select next value for dbo.ProductKey as ProductKey,
	pr.ProductName,
	pr.ProductColour,
	pr.ProductBrand,
	pr.ProductSize,
	pr.StartDate,
	CASE 
		WHEN pl.ProductName IS NULL THEN NULL
			ELSE @EndDate
        END AS EndDate
    FROM dbo.DimProducts pr
	LEFT JOIN dbo.Products_Preload pl    
        ON pl.ProductName = pr.ProductName
        AND pr.EndDate IS NULL;
	 -- Create new records
	 INSERT INTO dbo.Products_Preload
	Select next value for dbo.ProductKey as ProductKey,
	stg.StockItemName,
	stg.ColorName,
	stg.Brand,
	stg.Size,
	@StartDate,
	null
	FROM
	dbo.Products_Stage stg
	WHERE NOT EXISTS ( SELECT 1 FROM dbo.DimProducts cu WHERE stg.StockItemName = cu.ProductName );
	-- Expire missing records
	INSERT INTO dbo.Products_Preload
	Select next value for dbo.ProductKey as ProductKey,
	pr.ProductName,
	pr.ProductColour,
	pr.ProductBrand,
	pr.ProductSize,
	pr.StartDate,
	@EndDate
	FROM
	dbo.DimProducts pr
	WHERE NOT EXISTS ( SELECT 1 FROM dbo.Products_Stage stg WHERE stg.StockItemName = pr.ProductName )
          AND pr.EndDate IS NULL;
    COMMIT TRANSACTION;
END;
go

CREATE SEQUENCE dbo.SalesPeopleKey START WITH 1;
GO
CREATE PROCEDURE dbo.SalesPeople_Transform
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    TRUNCATE TABLE dbo.SalesPeople_Preload;
    BEGIN TRANSACTION;
    INSERT INTO dbo.SalesPeople_Preload /* Column list excluded for brevity */
    SELECT NEXT VALUE FOR dbo.SalesPeopleKey AS SalesPeopleKey,
        stg.FullName,
        stg.PreferredName,
        stg.LogonName,
        stg.PhoneNumber,
        stg.FaxNumber,
        stg.EmailAddress
    FROM  dbo.SalesPeople_Stage stg
    WHERE NOT EXISTS ( SELECT 1 FROM dbo.DimSalesPeople sp WHERE stg.FullName = sp.FullName );

    INSERT INTO dbo.SalesPeople_Preload /* Column list excluded for brevity */
    SELECT 
        sp.SalesPersonKey,
        stg.FullName,
        stg.PreferredName,
        stg.LogonName,
        stg.PhoneNumber,
        stg.FaxNumber,
        stg.EmailAddress
    FROM dbo.SalesPeople_Stage stg
    JOIN dbo.DimSalesPeople sp
        ON stg.FullName = sp.FullName
    COMMIT TRANSACTION;
END;
GO

GO
CREATE PROCEDURE dbo.Suppliers_Transform
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    TRUNCATE TABLE dbo.Suppliers_Preload;
    BEGIN TRANSACTION;
    INSERT INTO dbo.Suppliers_Preload /* Column list excluded for brevity */
    SELECT NEXT VALUE FOR dbo.SuppliersKey AS SuppliersKey,
        stg.SupplierName,
        stg.PhoneNumber,
        stg.FaxNumber,
        stg.WebsiteURL,
		stg.SupplierCategoryName
    FROM  dbo.Suppliers_Stage stg
    WHERE NOT EXISTS ( SELECT 1 FROM dbo.DimSuppliers su WHERE stg.SupplierName = su.SupplierName );
    COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE dbo.Orders_Transform
AS
BEGIN;
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    TRUNCATE TABLE dbo.Orders_Preload;
    INSERT INTO dbo.Orders_Preload /* Columns excluded for brevity */
    SELECT cu.CustomerKey,
           ci.CityKey,
           pr.ProductKey,
           sp.SalespersonKey,
           CAST(YEAR(ord.OrderDate) * 10000 + MONTH(ord.OrderDate) * 100 + DAY(ord.OrderDate) AS INT),
           SUM(ord.Quantity) AS Quantity,
           AVG(ord.UnitPrice) AS UnitPrice,
           AVG(ord.TaxRate) AS TaxRate,
           SUM(ord.Quantity * ord.UnitPrice) AS TotalBeforeTax,
           SUM(ord.Quantity * ord.UnitPrice * (1 + ord.TaxRate/100)) AS TotalAfterTax
    FROM dbo.Orders_Stage ord
    JOIN dbo.Customers_Preload cu
        ON ord.CustomerName = cu.CustomerName
    JOIN dbo.Cities_Preload ci
        ON ord.CityName = ci.CityName
        AND ord.StateProvinceName = ci.StateProvName
        AND ord.CountryName = ci.CountryName
    JOIN dbo.Products_Preload pr
        ON ord.StockItemName = pr.ProductName
    JOIN dbo.SalesPeople_Preload sp
        ON ord.LogonName = sp.LogonName
	Group by cu.CustomerKey, ci.CityKey, pr.ProductKey, sp.SalespersonKey, ord.OrderDate
END;
go

execute dbo.Customers_Transform
go
execute dbo.Products_Transform
go
execute dbo.SalesPeople_Transform
go
execute dbo.Suppliers_Transform
go
execute dbo.Cities_Transform
go
execute dbo.Orders_Transform
go
Select * from dbo.Cities_Preload
Select * from dbo.Customers_Preload
Select * from dbo.Products_Preload
Select * from dbo.Suppliers_Preload
Select * from dbo.SalesPeople_Preload
Select * from dbo.Orders_Preload
go