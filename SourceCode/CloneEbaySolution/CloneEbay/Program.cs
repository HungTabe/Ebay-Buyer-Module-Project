using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CloneEbay.Data;
using CloneEbay.Services;
using CloneEbay.Middleware;
using Microsoft.EntityFrameworkCore;
using CloneEbay.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add DbContext
builder.Services.AddDbContext<CloneEbayDbContext>();

// Add AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Add ProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Add OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

// Add AddressService
builder.Services.AddScoped<IAddressService, AddressService>();

// Add CartService
builder.Services.AddScoped<CartService>();

// Register PaymentService and EmailService
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Register plug-in payment and shipping providers
builder.Services.AddSingleton<IPaymentProvider, PayPalPaymentProvider>(sp => new PayPalPaymentProvider());
builder.Services.AddSingleton<IPaymentProvider>(sp => new CodPaymentProvider());
builder.Services.AddSingleton<IShippingProvider, ShippingProvider>(sp => new ShippingProvider());

// Register OrderProcessingService
builder.Services.AddScoped<OrderProcessingService>();

// Add Session and IHttpContextAccessor
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Add IShippingInfoService
builder.Services.AddScoped<IShippingInfoService, ShippingInfoService>();

// Add IOrderTimeoutService
builder.Services.AddScoped<IOrderTimeoutService, OrderTimeoutService>();


// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add CSRF protection
app.UseAntiforgery();

// Add Session middleware
app.UseSession();

// Add JWT middleware to handle cookies
app.UseJwtMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
