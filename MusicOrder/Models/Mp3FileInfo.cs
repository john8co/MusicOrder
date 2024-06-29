namespace MusicOrder.Models
{
    public class Mp3FileInfo
    {
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}, Artist: {Artist}, Album: {Album}, Year: {Year}, Genre: {Genre}";
        }
    }
}