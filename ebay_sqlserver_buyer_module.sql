CREATE DATABASE CloneEbayDB;
GO

USE CloneEbayDB;
GO

-- User table
CREATE TABLE [User] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [username] NVARCHAR(100),
    [email] NVARCHAR(100) UNIQUE,
    [password] NVARCHAR(255),
    [role] NVARCHAR(20),
    [avatarURL] NVARCHAR(MAX),
    [isVerified] BIT DEFAULT 0, -- Added for email verification
    [createdAt] DATETIME DEFAULT GETDATE()
);

-- Address table
CREATE TABLE [Address] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [fullName] NVARCHAR(100),
    [phone] NVARCHAR(20),
    [street] NVARCHAR(100),
    [city] NVARCHAR(50),
    [state] NVARCHAR(50),
    [country] NVARCHAR(50),
    [isDefault] BIT,
    [postalCode] NVARCHAR(20) -- Added for shipping precision
);

-- Category table
CREATE TABLE [Category] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [name] NVARCHAR(100),
    [parentId] INT NULL FOREIGN KEY REFERENCES [Category](id) -- Added for subcategories
);

-- Product table
CREATE TABLE [Product] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [title] NVARCHAR(255),
    [description] NVARCHAR(MAX),
    [price] DECIMAL(10,2),
    [images] NVARCHAR(MAX), -- Kept for compatibility, but use ProductImage for multiple images
    [categoryId] INT FOREIGN KEY REFERENCES [Category](id),
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [isAuction] BIT,
    [auctionEndTime] DATETIME,
    [condition] NVARCHAR(50), -- Added (new/used/refurbished)
    [createdAt] DATETIME DEFAULT GETDATE()
);

-- ProductImage table (Added for multiple images)
CREATE TABLE [ProductImage] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [imageURL] NVARCHAR(MAX),
    [isPrimary] BIT DEFAULT 0
);

-- OrderTable
CREATE TABLE [OrderTable] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [buyerId] INT FOREIGN KEY REFERENCES [User](id),
    [addressId] INT FOREIGN KEY REFERENCES [Address](id),
    [orderDate] DATETIME,
    [totalPrice] DECIMAL(10,2),
    [status] NVARCHAR(20),
    [couponId] INT NULL FOREIGN KEY REFERENCES [Coupon](id) -- Added for coupon tracking
);

-- OrderItem
CREATE TABLE [OrderItem] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [quantity] INT,
    [unitPrice] DECIMAL(10,2)
);

-- Payment
CREATE TABLE [Payment] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [amount] DECIMAL(10,2),
    [method] NVARCHAR(50),
    [status] NVARCHAR(20),
    [paidAt] DATETIME,
    [transactionId] NVARCHAR(100) -- Added for PayOS
);

-- ShippingInfo
CREATE TABLE [ShippingInfo] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [carrier] NVARCHAR(100),
    [trackingNumber] NVARCHAR(100),
    [status] NVARCHAR(50),
    [estimatedArrival] DATETIME
);

-- ReturnRequest
CREATE TABLE [ReturnRequest] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [reason] NVARCHAR(MAX),
    [status] NVARCHAR(20),
    [createdAt] DATETIME
);

-- Bid
CREATE TABLE [Bid] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [bidderId] INT FOREIGN KEY REFERENCES [User](id),
    [amount] DECIMAL(10,2),
    [bidTime] DATETIME
);

-- Review
CREATE TABLE [Review] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [reviewerId] INT FOREIGN KEY REFERENCES [User](id),
    [rating] INT,
    [comment] NVARCHAR(MAX),
    [createdAt] DATETIME
);

-- Message
CREATE TABLE [Message] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [senderId] INT FOREIGN KEY REFERENCES [User](id),
    [receiverId] INT FOREIGN KEY REFERENCES [User](id),
    [content] NVARCHAR(MAX),
    [timestamp] DATETIME,
    [isRead] BIT DEFAULT 0 -- Added for message status
);

-- Coupon
CREATE TABLE [Coupon] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [code] NVARCHAR(50),
    [discountPercent] DECIMAL(5,2),
    [startDate] DATETIME,
    [endDate] DATETIME,
    [maxUsage] INT,
    [productId] INT NULL FOREIGN KEY REFERENCES [Product](id),
    [usedCount] INT DEFAULT 0 -- Added for tracking usage
);

-- Inventory
CREATE TABLE [Inventory] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [productId] INT FOREIGN KEY REFERENCES [Product](id),
    [quantity] INT,
    [lastUpdated] DATETIME
);

-- Feedback
CREATE TABLE [Feedback] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [averageRating] DECIMAL(3,2),
    [totalReviews] INT,
    [positiveRate] DECIMAL(5,2)
);

-- Dispute
CREATE TABLE [Dispute] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [orderId] INT FOREIGN KEY REFERENCES [OrderTable](id),
    [raisedBy] INT FOREIGN KEY REFERENCES [User](id),
    [description] NVARCHAR(MAX),
    [status] NVARCHAR(20),
    [resolution] NVARCHAR(MAX)
);

-- Store
CREATE TABLE [Store] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [storeName] NVARCHAR(100),
    [description] NVARCHAR(MAX),
    [bannerImageURL] NVARCHAR(MAX)
);

-- Notification (Added for system alerts)
CREATE TABLE [Notification] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [userId] INT FOREIGN KEY REFERENCES [User](id),
    [content] NVARCHAR(MAX),
    [type] NVARCHAR(50),
    [isRead] BIT DEFAULT 0,
    [createdAt] DATETIME
);

-- Insert sample data
INSERT INTO [Category] (name) VALUES ('Electronics'), ('Clothing'), ('Books');
INSERT INTO [User] (username, email, password, role, isVerified) VALUES ('testuser', 'test@example.com', 'hashedpassword', 'Buyer', 1);
INSERT INTO [Product] (title, description, price, categoryId, sellerId, isAuction, condition, createdAt) 
VALUES ('Laptop', 'High-end laptop', 1000.00, 1, 1, 0, 'New', GETDATE());
INSERT INTO [ProductImage] (productId, imageURL, isPrimary) 
VALUES (1, 'https://example.com/laptop.jpg', 1);