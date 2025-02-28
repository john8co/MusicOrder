namespace MusicOrder.Models
{
    public class FolderInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public FolderInfo(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
