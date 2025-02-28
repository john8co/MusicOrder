using MusicOrder.Management;
using MusicOrder.Models;

namespace TestMusicOrder
{
    public class UnitTestYoutubeManagement
    {
        [Fact]
        public async void TestDownloadMusic()
        {
            string videoUrl = @"https://www.youtube.com/watch?v=NiSDXYBYIe8&ab_channel=CYRIL";
            var offer = new ExcelOrder(videoUrl, "Stumblin' In", "CYRIL", "", 0, "");
            string folderPath = @"E:\Musique\MusicOrder";
            Assert.True(await YoutubeManagement.DownloadMusic(offer, folderPath));
        }
        [Fact]
        public async void TestGetTagAsync()
        {
            var test = await TagManagement.GetMetadataListAsync("Stumblin' In", "CYRIL");
            Assert.True(test != null);
        }
    }
}