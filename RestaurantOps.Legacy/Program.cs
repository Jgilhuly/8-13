using Microsoft.EntityFrameworkCore;
using RestaurantOps.Legacy.Data;
using RestaurantOps.Legacy.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF Core DbContext registration (non-breaking addition alongside legacy SqlHelper)
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<RestaurantOpsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Secure configuration management with environment variable override
var environmentConnectionString = Environment.GetEnvironmentVariable("RESTAURANTOPS_DB_CONNECTION");
if (!string.IsNullOrEmpty(environmentConnectionString))
{
    // Override connection string if environment variable is set
    builder.Configuration["ConnectionStrings:Default"] = environmentConnectionString;
}

// Legacy ADO.NET helper initialization
SqlHelper.Initialize(builder.Configuration);

var app = builder.Build();

// Log configuration source after app is built
if (!string.IsNullOrEmpty(environmentConnectionString))
{
    app.Logger.LogInformation("[Config] Using connection string from environment variable");
}

// Basic database connectivity check (non-blocking)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestaurantOpsDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            app.Logger.LogInformation("[Startup] EF Core database connectivity check: SUCCESS");
        }
        else
        {
            app.Logger.LogWarning("[Startup] EF Core database connectivity check: FAILED");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "[Startup] EF Core database connectivity check threw an exception");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Use custom global exception handling middleware for production
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
