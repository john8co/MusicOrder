using MusicOrder;
using MusicOrder.Models;

namespace TestMusicOrder
{
    public class UnitTestYoutubeManagement
    {
        [Fact]
        public async void TestDownloadMusic()
        {
            string videoUrl = @"https://www.youtube.com/watch?v=NiSDXYBYIe8&ab_channel=CYRIL";
            var offer = new ExcelOrder(videoUrl, "Stumblin' In", "CYRIL");
            string folderPath = @"E:\Musique\MusicOrder";
            Assert.True(await YoutubeManagement.DownloadMusic(offer, folderPath));
        }
    }
}