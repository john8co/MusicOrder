using MusicOrder.Management;

namespace MusicOrder.Models
{
    public class ExistingFoldersInfo
    {
        public List<FolderInfo> ExistingFolders { get; set; }
        public List<string> ExistingOnes { get; set; }
        public ExistingFoldersInfo(string folderPath) 
        {
            this.ExistingFolders = FolderManagement.GetexistingFolders(folderPath);
            this.ExistingOnes = this.ExistingFolders.Select(f => f.Name).ToList();
        }
    }
}
