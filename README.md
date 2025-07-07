# Ebay-Buyer-Module-Project - Group 1 (Buyer)

## Project Overview
CloneEbay is an e-commerce platform mimicking eBay's functionality, built for buyers with RazorPage for both frontend and backend, integrated with PayOS for payments. Group 1 focuses on buyer features, ensuring a secure, responsive, and scalable system.

## Features
- **User Management**: Register (email, password, email verification), login/logout, update personal info.
- **Product Browsing**: List products with pagination, filter by category/price/name, view product details (images, description, seller, reviews).
- **Cart & Checkout**: Add products to cart (local & server), create orders, select shipping address (multiple, default option).
- **Payment**: Pay via PayOS (simulated PayPal, COD).
- **Order Management**: View order history (status, details), request returns.
- **Reviews & Coupons**: Rate/comment products, apply coupons.
- **Notifications**: System alerts for orders, promotions, responses.
- **Performance**: Search & product page load <1s, responsive UI for mobile.
- **Security**: Password hashing, JWT, CSRF protection, SSL.
- **Scalability**: Handle high concurrent users.
- **Debugging**: Detailed client/server error logging.

## Tech Stack
- **Frontend & Backend**: RazorPage
- **Database**: MS SQL Server
- **Payment Gateway**: PayOS
- **Infrastructure**: Load balancing (Nginx), Kubernetes (K8s) for zero downtime
- **CI/CD**: Jenkins, GitLab Actions
- **Rate Limiting**: Prevent request spam
- **Testing**: JMeter for load/security testing
- **Project Management**: Jira, story points for task estimation

## Database Schema
Below is the MS SQL database schema with additional tables/fields for enhanced functionality.

```sql
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
    [createdAt] DATETIME DEFAULT GETDATE() -- Added for tracking
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
    [images] NVARCHAR(MAX),
    [categoryId] INT FOREIGN KEY REFERENCES [Category](id),
    [sellerId] INT FOREIGN KEY REFERENCES [User](id),
    [isAuction] BIT,
    [auctionEndTime] DATETIME,
    [condition] NVARCHAR(50), -- Added (new/used/refurbished)
    [createdAt] DATETIME DEFAULT GETDATE()
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
    [transactionId] NVARCHAR(100) -- Added for PayOS transaction tracking
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
    [type] NVARCHAR(50), -- Order, promotion, response
    [isRead] BIT DEFAULT 0,
    [createdAt] DATETIME
);
```

### Additional Database Notes
- Added `isVerified` and `createdAt` to `User` for email verification and tracking.
- Added `postalCode` to `Address` for precise shipping.
- Added `parentId` to `Category` for subcategories.
- Added `condition` and `createdAt` to `Product` for item status and listing time.
- Added `couponId` to `OrderTable` for coupon application.
- Added `transactionId` to `Payment` for PayOS integration.
- Added `isRead` to `Message` for read status.
- Added `usedCount` to `Coupon` for usage tracking.
- Added `Notification` table for system alerts (orders, promotions).

## Setup Instructions
1. **Database Setup**:
   - Install MS SQL Server.
   - Run the provided SQL script to create `CloneEbayDB` and tables.
2. **Configure Environment**:
   - Update `appsettings.json` with PayOS API keys, database connection string, and JWT settings.
3. **Run Application**:
   ```bash
   dotnet restore
   dotnet run
   ```
5. **CI/CD**:
   - Configure Jenkins/GitLab Actions for automated builds and deployments.
6. **Testing**:
   - Use JMeter for load and security testing.

## Deployment
- **Infrastructure**: Deploy with Nginx for load balancing, K8s for zero downtime.
- **Rate Limiting**: Configure to prevent request spam.
- **Monitoring**: Log client/server errors for debugging.

## Notes
- Mimics eBay's buyer experience with secure, fast, and responsive design.
- Scalable for high user traffic, with robust logging for debugging.
- PayOS integration ensures smooth payment processing.
