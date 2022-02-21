using OfficeOpenXml;

namespace DataProcessing;

internal class DataWriter {
    private readonly List<(string, List<double>)> _data;
    private readonly List<string> _headers;

    /// <summary>
    ///     Creates a new datawriter.
    /// </summary>
    /// <param name="data">Data to be written, each row should be the same length as headers.</param>
    /// <param name="headers">The headers of the data.</param>
    /// <exception cref="ArgumentException">Throws if length of headers does not match the length of a row of data.</exception>
    public DataWriter(List<(string, List<double>)> data, List<string> headers) {
        if (data[0].Item2.Count != headers.Count) throw new ArgumentException("Headers count does not match values count");

        if (_data != null && _data.Any(row => row.Item2.Count != _data[0].Item2.Count)) throw new ArgumentException("All values should be of same length.");
        _data = data;
        _headers = headers;
    }

    /// <summary>
    ///     Writes the headers to a file.
    /// </summary>
    /// <param name="sheet">Sheet to write to.</param>
    private void WriteHeaders(ExcelWorksheet sheet) {
        sheet.Cells[1, 1].Value = "name";
        foreach (var i in Enumerable.Range(2, _headers.Count)) sheet.Cells[1, i].Value = _headers[i - 2];
    }

    /// <summary>
    ///     Writes a row to a file.
    /// </summary>
    /// <param name="sheet">Sheet to write to.</param>
    /// <param name="row">Row number.</param>
    /// <param name="name">Name of the sample.</param>
    /// <param name="values">Values of the sample.</param>
    private void WriteRow(ExcelWorksheet sheet, int row, string name, List<double> values) {
        sheet.Cells[row, 1].Value = name;
        foreach (var i in Enumerable.Range(2, values.Count)) sheet.Cells[row, i].Value = values[i - 2];
    }

    /// <summary>
    ///     Writes the stored data to a .xlsx file.
    /// </summary>
    /// <param name="file">Path to the .xlsx to write to.</param>
    /// <param name="sheetName">Name of the sheet to write to.</param>
    /// <param name="separate">True if writing to a new file.</param>
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