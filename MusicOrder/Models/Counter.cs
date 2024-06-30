namespace MusicOrder.Models
{
    public class Counter
    {
        public int NewFolderCounter { get; set; }
        public int MoveSongCounter { get; set; }
        public int DeleteExistingCounter { get; set; }
        public Counter() 
        { 
            NewFolderCounter = 0;
            MoveSongCounter = 0;
            DeleteExistingCounter = 0;
        }
    }
}
