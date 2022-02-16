using OfficeOpenXml;

namespace DataProcessing;

internal class DataWriter {
    private readonly List<(string, List<double>)> _data;
    private readonly List<string> _headers;

    public DataWriter(List<(string, List<double>)> data, List<string> headers) {
        if (data[0].Item2.Count != headers.Count) throw new ArgumentException("Headers count does not match values count");

        if (_data != null && _data.Any(row => row.Item2.Count != _data[0].Item2.Count)) throw new ArgumentException("All values should be of same length.");
        _data = data;
        _headers = headers;
    }

    private void WriteHeaders(ExcelWorksheet sheet) {
        sheet.Cells[1, 1].Value = "name";
        foreach (var i in Enumerable.Range(2, _headers.Count)) sheet.Cells[1, i].Value = _headers[i - 2];
    }

    private void WriteRow(ExcelWorksheet sheet, int row, string name, List<double> values) {
        sheet.Cells[row, 1].Value = name;
        foreach (var i in Enumerable.Range(2, values.Count)) sheet.Cells[row, i].Value = values[i - 2];
    }

    public void Write(string file, string sheetName, bool separate) {
        using (var p = new ExcelPackage(new FileInfo(file))) {
            if (p.Workbook.Worksheets[sheetName] != null) p.Workbook.Worksheets.Delete(sheetName);
            var ws = p.Workbook.Worksheets.Add(sheetName);
            WriteHeaders(ws);
            var ctr = 2;
            foreach (var (name, values) in _data) {
                WriteRow(ws, ctr, name, values);
                ctr++;
            }

            if (separate)
                p.SaveAs(new FileInfo(file));
            else
                p.Save();

            p.Dispose();
        }
    }
}