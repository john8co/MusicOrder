using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MusicOrder.Models;

public class TagManagement
{
    private readonly Query _musicBrainzQuery;

    public TagManagement()
    {
        // Initialiser avec un User-Agent personnalisé (obligatoire pour MusicBrainz)
        _musicBrainzQuery = new Query("MyMusicApp", "1.0", "contact@example.com");
    }

    public async Task<MusicMetadata> GetMetadataAsync(string artist, string title)
    {
        try
        {
            // Rechercher les enregistrements correspondants
            var searchQuery = $"artist:\"{artist}\" AND recording:\"{title}\"";
            var recordings = await _musicBrainzQuery.FindRecordingsAsync(searchQuery, limit: 10);

            if (recordings.Results?.Count > 0)
            {
                // Prendre le premier résultat (meilleur score)
                var bestMatch = recordings.Results.First().Item;

                // Récupérer les détails complets de l'enregistrement
                var fullRecording = await _musicBrainzQuery.LookupRecordingAsync(bestMatch.Id,
                    Include.Releases | Include.ReleaseGroups | Include.Artists);

                return await BuildMetadataFromRecording(fullRecording);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur MusicBrainz : {ex.Message}");
        }

        // Retourner avec les infos de base si pas trouvé
        return new MusicMetadata { Title = title, Artist = artist };
    }

    private async Task<MusicMetadata> BuildMetadataFromRecording(IRecording recording)
    {
        var metadata = new MusicMetadata
        {
            Title = recording.Title ?? "track",
            Artist = recording.ArtistCredit?.First()?.Name ?? "artist"
        };

        // Récupérer les infos de l'album depuis la première release
        if (recording.Releases?.Count > 0)
        {
            var release = recording.Releases.First();

            metadata.Album = release.Title ?? "Unknown";

            // Année de sortie
            if (release.Date != null)
            {
                metadata.Year = release.Date.Year ?? 0;
            }

            // Numéro de piste
            var medium = release.Media?.FirstOrDefault();
            var track = medium?.Tracks?.FirstOrDefault(t => t.Recording?.Id == recording.Id);
            if (track != null && uint.TryParse(track.Number, out uint trackNum))
            {
                metadata.TrackNumber = trackNum;
            }

            // Récupérer le genre depuis le release group
            if (release.ReleaseGroup != null)
            {
                try
                {
                    var fullReleaseGroup = await _musicBrainzQuery.LookupReleaseGroupAsync(
                        release.ReleaseGroup.Id, Include.Tags);

                    var genre = fullReleaseGroup.Tags?.FirstOrDefault()?.Name;
                    if (!string.IsNullOrEmpty(genre))
                    {
                        metadata.Genre = genre;
                    }
                }
                catch { /* Ignorer si erreur */ }
            }

            // Récupérer l'artwork via Cover Art Archive
            metadata.AlbumArtUrl = await GetAlbumArtUrlAsync(release.Id);
        }

        return metadata;
    }

    private async Task<string> GetAlbumArtUrlAsync(Guid releaseId)
    {
        try
        {
            using var httpClient = new HttpClient();
            var coverArtUrl = $"https://coverartarchive.org/release/{releaseId}";

            var response = await httpClient.GetAsync(coverArtUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var coverArt = System.Text.Json.JsonSerializer.Deserialize<CoverArtResponse>(json);

                return coverArt?.Images?.FirstOrDefault(i => i.Front)?.Image ??
                       coverArt?.Images?.FirstOrDefault()?.Image ?? "Unknown";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur Cover Art : {ex.Message}");
        }

        return "Unknown";
    }
}

// Classes pour deserializer Cover Art Archive API
public class CoverArtResponse
{
    public List<CoverArtImage> Images { get; set; }
}

public class CoverArtImage
{
    public bool Front { get; set; }
    public string Image { get; set; }
}