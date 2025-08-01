USE CloneEbayDB;
GO

-- Clear existing coupon data (optional - uncomment if needed)
-- DELETE FROM [Coupon];
-- GO

-- Coupon Seed Data for CloneEbay
-- Various types of coupons for testing the coupon functionality

-- 1. General Store-wide Coupons (no productId specified)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Welcome coupon for new users
('WELCOME15', 15.00, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 1000, NULL, 0),
-- Seasonal sale coupon
('SUMMER20', 20.00, GETDATE(), DATEADD(DAY, 60, GETDATE()), 500, NULL, 0),
-- Flash sale coupon (short duration)
('FLASH25', 25.00, GETDATE(), DATEADD(DAY, 7, GETDATE()), 200, NULL, 0),
-- Loyalty coupon
('LOYAL10', 10.00, GETDATE(), DATEADD(MONTH, 6, GETDATE()), 100, NULL, 0);

-- 2. Product-Specific Coupons
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- iPhone specific coupon (Product ID 1)
('IPHONE10', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 1, 0),
-- MacBook specific coupon (Product ID 2)
('MACBOOK15', 15.00, GETDATE(), DATEADD(DAY, 45, GETDATE()), 50, 2, 0),
-- Book specific coupon (Product ID 4)
('BOOK20', 20.00, GETDATE(), DATEADD(DAY, 15, GETDATE()), 50, 4, 5),
-- Clothing specific coupon (Product ID 3)
('FASHION12', 12.00, GETDATE(), DATEADD(DAY, 20, GETDATE()), 200, 3, 0);

-- 3. Time-Limited Coupons (Future start dates)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Black Friday coupon (starts in future)
('BLACKFRIDAY30', 30.00, DATEADD(DAY, 30, GETDATE()), DATEADD(DAY, 37, GETDATE()), 1000, NULL, 0),
-- Cyber Monday coupon
('CYBER25', 25.00, DATEADD(DAY, 35, GETDATE()), DATEADD(DAY, 42, GETDATE()), 800, NULL, 0);

-- 4. Expired Coupons (for testing validation)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Expired coupon
('EXPIRED10', 10.00, DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -1, GETDATE()), 100, NULL, 0),
-- Future coupon (not yet valid)
('FUTURE20', 20.00, DATEADD(DAY, 10, GETDATE()), DATEADD(DAY, 40, GETDATE()), 100, NULL, 0);

-- 5. Limited Usage Coupons (for testing usage limits)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Almost used up coupon
('LIMITED5', 5.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 10, NULL, 8),
-- Single use coupon
('SINGLE15', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, NULL, 0);

-- 6. High Value Coupons (for testing different discount amounts)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- High discount coupon
('MEGA50', 50.00, GETDATE(), DATEADD(DAY, 7, GETDATE()), 50, NULL, 0),
-- Small discount coupon
('MINI5', 5.00, GETDATE(), DATEADD(DAY, 90, GETDATE()), 1000, NULL, 0);

-- 7. Electronics Category Coupons (for testing category-specific logic)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Electronics sale (iPhone and MacBook)
('ELECTRONICS20', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 200, 1, 0),
('TECH15', 15.00, GETDATE(), DATEADD(DAY, 45, GETDATE()), 150, 2, 0);

-- 8. Special Event Coupons
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Holiday season coupon
('HOLIDAY25', 25.00, GETDATE(), DATEADD(DAY, 90, GETDATE()), 500, NULL, 0),
-- New Year coupon
('NEWYEAR30', 30.00, GETDATE(), DATEADD(DAY, 120, GETDATE()), 300, NULL, 0);

-- 9. Testing Coupons (for development and testing)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Test coupon with all fields
('TEST10', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
-- Test coupon for specific product
('TESTPROD15', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 50, 1, 0);

-- 10. Edge Case Coupons
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- No usage limit coupon
('UNLIMITED10', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), NULL, NULL, 0),
-- No end date coupon
('NOEND15', 15.00, GETDATE(), NULL, 100, NULL, 0),
-- Maximum discount coupon
('MAX100', 100.00, GETDATE(), DATEADD(DAY, 7, GETDATE()), 10, NULL, 0);

-- Display summary of inserted coupons
SELECT 
    'Coupon Seed Data Summary' as Info,
    COUNT(*) as TotalCoupons,
    COUNT(CASE WHEN productId IS NULL THEN 1 END) as GeneralCoupons,
    COUNT(CASE WHEN productId IS NOT NULL THEN 1 END) as ProductSpecificCoupons,
    COUNT(CASE WHEN endDate < GETDATE() THEN 1 END) as ExpiredCoupons,
    COUNT(CASE WHEN startDate > GETDATE() THEN 1 END) as FutureCoupons,
    COUNT(CASE WHEN usedCount > 0 THEN 1 END) as UsedCoupons
FROM [Coupon];

-- Display all coupons for verification
SELECT 
    Id,
    Code,
    DiscountPercent,
    StartDate,
    EndDate,
    MaxUsage,
    ProductId,
    UsedCount,
    CASE 
        WHEN EndDate < GETDATE() THEN 'Expired'
        WHEN StartDate > GETDATE() THEN 'Future'
        WHEN MaxUsage IS NOT NULL AND UsedCount >= MaxUsage THEN 'Used Up'
        ELSE 'Active'
    END as Status
FROM [Coupon]
ORDER BY Id; 