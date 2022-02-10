using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace DataProcessor
{
    public class DataWriter {
        private List<(string, List<double>)> _data;
        private List<string> _headers;

        public DataWriter(List<(string, List<double>)> data, List<string> headers) {
            if (data[0].Item2.Count != headers.Count) {
                throw new ArgumentException("Headers count does not match values count");
            }

            if (_data != null && _data.Any(row => row.Item2.Count != _data[0].Item2.Count)) {
                throw new ArgumentException("All values should be of same length.");
            }
            this._data = data;
            this._headers = headers;
        }

        private void WriteHeaders(ExcelWorksheet sheet) {
            sheet.Cells[1, 1].Value = "name";
            foreach (int i in Enumerable.Range(2, _headers.Count)) {
                sheet.Cells[1, i].Value = _headers[i-2];
            }
        }

        private void WriteRow(ExcelWorksheet sheet, int row, string name, List<double> values) {
            sheet.Cells[row, 1].Value = name;
            foreach(int i in Enumerable.Range(2, values.Count)) {
                sheet.Cells[row, i].Value = values[i-2];
            }
        }

        public void Write(string file, string sheetName, bool separate) {
            using (var p = new ExcelPackage(new FileInfo(file))) {
                if (p.Workbook.Worksheets[sheetName] != null) {
                    p.Workbook.Worksheets.Delete(sheetName);
                }
                var ws = p.Workbook.Worksheets.Add(sheetName);
                WriteHeaders(ws);
                int ctr = 2;
                foreach (var (name, values) in _data) {
                    WriteRow(ws, ctr, name, values);
                    ctr++;
                }

                if (separate) {
                    p.SaveAs(new FileInfo(file));
                }
                else {
                    p.Save();
                }

                p.Dispose();
            }
        }
    }
}
