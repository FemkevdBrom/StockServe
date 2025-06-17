using Serilog;
using Serilog.Events;
using StockServe.Data;
using StockServe.Data.Repository;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Service;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

// Register services
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<DishService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderDishRepository, OrderDishRepository>();
builder.Services.AddScoped<OrderDishService>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<TableService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

// Error handeling Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddScoped<IOrderDishRepository, OrderDishRepository>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information().WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog();


var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
