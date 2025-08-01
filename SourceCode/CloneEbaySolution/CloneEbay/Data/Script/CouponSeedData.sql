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

-- 11. Additional Test Coupons for Comprehensive Testing
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- Quick test coupons (valid for 1 day)
('QUICK10', 10.00, GETDATE(), DATEADD(DAY, 1, GETDATE()), 50, NULL, 0),
('QUICK20', 20.00, GETDATE(), DATEADD(DAY, 1, GETDATE()), 25, NULL, 0),
('QUICK30', 30.00, GETDATE(), DATEADD(DAY, 1, GETDATE()), 10, NULL, 0),

-- Coupons for specific products (assuming more products exist)
('LAPTOP25', 25.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 30, 2, 0),
('PHONE15', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 1, 0),
('BOOKS30', 30.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 50, 4, 0),
('SHOES20', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 75, 3, 0),

-- Coupons with different discount percentages for testing
('DISCOUNT1', 1.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT5', 5.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT10', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT15', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT20', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT25', 25.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT30', 30.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT40', 40.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT50', 50.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT75', 75.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('DISCOUNT90', 90.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),

-- Coupons for testing edge cases
('ZERO0', 0.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('NEGATIVE5', -5.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),
('OVER100', 150.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1000, NULL, 0),

-- Coupons with special characters in code
('TEST-CODE', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('TEST_CODE', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('TEST.CODE', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('TEST@CODE', 25.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),

-- Coupons with very short and very long codes
('A', 5.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('VERYLONGCOUPONCODE123456789', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),

-- Coupons for stress testing
('STRESS1', 1.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 10000, NULL, 0),
('STRESS2', 2.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 10000, NULL, 0),
('STRESS3', 3.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 10000, NULL, 0),

-- Coupons for different time periods
('TODAY', 10.00, GETDATE(), GETDATE(), 100, NULL, 0),
('WEEK', 15.00, GETDATE(), DATEADD(WEEK, 1, GETDATE()), 100, NULL, 0),
('MONTH', 20.00, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 100, NULL, 0),
('YEAR', 25.00, GETDATE(), DATEADD(YEAR, 1, GETDATE()), 100, NULL, 0),

-- Coupons for testing usage tracking
('USED1', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 5, NULL, 1),
('USED2', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 5, NULL, 2),
('USED3', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 5, NULL, 3),
('USED4', 25.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 5, NULL, 4),
('USED5', 30.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 5, NULL, 5),

-- Coupons for testing product-specific validation
('PROD1', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 1, 0),
('PROD2', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 2, 0),
('PROD3', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 3, 0),
('PROD4', 25.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 4, 0),
('PROD5', 30.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 5, 0),

-- Coupons for testing invalid product IDs
('INVALIDPROD', 10.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, 999, 0),
('NEGPROD', 15.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, -1, 0),

-- Coupons for testing different start/end date combinations
('STARTFUTURE', 10.00, DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('ENDTODAY', 15.00, DATEADD(DAY, -30, GETDATE()), GETDATE(), 100, NULL, 0),
('STARTENDTODAY', 20.00, GETDATE(), GETDATE(), 100, NULL, 0),
('STARTENDFUTURE', 25.00, DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 2, GETDATE()), 100, NULL, 0),

-- Coupons for testing null values
('NULLSTART', 10.00, NULL, DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('NULLEND', 15.00, GETDATE(), NULL, 100, NULL, 0),
('NULLUSAGE', 20.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), NULL, NULL, 0),
('NULLPROD', 25.00, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),

-- Coupons for testing decimal precision
('DECIMAL1', 1.5, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('DECIMAL2', 2.75, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('DECIMAL3', 3.33, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('DECIMAL4', 4.99, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0),
('DECIMAL5', 5.01, GETDATE(), DATEADD(DAY, 30, GETDATE()), 100, NULL, 0);

-- 12. Seasonal Coupons (June to September)
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId, usedCount) VALUES
-- June Coupons
('JUNE10', 10.00, '2025-06-01', '2025-06-30', 500, NULL, 0),
('JUNE15', 15.00, '2025-06-01', '2025-06-30', 300, NULL, 0),
('JUNE20', 20.00, '2025-06-01', '2025-06-30', 200, NULL, 0),
('JUNESTART', 12.00, '2025-06-01', '2025-06-15', 150, NULL, 0),
('JUNEEND', 18.00, '2025-06-15', '2025-06-30', 150, NULL, 0),

-- July Coupons
('JULY10', 10.00, '2025-07-01', '2025-07-31', 500, NULL, 0),
('JULY15', 15.00, '2025-07-01', '2025-07-31', 300, NULL, 0),
('JULY20', 20.00, '2025-07-01', '2025-07-31', 200, NULL, 0),
('JULY4TH', 25.00, '2025-07-01', '2025-07-07', 100, NULL, 0),
('JULYMID', 12.00, '2025-07-10', '2025-07-20', 200, NULL, 0),

-- August Coupons
('AUGUST10', 10.00, '2025-08-01', '2025-08-31', 500, NULL, 0),
('AUGUST15', 15.00, '2025-08-01', '2025-08-31', 300, NULL, 0),
('AUGUST20', 20.00, '2025-08-01', '2025-08-31', 200, NULL, 0),
('BACKTOSCHOOL', 30.00, '2025-08-15', '2025-08-31', 100, NULL, 0),
('SUMMEREND', 25.00, '2025-08-20', '2025-08-31', 150, NULL, 0),

-- September Coupons
('SEPTEMBER10', 10.00, '2025-09-01', '2025-09-30', 500, NULL, 0),
('SEPTEMBER15', 15.00, '2025-09-01', '2025-09-30', 300, NULL, 0),
('SEPTEMBER20', 20.00, '2025-09-01', '2025-09-30', 200, NULL, 0),
('FALLSTART', 22.00, '2025-09-01', '2025-09-15', 180, NULL, 0),
('AUTUMN25', 25.00, '2025-09-15', '2025-09-30', 120, NULL, 0),

-- Cross-month Coupons
('SUMMER2025', 20.00, '2025-06-01', '2025-08-31', 1000, NULL, 0),
('Q3SALE', 15.00, '2025-07-01', '2025-09-30', 800, NULL, 0),
('HOLIDAYSEASON', 30.00, '2025-06-15', '2025-09-15', 500, NULL, 0),

-- Product-specific seasonal coupons
('SUMMERPHONE', 15.00, '2025-06-01', '2025-08-31', 200, 1, 0),
('SUMMERLAPTOP', 20.00, '2025-06-01', '2025-08-31', 100, 2, 0),
('SUMMERBOOKS', 25.00, '2025-06-01', '2025-08-31', 150, 4, 0),
('SUMMERFASHION', 18.00, '2025-06-01', '2025-08-31', 300, 3, 0),

-- Monthly special events
('FATHERSDAY', 20.00, '2025-06-15', '2025-06-16', 50, NULL, 0),
('INDEPENDENCEDAY', 25.00, '2025-07-04', '2025-07-04', 100, NULL, 0),
('LABORDAY', 15.00, '2025-09-02', '2025-09-02', 75, NULL, 0),

-- Weekly coupons within months
('JUNEWEEK1', 10.00, '2025-06-01', '2025-06-07', 100, NULL, 0),
('JUNEWEEK2', 12.00, '2025-06-08', '2025-06-14', 100, NULL, 0),
('JUNEWEEK3', 15.00, '2025-06-15', '2025-06-21', 100, NULL, 0),
('JUNEWEEK4', 18.00, '2025-06-22', '2025-06-28', 100, NULL, 0),

('JULYWEEK1', 10.00, '2025-07-01', '2025-07-07', 100, NULL, 0),
('JULYWEEK2', 12.00, '2025-07-08', '2025-07-14', 100, NULL, 0),
('JULYWEEK3', 15.00, '2025-07-15', '2025-07-21', 100, NULL, 0),
('JULYWEEK4', 18.00, '2025-07-22', '2025-07-28', 100, NULL, 0),

('AUGUSTWEEK1', 10.00, '2025-08-01', '2025-08-07', 100, NULL, 0),
('AUGUSTWEEK2', 12.00, '2025-08-08', '2025-08-14', 100, NULL, 0),
('AUGUSTWEEK3', 15.00, '2025-08-15', '2025-08-21', 100, NULL, 0),
('AUGUSTWEEK4', 18.00, '2025-08-22', '2025-08-28', 100, NULL, 0),

('SEPTWEEK1', 10.00, '2025-09-01', '2025-09-07', 100, NULL, 0),
('SEPTWEEK2', 12.00, '2025-09-08', '2025-09-14', 100, NULL, 0),
('SEPTWEEK3', 15.00, '2025-09-15', '2025-09-21', 100, NULL, 0),
('SEPTWEEK4', 18.00, '2025-09-22', '2025-09-28', 100, NULL, 0),

-- Weekend specials
('JUNEWEEKEND', 15.00, '2025-06-29', '2025-06-30', 50, NULL, 0),
('JULYWEEKEND', 15.00, '2025-07-27', '2025-07-28', 50, NULL, 0),
('AUGUSTWEEKEND', 15.00, '2025-08-31', '2025-09-01', 50, NULL, 0),
('SEPTWEEKEND', 15.00, '2025-09-28', '2025-09-29', 50, NULL, 0),

-- Limited time flash sales
('JUNEFLASH', 30.00, '2025-06-15', '2025-06-16', 25, NULL, 0),
('JULYFLASH', 30.00, '2025-07-15', '2025-07-16', 25, NULL, 0),
('AUGUSTFLASH', 30.00, '2025-08-15', '2025-08-16', 25, NULL, 0),
('SEPTFLASH', 30.00, '2025-09-15', '2025-09-16', 25, NULL, 0),

-- Category-specific seasonal coupons
('SUMMERELECTRONICS', 20.00, '2025-06-01', '2025-08-31', 200, 1, 0),
('SUMMERELECTRONICS2', 20.00, '2025-06-01', '2025-08-31', 200, 2, 0),
('SUMMERBOOKSALE', 25.00, '2025-06-01', '2025-08-31', 150, 4, 0),
('SUMMERFASHIONSALE', 18.00, '2025-06-01', '2025-08-31', 300, 3, 0),

-- Gradual discount increase
('JUNEPROGRESSIVE', 5.00, '2025-06-01', '2025-06-10', 100, NULL, 0),
('JUNEPROGRESSIVE2', 10.00, '2025-06-11', '2025-06-20', 100, NULL, 0),
('JUNEPROGRESSIVE3', 15.00, '2025-06-21', '2025-06-30', 100, NULL, 0),

-- Student discounts (back to school season)
('STUDENT10', 10.00, '2025-08-01', '2025-09-30', 500, NULL, 0),
('STUDENT15', 15.00, '2025-08-15', '2025-09-15', 300, NULL, 0),
('STUDENT20', 20.00, '2025-09-01', '2025-09-30', 200, NULL, 0),

-- Holiday preparation coupons
('HOLIDAYPREP', 15.00, '2025-09-01', '2025-09-30', 400, NULL, 0),
('EARLYBIRD', 20.00, '2025-09-15', '2025-09-30', 250, NULL, 0);

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