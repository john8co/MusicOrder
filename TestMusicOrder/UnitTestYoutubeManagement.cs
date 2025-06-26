using MusicOrder.Management;
using MusicOrder.Models;

namespace TestMusicOrder
{
    public class UnitTestYoutubeManagement
    {
        private readonly TagManagement _tagManagement = new();

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
            var test = await _tagManagement.GetMetadataAsync("CYRIL", "Stumblin' In");
            Assert.True(test != null);
        }
    }
}