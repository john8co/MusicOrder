using Microsoft.Extensions.Configuration;
using MusicOrder.Management;
using System.Collections;

namespace MusicOrder.Models
{
    public class ExcelOrder
    {
        public ExcelOrder(string url, string title, string artist, string album, int piste, string genre)
        {
            Url = url;
            Title = title;
            Artist = artist;
            Album = album;
            Piste = piste;
            Genre = genre;
        }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string? Album { get; set; }
        public int? Piste { get; set; }
        public string? Genre { get; set; }
        public string? Status { get; set; }

    }
    public class ExcelOrders
    {
        private readonly IConfiguration _configuration;
        public ExcelOrders(IConfiguration configuration)
        {
            _configuration = configuration;
            Orders = new List<ExcelOrder>();
        }
        public List<ExcelOrder> Orders { get; set; }
        private string GetMusicOrderListPath()
        {
            return Path.Combine(_configuration["AppSettings:MusicOrderFolder"], "MusicOrderList.xlsx");
        }
        public void SetOrdersList()
        {
            using (var xls = new ExcelManagement())
            {
                xls.StartReader(GetMusicOrderListPath(), 1);
                for (int i = 2; i <= xls.GetLastRow(); i++)
                {
                    Orders.Add(xls.GetExcelOrder(i));
                }
            }
        }
    }
}