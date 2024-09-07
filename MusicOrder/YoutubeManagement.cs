using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MusicOrder
{
    public class YoutubeManagement
    {
        public static async Task<bool> DownloadMusic(string videoUrl, string folderPath)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var audioStreamInfo = streamManifest
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            if (audioStreamInfo == null)
            {
                Console.WriteLine("Impossible de trouver un flux audio pour cette vidéo.");
                return false;
            }
            var filePath = Path.Combine(folderPath, $"{video.Title}.mp3");
            await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath);
            Console.WriteLine($"Téléchargement terminé : {filePath}");
            return true;
        }
    }
}