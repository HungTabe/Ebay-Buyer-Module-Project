USE CloneEbayDB;
GO

-- Danh mục cha
INSERT INTO [Category] (name, parentId) VALUES
(N'Electronics', NULL),
(N'Clothing', NULL),
(N'Books', NULL);

-- Danh mục con cho Electronics
INSERT INTO [Category] (name, parentId) VALUES
(N'Smartphones', 1),
(N'Laptops', 1),
(N'Headphones', 1),
(N'TVs', 1),
(N'Cameras', 1);

-- Danh mục con cho Clothing
INSERT INTO [Category] (name, parentId) VALUES
(N'Mens Clothing', 2),
(N'Womens Clothing', 2),
(N'Shoes', 2),
(N'Accessories', 2);

-- Danh mục con cho Books
INSERT INTO [Category] (name, parentId) VALUES
(N'Fiction', 3),
(N'Non-fiction', 3),
(N'Children''s Books', 3),
(N'Comics', 3),
(N'Textbooks', 3); 

-- Thêm thêm products với đa dạng hơn
INSERT INTO [Product] (title, description, price, images, categoryId, sellerId, isAuction, auctionEndTime, condition, createdAt) VALUES
-- Electronics
('Sony WH-1000XM4 Headphones', 'Wireless noise-canceling headphones with 30-hour battery life', 349.99, 'https://tse4.mm.bing.net/th/id/OIP.B4ZpRhAkszS_lq1SDFfA9gHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3', 1, 3, 0, NULL, 'New', GETDATE()),
('Dell XPS 13 Laptop', '13-inch Ultrabook with Intel i7, 16GB RAM, 512GB SSD', 1299.99, 'https://www.aspartin.com/wp-content/uploads/2021/09/XPS_13_black_open_up_right.0-1024x683.webp', 1, 3, 0, NULL, 'New', GETDATE()),
('Apple Watch Series 7', 'GPS, Always-On Retina display, 41mm', 399.99, 'https://tse3.mm.bing.net/th/id/OIP.TiXZExPhJCC-F8vcr5mgbQHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3', 1, 3, 0, NULL, 'New', GETDATE());

-- Thêm reviews cho các sản phẩm
INSERT INTO [Review] (productId, reviewerId, rating, comment, createdAt) VALUES
(2, 1, 5, 'Amazing sound quality! Best headphones I''ve ever owned.', GETDATE()),
(3, 1, 4, 'Great laptop, very fast. Only giving 4 stars because it''s expensive.', DATEADD(DAY, -1, GETDATE())),
(4, 1, 5, 'Excellent mixer, makes running so much easier!', DATEADD(DAY, -5, GETDATE())); 




















-- Thêm bids cho các auction items
INSERT INTO [Bid] (productId, bidderId, amount, bidTime) VALUES
(13, 1, 15500.00, DATEADD(HOUR, -2, GETDATE())),
(13, 2, 16000.00, DATEADD(HOUR, -1, GETDATE())),
(14, 1, 2600.00, DATEADD(HOUR, -3, GETDATE())),
(14, 2, 2700.00, DATEADD(HOUR, -2, GETDATE())),
(15, 1, 5200.00, DATEADD(HOUR, -1, GETDATE())),
(15, 2, 5300.00, GETDATE());



-- Thêm inventory cho các sản phẩm mới
INSERT INTO [Inventory] (productId, quantity, lastUpdated) VALUES
(6, 25, GETDATE()),
(7, 15, GETDATE()),
(8, 30, GETDATE()),
(9, 50, GETDATE()),
(10, 40, GETDATE()),
(11, 20, GETDATE()),
(12, 35, GETDATE()),
(13, 10, GETDATE()),
(14, 20, GETDATE()),
(15, 15, GETDATE()),
(16, 30, GETDATE()),
(17, 25, GETDATE()),
(18, 20, GETDATE()),
(19, 1, GETDATE()), -- Auction item
(20, 1, GETDATE()), -- Auction item
(21, 1, GETDATE()), -- Auction item
(22, 5, GETDATE()),
(23, 3, GETDATE()),
(24, 2, GETDATE());
