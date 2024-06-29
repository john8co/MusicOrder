using MusicOrder.Models;

namespace MusicOrder
{
    public class FolderManagement
    {
        public static List<FolderInfo> GetexistingFolders(string folderPath)
        {
            var folderPaths = Directory.GetDirectories(folderPath)
                .Select(rep => new FolderInfo(Path.GetFileName(rep), rep)).ToList();
            return folderPaths;
        }
        public static FolderInfo CreateFolder(string folderPath, string nom)
        {

            string nouveaufolderPath = Path.Combine(folderPath, nom);
            Directory.CreateDirectory(nouveaufolderPath);
            return new FolderInfo(nom, nouveaufolderPath);
        }
        public static FolderInfo GetFolderAnime(string folderPath)
        {
            if (Directory.Exists(Path.Combine(folderPath, "Anime")))
                return new FolderInfo("Anime", Path.Combine(folderPath, "Anime"));
            else
                return CreateFolder(folderPath, "Anime");
        }
        public static string GetCleanCriteria(string criteria)
        {
            List<string> toRemove = new List<string>() { "Opening", "Ending" };
            return CleanCriteria(criteria, toRemove);
        }
        private static string CleanCriteria(string criteria, List<string> toRemove)
        {
            foreach (string item in toRemove)
            {
                if (criteria.EndsWith(item, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = criteria.Substring(0, criteria.Length - item.Length).Trim();
                }
            }
            return criteria;
        }
    }
}