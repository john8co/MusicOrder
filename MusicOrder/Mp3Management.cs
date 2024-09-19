using MusicOrder.Models;

namespace MusicOrder
{
    public class Mp3Management : BaseClass
    {
        private static List<Mp3FileInfo> GetAllMp3(string folderPath)
        {
            var result = new List<Mp3FileInfo>();
            try
            {
                var filesMp3 = Directory.GetFiles(folderPath, "*.mp3");
                result.AddRange(filesMp3.Select(GetMp3FileInfo));
            }
            catch (Exception ex)
            {
                _logger.Error($"Une erreur s'est produite lors de la récupération des fichiers MP3 : {ex.Message}");
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
            var counter = new Counter();
            try
            {
                var existingArtists = new ExistingFoldersInfo(folderPath);
                var animeFolders = FolderManagement.GetFolderAnime(folderPath);
                var existingAnimes = new ExistingFoldersInfo(animeFolders.Path);

                foreach (var mp3File in GetAllMp3(folderPath))
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
                _logger.Error($"Une erreur s'est produite dans OrderMusicFiles : {ex.Message}");
            }
            _logger.Information($"{counter.NewFolderCounter} nouvels artistes ajoutés, {counter.MoveSongCounter} chansons triées et {counter.DeleteExistingCounter} doublons supprimés");
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
                var destFilePath = Path.Combine(destinationFolder.Path, Path.GetFileName(criteriaFileParh));
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
                _logger.Error($"Le folderPath de destination pour '{criteria}' n'a pas été trouvé.");
            }
        }
    }
}