using CodeWhispererAI.DataAccess;
using CodeWhispererAI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using CodeWhispererAI.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console().WriteTo.File("logs/CodeAI.txt", rollingInterval: RollingInterval.Day).CreateLogger();

try
{
    Log.Information("Starting Code Whisperer AI application");
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    builder.Services.AddSingleton<OpenAIService>();

    // Set up ASP.NET Core Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<CodeWhispererAIContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddDbContext<CodeWhispererAIContext>(
            options =>
                options
                    .UseNpgsql(builder.Configuration["CODEAI_DBCONNECTIONSTRING"])
                    .UseSnakeCaseNamingConvention()
        );


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication(); // This should be after UseRouting
    app.UseAuthorization();  // This should be after UseAuthentication

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

