using Microsoft.Extensions.Configuration;
using MusicOrder.Management;

namespace MusicOrder.Models
{
    public class ExcelOrder(string url, string title, string artist, string album, int piste, string genre)
    {
        public string Url { get; set; } = url;
        public string Title { get; set; } = title;
        public string Artist { get; set; } = artist;
        public string? Album { get; set; } = album;
        public int? Piste { get; set; } = piste;
        public string? Genre { get; set; } = genre;
        public string? Status { get; set; }

    }
    public class ExcelOrders(IConfiguration configuration) : BaseClass
    {
        private readonly IConfiguration _configuration = configuration;

        public List<ExcelOrder> Orders { get; set; } = [];
        private string GetMusicOrderListPath()
        {
            string? folder = _configuration["AppSettings:MusicOrderFolder"];
            if (!string.IsNullOrWhiteSpace(folder))
                return Path.Combine(folder, "MusicOrderList.xlsx");
            else
            {
                string message = "AppSettings:MusicOrderFolder is null";
                _logger.Error(message);
                throw new ArgumentNullException(message);
            }
        }
        public void SetOrdersList()
        {
            using var xls = new ExcelManagement();
            xls.StartReader(GetMusicOrderListPath(), 1);
            for (int i = 2; i <= xls.GetLastRow(); i++)
            {
                Orders.Add(xls.GetExcelOrder(i));
            }
        }
    }
}