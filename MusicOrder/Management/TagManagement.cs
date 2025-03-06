using MusicOrder.Models;
using System.Text.Json;

namespace MusicOrder.Management
{
    public class TagManagement : BaseClass
    {
        private static readonly HttpClient _httpClient = new();

        static TagManagement()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyMusicTagger/1.0 (contact@example.com)");
        }

        public static async Task<List<MusicMetadata>> GetMetadataListAsync(string title, string artist)
        {
            string query = $"https://musicbrainz.org/ws/2/recording/?query=recording:\"{title}\" AND artist:\"{artist}\"&fmt=json&inc=releases+release-groups+tags";

            var metadataList = new List<MusicMetadata>();

            try
            {
                string json = await _httpClient.GetStringAsync(query);
                using var doc = JsonDocument.Parse(json);
                var recordings = doc.RootElement.GetProperty("recordings");

                foreach (var recording in recordings.EnumerateArray())
                {
                    var releases = recording.GetProperty("releases");

                    foreach (var release in releases.EnumerateArray())
                    {
                        var releaseGroup = release.GetProperty("release-group");
                        string primaryType = releaseGroup.GetProperty("primary-type").GetString() ?? "";

                        // Ignorer les compilations
                        if (primaryType.Equals("Compilation", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var metadata = new MusicMetadata
                        {
                            Title = title,
                            Artist = artist,
                            Album = release.GetProperty("title").GetString() ?? "Unknown",
                            Year = ExtractYear(release),
                            Genre = ExtractGenre(releaseGroup),
                            AlbumArtUrl = $"https://coverartarchive.org/release/{release.GetProperty("id").GetString()}/front"
                        };

                        metadataList.Add(metadata);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des métadonnées : {ex.Message}");
            }

            return metadataList;
        }

        private static int ExtractYear(JsonElement release)
        {
            if (release.TryGetProperty("date", out var dateProp))
            {
                string dateStr = dateProp.GetString() ?? "0";
                if (!string.IsNullOrEmpty(dateStr) && dateStr.Length >= 4 && int.TryParse(dateStr.AsSpan(0, 4), out int year))
                    return year;
            }
            return 0;
        }

        private static string ExtractGenre(JsonElement releaseGroup)
        {
            if (releaseGroup.TryGetProperty("tags", out var tags) && tags.GetArrayLength() > 0)
            {
                return tags[0].GetProperty("name").GetString() ?? "Unknown";
            }
            return "Unknown";
        }
    }
}