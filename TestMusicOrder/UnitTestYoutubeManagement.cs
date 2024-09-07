using MusicOrder;

namespace TestMusicOrder
{
    public class UnitTestYoutubeManagement
    {
        [Fact]
        public async void TestDownloadMusic()
        {
            string videoUrl = @"https://www.youtube.com/watch?v=X4nZ3jivx9A&ab_channel=ChloeMillion";
            string folderPath = @"E:\Musique\MusicOrder";
            Assert.True(await YoutubeManagement.DownloadMusic(videoUrl, folderPath));
        }
    }
}