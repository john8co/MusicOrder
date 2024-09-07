using ClosedXML.Excel;
using MusicOrder.Models;

namespace MusicOrder
{
    public class ExcelManagement : IDisposable
    {
        private XLWorkbook _wb;
        private int _usedWorksheet = 1;
        private int _lastRow;
        private bool _disposed = false;

        private string ReadCell(int row, int col, int sheetNum)
        {
            return _wb.Worksheet(sheetNum).Cell(row, col).Value.ToString();
        }
        public string ReadCell(int row, int col)
        {
            return ReadCell(row, col, _usedWorksheet);
        }
        private void WriteCell(int row, int col, string value, int sheetNum)
        {
            var cell = _wb.Worksheet(sheetNum).Cell(row, col);
            cell.Value = value;
        }
        public void WriteCell(int row, int col, string value)
        {
            WriteCell(row, col, value, _usedWorksheet);
        }
        public void Save()
        {
           _wb.Save();
        }
        public void SaveAs(string filename)
        {
            _wb.SaveAs(filename, false);
        }
        public void StartReader(string filepath, int sheetNum)
        {
            _wb = new XLWorkbook(filepath);
            var ws = _wb.Worksheet(sheetNum);
            _usedWorksheet = sheetNum;
            _lastRow = ws.LastRowUsed().RowNumber();
        }
        public int GetLastRow()
        {
            return _lastRow;
        }
        public ExcelOrder GetExcelOrder(int row)
        {
            return new ExcelOrder(ReadCell(row, 1), ReadCell(row, 2), ReadCell(row, 3));
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Empêche la finalisation automatique
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Libérer les ressources managées
                    _wb?.Dispose();
                }
                // Libérer les ressources non managées
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
    }
}