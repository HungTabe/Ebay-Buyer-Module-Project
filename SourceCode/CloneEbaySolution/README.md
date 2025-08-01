# CloneEbay Solution

A comprehensive eBay clone built with ASP.NET Core Razor Pages, featuring user authentication, product management, and auction functionality.

## Features

### ✅ Completed Features

#### Authentication & User Management
- User registration with email verification
- User login with JWT authentication
- **Profile management** - Users can update their personal information
- Password change functionality
- Avatar URL support

#### Product Management
- **Product listing with advanced filtering and pagination**
- Search products by name and description
- Filter by category, price range, condition, and auction status
- Sort by name, price, or date (ascending/descending)
- **Product detail pages** with image carousel
- Support for both regular products and auction items
- Bid tracking for auction items

#### Database & Models
- Complete Entity Framework Core setup
- Comprehensive data models for all eBay-like features
- Sample data for testing

### 🚧 In Progress / Planned Features
- Shopping cart functionality
- Order management
- Payment integration
- Real-time bidding
- Seller dashboard
- Review and rating system
- Advanced search with filters

## Getting Started

### Prerequisites
- .NET 6.0 or later
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CloneEbaySolution
   ```

2. **Configure the database connection**
   - Update the connection string in `appsettings.json`
   - Ensure your SQL Server instance is running

3. **Run database migrations**
   ```bash
   cd CloneEbay
   dotnet ef database update
   ```

4. **Seed the database with sample data**
   - Run the SQL scripts in `Data/Script/SeedData.sql`
   - Run the additional data in `Data/Script/AdditionalSeedData.sql`

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - Navigate to `https://localhost:7000` (or the port shown in your terminal)
   - Register a new account or use existing test accounts

### Test Accounts

The following test accounts are available after running the seed data:

**Buyers:**
- Email: `buyer1@example.com` / Password: `password123`
- Email: `buyer2@example.com` / Password: `password123`

**Sellers:**
- Email: `seller1@example.com` / Password: `password123`
- Email: `seller2@example.com` / Password: `password123`

## New Features Documentation

### 1. Profile Management (`/Profile`)

Users can now update their personal information:

- **Update Username**: Change display name
- **Update Email**: Change email address (with duplicate checking)
- **Update Avatar**: Add profile picture URL
- **Change Password**: Optional password update with confirmation

**Features:**
- Form validation with client and server-side validation
- Duplicate email/username checking
- Success/error message display
- Responsive design matching eBay's style

### 2. Product Listing (`/Products`)

Advanced product browsing with comprehensive filtering:

**Search & Filter Options:**
- **Text Search**: Search by product title and description
- **Category Filter**: Filter by product category
- **Price Range**: Set minimum and maximum price
- **Condition Filter**: New, Used, Refurbished
- **Auction Filter**: Show only auction items
- **Sorting**: By name, price, or date (ascending/descending)

**Display Features:**
- **Grid/List View**: Toggle between grid and list layouts
- **Pagination**: Navigate through large product sets
- **Product Cards**: Show key information including:
  - Product image
  - Title and description
  - Price and category
  - Seller information
  - Auction status and bid count
  - Condition

**Responsive Design:**
- Mobile-friendly layout
- Bootstrap-based responsive grid
- Hover effects and animations

### 3. Product Details (`/Product/{id}`)

Detailed product view with:

**Image Gallery:**
- Image carousel for multiple product images
- Thumbnail navigation
- Fallback for products without images

**Product Information:**
- Complete product details
- Seller information
- Condition and category
- Auction details (if applicable)
- Current bid information

**Interactive Elements:**
- Add to cart button
- Add to watchlist
- Place bid (for auction items)
- Breadcrumb navigation

## Technical Implementation

### Architecture
- **ASP.NET Core Razor Pages**: Modern web framework
- **Entity Framework Core**: ORM for database operations
- **JWT Authentication**: Secure user authentication
- **Bootstrap 5**: Responsive UI framework
- **Font Awesome**: Icon library

### Key Services
- `AuthService`: Handles user authentication and profile management
- `ProductService`: Manages product listing, filtering, and details
- `JwtMiddleware`: JWT token processing

### Database Design
- **Users**: User accounts and profiles
- **Products**: Product information and metadata
- **Categories**: Product categorization
- **Bids**: Auction bidding system
- **Reviews**: Product reviews and ratings
- **Orders**: Order management
- **Inventory**: Stock management

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Inspired by eBay's design and functionality
- Built with modern web development best practices
- Uses open-source libraries and frameworks 