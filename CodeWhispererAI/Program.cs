using CodeWhispererAI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console().WriteTo.File("logs/CodeAI.txt", rollingInterval: RollingInterval.Day).CreateLogger();
try
{
    Log.Information("Starting Code Whisperer AI application");
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<CodeWhispererAIContext>(
            options =>
                options
                    .UseNpgsql(builder.Configuration["CODEAI_DBCONNECTIONSTRING"]/*, sqlOptionsBuilder => sqlOptionsBuilder.EnableRetryOnFailure()*/)
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

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

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