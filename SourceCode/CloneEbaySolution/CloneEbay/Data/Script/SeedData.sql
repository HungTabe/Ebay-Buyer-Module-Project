USE CloneEbayDB;
GO

-- 1. User (Người mua và người bán)
INSERT INTO [User] (username, email, password, role, isVerified, createdAt) VALUES
('buyer1', 'buyer1@example.com', 'hashedpassword123', 'Buyer', 1, GETDATE()),
('buyer2', 'buyer2@example.com', 'hashedpassword123', 'Buyer', 1, GETDATE()),
('seller1', 'seller1@example.com', 'hashedpassword123', 'Seller', 1, GETDATE()),
('seller2', 'seller2@example.com', 'hashedpassword123', 'Seller', 1, GETDATE());

-- 2. Address (Địa chỉ giao hàng của người mua)
INSERT INTO [Address] (userId, fullName, phone, street, city, state, country, isDefault, postalCode) VALUES
(1, 'Nguyen Van A', '0901234567', '123 Le Loi', 'Ho Chi Minh', 'HCM', 'Vietnam', 1, '700000'),
(1, 'Nguyen Van A', '0901234568', '456 Tran Hung Dao', 'Ho Chi Minh', 'HCM', 'Vietnam', 0, '700000'),
(2, 'Tran Thi B', '0912345678', '789 Nguyen Trai', 'Hanoi', 'HN', 'Vietnam', 1, '100000');

-- 3. Category (Danh mục sản phẩm)
INSERT INTO [Category] (name, parentId) VALUES
('Electronics', NULL),
('Clothing', NULL),
('Books', NULL),
('Smartphones', 1), -- Subcategory của Electronics
('Laptops', 1);

-- 4. Product (Sản phẩm)
INSERT INTO [Product] (title, description, price, images, categoryId, sellerId, isAuction, auctionEndTime, condition, createdAt) VALUES
('iPhone 13', 'Brand new iPhone 13, 128GB', 800.00, '', 4, 3, 0, NULL, 'New', GETDATE()),
('MacBook Pro', '16-inch MacBook Pro, M1 Max', 2000.00, '', 5, 3, 0, NULL, 'New', GETDATE()),
('T-Shirt', 'Cotton T-Shirt, size M', 15.00, '', 2, 4, 0, NULL, 'New', GETDATE()),
('Python Book', 'Learn Python programming', 25.00, '', 3, 4, 0, NULL, 'New', GETDATE()),
('Samsung Galaxy S21', 'Used Galaxy S21, good condition', 500.00, '', 4, 3, 1, DATEADD(DAY, 7, GETDATE()), 'Used', GETDATE());

-- 5. ProductImage (Ảnh sản phẩm)
INSERT INTO [ProductImage] (productId, imageURL, isPrimary) VALUES
(1, 'https://example.com/iphone13.jpg', 1),
(1, 'https://example.com/iphone13_side.jpg', 0),
(2, 'https://example.com/macbookpro.jpg', 1),
(3, 'https://example.com/tshirt.jpg', 1),
(4, 'https://example.com/pythonbook.jpg', 1),
(5, 'https://example.com/galaxys21.jpg', 1);

-- 6. Inventory (Tồn kho sản phẩm)
INSERT INTO [Inventory] (productId, quantity, lastUpdated) VALUES
(1, 50, GETDATE()),
(2, 20, GETDATE()),
(3, 100, GETDATE()),
(4, 30, GETDATE()),
(5, 10, GETDATE());

-- 7. Coupon (Mã giảm giá)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
('SAVE10', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 1, 0), -- Giảm 10% cho iPhone
('BOOK20', 20.00, GETDATE(), DATEADD(DAY, 15, GETDATE()), 50, 4, 5); -- Giảm 20% cho sách

-- 8. OrderTable (Đơn hàng)
INSERT INTO [OrderTable] (buyerId, addressId, orderDate, totalPrice, status, couponId) VALUES
(1, 1, GETDATE(), 815.00, 'Pending', 1), -- Đơn hàng iPhone với mã giảm giá
(2, 3, DATEADD(DAY, -1, GETDATE()), 25.00, 'Delivered', NULL), -- Đơn hàng sách
(1, 2, DATEADD(DAY, -2, GETDATE()), 30.00, 'Processing', NULL); -- Đơn hàng T-Shirt

-- 9. OrderItem (Chi tiết đơn hàng)
INSERT INTO [OrderItem] (orderId, productId, quantity, unitPrice) VALUES
(1, 1, 1, 800.00), -- iPhone trong đơn hàng 1
(2, 4, 1, 25.00), -- Sách trong đơn hàng 2
(3, 3, 2, 15.00); -- 2 T-Shirt trong đơn hàng 3

-- 10. Payment (Thanh toán qua PayOS)
INSERT INTO [Payment] (orderId, userId, amount, method, status, paidAt, transactionId) VALUES
(1, 1, 815.00, 'PayOS', 'Completed', GETDATE(), 'TXN123456'),
(2, 2, 25.00, 'COD', 'Completed', DATEADD(DAY, -1, GETDATE()), NULL),
(3, 1, 30.00, 'PayOS', 'Pending', DATEADD(DAY, -2, GETDATE()), 'TXN789012');

-- 11. Review (Đánh giá sản phẩm)
INSERT INTO [Review] (productId, reviewerId, rating, comment, createdAt) VALUES
(1, 1, 5, 'Great phone, fast delivery!', GETDATE()),
(4, 2, 4, 'Good book, but cover slightly damaged.', DATEADD(DAY, -1, GETDATE())),
(3, 1, 3, 'T-Shirt is okay, size a bit small.', DATEADD(DAY, -2, GETDATE()));

-- 12. Notification (Thông báo hệ thống)
INSERT INTO [Notification] (userId, content, type, isRead, createdAt) VALUES
(1, 'Your order #1 has been placed successfully.', 'Order', 0, GETDATE()),
(1, 'Your payment for order #1 was successful.', 'Payment', 0, GETDATE()),
(2, 'Order #2 has been delivered.', 'Order', 1, DATEADD(DAY, -1, GETDATE())),
(1, 'New coupon available for iPhone 13!', 'Promotion', 0, DATEADD(DAY, -3, GETDATE()));

-- 13. ReturnRequest (Yêu cầu hoàn trả)
INSERT INTO [ReturnRequest] (orderId, userId, reason, status, createdAt) VALUES
(2, 2, 'Book cover damaged', 'Pending', DATEADD(DAY, -1, GETDATE()));

-- 14. Bid (Đấu giá cho sản phẩm Samsung Galaxy S21)
INSERT INTO [Bid] (productId, bidderId, amount, bidTime) VALUES
(5, 1, 510.00, GETDATE()),
(5, 2, 520.00, DATEADD(HOUR, 1, GETDATE()));