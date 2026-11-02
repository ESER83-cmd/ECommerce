UPDATE Users
SET Role = 'Admin'
WHERE Email = 'admin@ecommerce.com';

SELECT Id, Email, Role
FROM Users;
SELECT * FROM OrderItems;

SELECT Id, Name, Stock
FROM Products
WHERE Id = 3;
SELECT * FROM CartItems;
SELECT Id, UserId, ProductId, Quantity
FROM CartItems
WHERE UserId = 3
ORDER BY ProductId, Id;

SELECT
    ProductId,
    COUNT(*) AS ItemCount,
    SUM(Quantity) AS TotalQuantity
FROM CartItems
WHERE UserId = 3
GROUP BY ProductId
ORDER BY ProductId;
 
 SELECT MIN(Id) AS KeepId
FROM CartItems
WHERE UserId = 3 AND ProductId = 3;

SELECT *
INTO CartItems_Backup_20261014
FROM CartItems;

BEGIN TRANSACTION;

UPDATE CartItems
SET Quantity = (
    SELECT SUM(Quantity)
    FROM CartItems
    WHERE UserId = 3 AND ProductId = 3
)
WHERE Id = 9 AND UserId = 3 AND ProductId = 3;

DELETE FROM CartItems
WHERE UserId = 3
  AND ProductId = 3
  AND Id <> 9;

SELECT Id, UserId, ProductId, Quantity
FROM CartItems
WHERE UserId = 3;

BEGIN TRANSACTION;

UPDATE CartItems
SET Quantity = (
    SELECT SUM(Quantity)
    FROM CartItems
    WHERE UserId = 3 AND ProductId = 3
)
WHERE Id = 9 AND UserId = 3 AND ProductId = 3;

DELETE FROM CartItems
WHERE UserId = 3
  AND ProductId = 3
  AND Id <> 9;

SELECT Id, UserId, ProductId, Quantity
FROM CartItems
WHERE UserId = 3;

COMMIT TRANSACTION;

SELECT Id, Name, Stock
FROM Products
WHERE Id = 3;

UPDATE CartItems
SET Quantity = 1
WHERE Id = 9
  AND UserId = 3
  AND ProductId = 3;

SELECT Id, UserId, ProductId, Quantity
FROM CartItems
WHERE UserId = 3;

DELETE FROM CartItems
WHERE UserId = 3;

SELECT Id, Name, Price, Stock, ImageUrl
FROM Products;

SELECT TOP 1 *
FROM Products;

SELECT Id, Name, Description, Price, Stock, ImageUrl
FROM Products;

UPDATE Products
SET Name = 'Keychain',
    Description = 'Leather keychain',
    Price = 149.00,
    Stock = 10,
    ImageUrl = 'keychain.png'
WHERE Id = 1;

UPDATE Products
SET Name = 'Wallet',
    Description = 'Leather wallet',
    Price = 599.00,
    Stock = 10,
    ImageUrl = 'wallet.png'
WHERE Id = 3;

INSERT INTO Products
(Name, Description, Price, Stock, ImageUrl, IsActive, CategoryId)
VALUES
('Belt', 'Leather belt', 799.00, 10, 'belt.png', 1, 1),
('Bracelet', 'Leather bracelet', 299.00, 10, 'bracelet.png', 1, 1);

SELECT Id, Name, Description, Price, Stock, ImageUrl
FROM Products;

UPDATE Products
SET Name = 'Wallet - Black',
    Description = 'Black leather wallet',
    ImageUrl = 'wallet-black.png'
WHERE Name = 'Wallet';

INSERT INTO Products
(Name, Description, Price, Stock, ImageUrl, IsActive, CategoryId)
VALUES
('Wallet - Green', 'Green leather wallet', 599.00, 10, 'wallet-green.png', 1, 1),
('Wallet - Burgundy', 'Burgundy leather wallet', 599.00, 10, 'wallet-burgundy.png', 1, 1);

SELECT Id, Name, ImageUrl
FROM Products;

UPDATE Products
SET Name = 'Wallet',
    Description = 'Premium leather wallet',
    Price = 599.00,
    Stock = 10,
    ImageUrl = 'wallet.png'
WHERE Name = 'Wallet - Black'
   OR Name = 'Wallet';

DELETE FROM Products
WHERE Name = 'Wallet - Green'
   OR Name = 'Wallet - Burgundy'
   OR Name = 'Wallet - Red';

SELECT Id, Name, ImageUrl
FROM Products;

SELECT Id, Name
FROM Products
ORDER BY Id;

UPDATE dbo.Products
SET ImageUrl = '/Images/Products/card holders.png'
WHERE Name = 'Leather Card Holder';

SELECT Id, Name, ImageUrl
FROM dbo.Products
WHERE Name = 'Leather Card Holder';

SELECT Id, Email
FROM dbo.Users;