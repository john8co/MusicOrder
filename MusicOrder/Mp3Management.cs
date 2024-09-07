using MusicOrder.Models;

namespace MusicOrder
{
    public class Mp3Management
    {
        private static List<Mp3FileInfo> GetAllMp3(string folderPath)
        {
            List<Mp3FileInfo> result = new List<Mp3FileInfo>();
            try
            {
                string[] filesMp3 = Directory.GetFiles(folderPath, "*.mp3");
                foreach (string file in filesMp3)
                {
                    var infoMp3 = GetMp3FileInfo(file);
                    result.Add(infoMp3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }
            return result;
        }
        private static Mp3FileInfo GetMp3FileInfo(string folderPath)
        {
            var file = TagLib.File.Create(folderPath);
            return new Mp3FileInfo
            {
                FilePath = folderPath,
                Title = file.Tag.Title,
                Artist = string.Join(", ", file.Tag.Performers).Trim(),
                Album = file.Tag.Album,
                Year = (int)file.Tag.Year,
                Genre = string.Join(", ", file.Tag.Genres)                
            };
        }        

        public static void OrderMusicFiles(string folderPath)
        {
            Counter counter = new Counter();
            try
            {
                ExistingFoldersInfo existingArtists = new ExistingFoldersInfo(folderPath);
                FolderInfo animeFolders = FolderManagement.GetFolderAnime(folderPath);
                ExistingFoldersInfo existingAnimes = new ExistingFoldersInfo(animeFolders.Path);

                foreach (Mp3FileInfo mp3File in GetAllMp3(folderPath))
                {
                    if (mp3File.Genre != "Anime")
                    {
                        ManageMp3File(ref existingArtists, mp3File.Artist, folderPath, mp3File.FilePath, ref counter);
                    }
                    else
                    {
                        ManageMp3File(ref existingAnimes, mp3File.Album, animeFolders.Path, mp3File.FilePath, ref counter);
                    }
                }            
            }            
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite dans OrderMusicFiles : {ex.Message}");
            }
            Console.WriteLine($"{counter.NewFolderCounter} nouvels artistes ajoutés, {counter.MoveSongCounter} chansons triées et {counter.DeleteExistingCounter} doublons supprimés");
        }
        private static void ManageMp3File(ref ExistingFoldersInfo existings, string criteria, string folderPath, string criteriaFileParh,ref Counter counter)
        {
            criteria = FolderManagement.GetCleanCriteria(criteria);
            if (!existings.ExistingOnes.Contains(criteria))
            {
                var newFolder = FolderManagement.CreateFolder(folderPath, criteria);
                counter.NewFolderCounter++;
                existings.ExistingFolders.Add(newFolder);
                existings.ExistingOnes.Add(criteria);
            }
            var destinationFolder = existings.ExistingFolders.FirstOrDefault(f => f.Name == criteria);
            if (destinationFolder != null)
            {
                string destFilePath = Path.Combine(destinationFolder.Path, Path.GetFileName(criteriaFileParh));

                if (!File.Exists(destFilePath))
                {
                    File.Move(criteriaFileParh, destFilePath);
                    counter.MoveSongCounter++;
                }
                else
                {
                    File.Delete(criteriaFileParh);
                    counter.DeleteExistingCounter++;
                }
            }
            else
            {
                Console.WriteLine($"Le folderPath de destination pour '{criteria}' n'a pas été trouvé.");
            }
        }
    }
}