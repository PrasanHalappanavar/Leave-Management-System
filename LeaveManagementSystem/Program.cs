using DL_LeaveManagementSystem;
using DL_LeaveManagementSystem.Abstract;
using DL_LeaveManagementSystem.Model;
using DL_LeaveManagementSystem.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/lms-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Application starting up");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Host.UseSerilog();

    var connectionString = builder.Configuration.GetConnectionString("LMSConnection");

    // initialize singleton with connection string
    AppConfig.GetInstance(connectionString);

    builder.Services.AddMemoryCache();

    builder.Services.AddDbContext<LMS_DbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();

    builder.Services.AddSingleton<EmailHelper>(provider =>
    {
        var config = provider.GetRequiredService<IConfiguration>();
        return EmailHelper.GetEmailInstance(config);
    });

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
    });


    builder.Services.AddSingleton<CacheService>(provider =>
    {
        var memCache = provider.GetRequiredService<IMemoryCache>();
        return CacheService.GetCacheInstance(memCache);
    });

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseSession();

    app.Use(async (context, next) =>
    {
        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        await next();
    });

    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}")
        .WithStaticAssets();

    Log.Information("Application started successfully");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}