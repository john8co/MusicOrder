namespace MusicOrder.Models
{
    public class Mp3FileInfo
    {
        public required string FilePath { get; set; }
        public required MusicMetadata Data { get; set; }
    }
    public class MusicMetadata
    {
        public MusicMetadata()
        {
            Title = "track";
            Artist = "artist";
            Album = "Unknown";
            Year = 0;
            Genre = "Unknown";
            TrackNumber = 0;
            AlbumArtUrl = "Unknown";
        }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public uint TrackNumber { get; set; }
        public string AlbumArtUrl { get; set; }
    }
}