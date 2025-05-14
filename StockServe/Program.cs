using StockServe.Data;
using StockServe.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

// Register services
builder.Services.AddScoped<OrderDishService>();
builder.Services.AddScoped<TableService>();

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
