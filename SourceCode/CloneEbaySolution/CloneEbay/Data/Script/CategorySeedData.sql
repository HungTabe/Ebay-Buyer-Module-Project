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

-- Sản phẩm cho Smartphones
INSERT INTO [Product] (name, description, price, categoryId) VALUES
(N'iPhone 14', N'Smartphone cao cấp của Apple', 25000000, 4),
(N'Samsung Galaxy S23', N'Smartphone flagship của Samsung', 22000000, 4);

-- Sản phẩm cho Laptops
INSERT INTO [Product] (name, description, price, categoryId) VALUES
(N'MacBook Pro 16', N'Laptop mạnh mẽ cho dân chuyên nghiệp', 55000000, 5),
(N'Dell XPS 13', N'Laptop mỏng nhẹ, hiệu năng cao', 32000000, 5);

-- Sản phẩm cho Fiction
INSERT INTO [Product] (name, description, price, categoryId) VALUES
(N'The Great Gatsby', N'Tác phẩm kinh điển của F. Scott Fitzgerald', 150000, 13),
(N'1984', N'Tiểu thuyết dystopia của George Orwell', 120000, 13);

-- Sản phẩm cho Mens Clothing
INSERT INTO [Product] (name, description, price, categoryId) VALUES
(N'Ao so mi nam', N'Ao so mi công sở', 350000, 9),
(N'Quan jeans nam', N'Quan jeans thời trang', 450000, 9); 