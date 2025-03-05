using ClosedXML.Excel;
using MusicOrder.Models;

namespace MusicOrder.Management
{
    public class ExcelManagement : BaseClass, IDisposable
    {
        private XLWorkbook? _wb;
        private int _lastRow;
        private bool _disposed = false;
        public string? ReadCell(int row, int col, int sheetNum = 1)
        {
            return _wb?.Worksheet(sheetNum).Cell(row, col).Value.ToString();
        }
        public void WriteCell(int row, int col, string value, int sheetNum = 1)
        {
            var cell = _wb?.Worksheet(sheetNum).Cell(row, col);
            if(cell !=null)
                cell.Value = value;
        }
        public void Save()
        {
            _wb?.Save();
        }
        public void SaveAs(string filename)
        {
            _wb?.SaveAs(filename, false);
        }
        public void StartReader(string filepath, int sheetNum = 1, int retries = 5, int delayMilliseconds = 1000)
        {
            int attempt = 0;

            try
            {
                _logger.Information("Démarrage de la lecture du fichier Excel.");
                while (attempt < retries)
                {
                    if (!IsFileLocked(filepath))
                    {
                        break;
                    }
                    _logger.Information("Le fichier est verrouillé. Réessai {Attempt}/{Retries} dans {DelaySeconds} secondes...", attempt + 1, retries, delayMilliseconds / 1000);
                    Thread.Sleep(delayMilliseconds);
                    attempt++;
                }
                _wb = new XLWorkbook(filepath);
                var ws = _wb.Worksheet(sheetNum);
                var lastRow = ws.LastRowUsed();
                _lastRow = lastRow?.RowNumber() ?? 0;
                _logger.Information("Fin de la lecture du fichier Excel.");
            }
            catch (FileNotFoundException)
            {
                _logger.Error("Erreur : Le fichier '{Filepath} 'est introuvable.", filepath);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Error("Erreur : Accès non autorisé au fichier '{Filepath}'.", filepath);
            }
            catch (IOException)
            {
                _logger.Error("Erreur : Problème d'accès au fichier '{Filepath}'.", filepath);
            }
            catch (Exception ex)
            {
                _logger.Error("Erreur inattendue : {Message}", ex.Message);
            }
        }
        public int GetLastRow()
        {
            return _lastRow;
        }
        public ExcelOrder GetExcelOrder(int row)
        {
            if (!int.TryParse(ReadCell(row, 5), out int pisteValue))
            {
                _logger.Warning("Failed to parse piste value at row {Row}. Defaulting to 0.", row);
                pisteValue = 0;
            }
            return new ExcelOrder(ReadCell(row, 1), ReadCell(row, 2), ReadCell(row, 3), ReadCell(row, 4), pisteValue, ReadCell(row, 6));
        }
        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                return true; // Le fichier est verrouillé par une autre application
            }
            return false;
        }
        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _wb?.Dispose();
                }
                _disposed = true;
            }
        }
        #endregion
    }
}