using ClosedXML.Excel;
using MusicOrder.Models;

namespace MusicOrder.Management
{
    public class ExcelManagement : BaseClass, IDisposable
    {
        private XLWorkbook _wb;
        private int _lastRow;
        private bool _disposed = false;
        public string ReadCell(int row, int col, int sheetNum = 1)
        {
            return _wb.Worksheet(sheetNum).Cell(row, col).Value.ToString();
        }
        public void WriteCell(int row, int col, string value, int sheetNum = 1)
        {
            var cell = _wb.Worksheet(sheetNum).Cell(row, col);
            cell.Value = value;
        }
        public void Save()
        {
            _wb.Save();
        }
        public void SaveAs(string filename)
        {
            _wb.SaveAs(filename, false);
        }
        public void StartReader(string filepath, int sheetNum = 1, int retries = 5, int delayMilliseconds = 1000)
        {
            int attempt = 0;
            bool fileLocked = false;

            try
            {
                _logger.Information("Démarrage de la lecture du fichier Excel.");
                while (attempt < retries)
                {
                    if (!IsFileLocked(filepath))
                    {
                        break;
                    }
                    _logger.Information($"Le fichier est verrouillé. Réessai {attempt + 1}/{retries} dans {delayMilliseconds / 1000} secondes...");
                    Thread.Sleep(delayMilliseconds);
                    attempt++;
                }
                _wb = new XLWorkbook(filepath);
                var ws = _wb.Worksheet(sheetNum);
                _lastRow = ws.LastRowUsed().RowNumber();
                _logger.Information("Fin de la lecture du fichier Excel.");
            }
            catch (FileNotFoundException)
            {
                _logger.Error($"Erreur : Le fichier '{filepath}' est introuvable.");
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Error($"Erreur : Accès non autorisé au fichier '{filepath}'.");
            }
            catch (IOException)
            {
                if (fileLocked)
                {
                    _logger.Error($"Erreur : Le fichier est verrouillé après {retries} tentatives.");
                }
                else
                {
                    _logger.Error($"Erreur : Problème d'accès au fichier '{filepath}'.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Erreur inattendue : {ex.Message}");
            }
        }
        public int GetLastRow()
        {
            return _lastRow;
        }
        public ExcelOrder GetExcelOrder(int row)
        {
            int.TryParse(ReadCell(row, 5), out int pisteValue);
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
        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ExcelManagement));
            }
        }
        #endregion
    }
}