CREATE DATABASE MRP
GO

use MRP
GO

CREATE TABLE Vendor
(
    VendorId int IDENTITY(1, 1) PRIMARY KEY NOT NULL
    , CompanyName varchar(40) NOT NULL
    , mainContactName varchar(40) NOT NULL
    , phoneNumber varchar(40) NOT NULL
    , paymentAddress varchar(200)  NOT NULL
    , rating varchar(2)  NOT NULL
)
GO

CREATE TABLE Parts (
    partID int IDENTITY(1, 1) PRIMARY KEY NOT NULL
    , partName varchar(30)NOT NULL
    , description varchar(30)NULL
    , orderLeadTime datetime NULL 
    , safetyStock int NULL
    , maxLevelOnHand int NULL 
    , UnitCost money NOT NULL
    , UnitType varchar(30)NOT NULL
    , PartPicture varchar(30)NULL
    , HoursofAssembly real NOT NULL
	, supplyVendorId int FOREIGN KEY REFERENCES Vendor(VendorId) NULL
);

GO

CREATE TABLE Inventory
(
    ProductId int FOREIGN KEY REFERENCES Parts(partID) NOT NULL
    , OnHandQty int NOT NULL
    , OnOrderQty int NOT NULL
    , CommittedQty int NOT NULL
);
GO

CREATE TABLE InventoryLocation
(
    ProductId int NOT NULL
    , Location varchar(10) NOT NULL
    , Qty int NOT NULL
    PRIMARY KEY (ProductID, Location)
);
GO

CREATE TABLE Customer
(
    CustomerId int Primary Key  NOT NULL
    , AccountName varchar(25) NOT NULL
    , CompanyName varchar(25) NOT NULL
    , CustomerAccountName varchar(25) NOT NULL
    , BillToAddress varchar(40) NOT NULL
    , ShippingAddress varchar(40) NOT NULL
    , CreditCardNo varchar(16) NOT NULL
);
GO

CREATE TABLE CustomerOrderHead (
    corderId int IDENTITY(1,1) PRIMARY KEY NOT NULL
    , CustomerId int FOREIGN KEY REFERENCES Customer(CustomerID)    
    , DateSubmitted datetime NOT NULL
    , StatusCode int NOT NULL -- 1 Pending; 2 Approved; 3 Rejected; 4 Complete
    , PaymentTotal money NOT NULL
    , PamentReceived money NULL
    , SplitShipmentsAvailible bit NULL
    , DateUpdated datetime NULL,
    
   CONSTRAINT validStatus
   CHECK (StatusCode BETWEEN 0 and 5)
)
GO

CREATE TABLE CustomerOrderDetail
(
    OrderId int FOREIGN KEY REFERENCES CustomerOrderHead(COrderId) NOT NULL
    , ProductId int FOREIGN KEY REFERENCES Parts(partID) NOT NULL
    , Qty real NOT NULL 
    , PricePer money NOT NULL
)

CREATE TABLE CreditHistory
(
    CustomerId int FOREIGN KEY REFERENCES Customer(CustomerId) NOT NULL
    , CreditorName varchar(25)NOT NULL
    , CreditorAccNo varchar(20)NOT NULL
    , CurrentBalance int NOT NULL
);

GO

CREATE TABLE Resources
(
    InventoryId int Primary Key NOT NULL
    , NumOfOperators int NOT NULL
    , MaintenanceDesc varchar(200) NOT NULL
    , MaintenanceFrequency varchar(20) NOT NULL
    , LastServiced varchar(20) NOT NULL
    , ResourceStatus varchar(100) NOT NULL

)
GO

CREATE TABLE BoM (
    ParentID int FOREIGN KEY REFERENCES Parts(partID) NOT NULL
    , ChildID int FOREIGN KEY REFERENCES Parts(partID) NOT NULL
    , Qty real NOT NULL
    , UnitMeasureCode varchar(10) NOT NULL
)
GO

CREATE TABLE VendorOrderHead (
    VendorOrderID int IDENTITY(1, 1) PRIMARY KEY NOT NULL
    , VendorID int FOREIGN KEY REFERENCES Vendor(VendorId) NOT NULL
    , DateSubmitted datetime NOT NULL
    , Status int NOT NULL
    , Total int  NOT NULL
    , splitShipmentsAvaliable Bit NOT NULL
    , DateUpdated datetime  NOT NULL
)

CREATE TABLE VendorOrderDetail (
     VendorOrderDetail int FOREIGN KEY REFERENCES VendorOrderHead(VendorOrderId)
    , ProductID int NOT NULL
    , Qty int NOT NULL
    , PricePer int NOT NULL
)
GO

CREATE PROC sp_queryParts
AS
BEGIN
	SELECT PartID, partName
	FROM Parts
END
GO

CREATE PROC sp_AddVendor
@name varchar(40),
@contactName varchar(40),
@phoneNum varchar(40),
@paymentAddress varchar(200),
@rating varchar(2)
AS
BEGIN
	INSERT INTO Vendor
	VALUES (@name, @contactName, @phoneNum, @paymentAddress, @rating)
END
GO

CREATE PROC sp_AddVendorPart
@vendorId int,
@partId int
AS
BEGIN
	IF EXISTS (SELECT * FROM Parts WHERE partID = @partId)
	BEGIN
		UPDATE Parts
		SET supplyVendorId = @vendorId
		WHERE partID = @partId
	END
END
GO

CREATE PROC sp_GetVendorId
@name varchar(40)
AS
BEGIN
	SELECT VendorId
	FROM Vendor
	WHERE companyName = @name
END
GO

CREATE INDEX inventoryIndex
ON Inventory (ProductId);
GO


CREATE INDEX customerIndex
ON Customer (CustomerId);
GO

CREATE INDEX BoMIndex
ON BoM (ParentID, ChildID);
GO


CREATE INDEX PartsIndex
ON Parts (partID);
GO


CREATE INDEX VendorIndex
ON Vendor (VendorId);
GO
