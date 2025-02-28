using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicOrder.Management;
using MusicOrder.Models;
using Serilog;

// Configurer et charger la configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Configurer le conteneur de services
var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddTransient<ExcelOrders>()
    .BuildServiceProvider();

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Console()
           .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

Log.Information("Application démarre");
var excelOrders = serviceProvider.GetService<ExcelOrders>();
if (excelOrders != null)
{
    excelOrders.SetOrdersList();
    var countOrders = excelOrders.Orders.Count();
    for(int i = 0; i< countOrders; i++)
    {
        await YoutubeManagement.DownloadMusic(excelOrders.Orders[i], configuration["AppSettings:MusicOrderFolder"], i + 1,countOrders);
    }
}
//string folderPath = @"E:\Musique\Tagués";
//Mp3Management.OrderMusicFiles(folderPath);