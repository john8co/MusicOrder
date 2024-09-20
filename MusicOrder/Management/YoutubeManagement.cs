using MusicOrder.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MusicOrder.Management
{
    public class YoutubeManagement : BaseClass
    {
        public static async Task<bool> DownloadMusic(ExcelOrder order, string folderPath)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(order.Url);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var audioStreamInfo = streamManifest
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            if (audioStreamInfo == null)
            {
                _logger.Error("Impossible de trouver un flux audio pour cette vidéo.");
                return false;
            }
            var filePath = Path.Combine(folderPath, $"{order.Artist} - {SanitizeFileName(order.Title)}.mp3");
            await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath);
            _logger.Information($"Téléchargement terminé : {filePath}");
            return true;
        }
        private static string SanitizeFileName(string text)
        {
            // Liste des caractères à remplacer
            string[] invalidChars = { "?", "*", "<", ">", "|", ":", "\"" };
            foreach (string invalidChar in invalidChars)
            {
                text = text.Replace(invalidChar, "");
            }
            return text;
        }
    }
}