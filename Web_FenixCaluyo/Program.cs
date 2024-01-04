using Library.Business.IBusiness;
using Library.Business;
using Library.Log.ILog;
using Library.Log;
using Serilog;
using Library.DataBase.Data.IData;
using Library.DataBase.Data;

namespace Core.WebPase.Reporte;
public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IReport, Report>();
            builder.Services.AddScoped<IReport_Data, Report_Data>();

            var _logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext()
                                           .CreateLogger();

            builder.Logging.AddSerilog(_logger);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Report/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            IWebHostEnvironment env = app.Environment;
            Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../Rotativa/Windows");

            app.Run();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}