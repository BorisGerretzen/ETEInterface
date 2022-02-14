using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace DataProcessor {
    internal class DataReaderTensile : AbstractDataReader {
        public override List<List<(double, double)>> ReadData(string sheetName = "") {
            if (string.IsNullOrEmpty(sheetName) && string.IsNullOrEmpty(this.SheetName)) {
                throw new ArgumentException("No sheetName passed to function, but there was also no sheetName found in object.");
            }

            sheetName = string.IsNullOrEmpty(sheetName) ? this.SheetName : sheetName;
            List<List<(double, double)>> returnList = new List<List<(double, double)>>();

            using (var package = new ExcelPackage(new FileInfo(this.FileName))) {
                // Try to open sheet
                var sheet = package.Workbook.Worksheets[sheetName];
                if (sheet == null) {
                    throw new ArgumentException($"No sheet with name {sheetName}.");
                }

                // Get dimensions and loop over target cells
                int colCount = sheet.Dimension.End.Column;
                int rowCount = sheet.Dimension.End.Row;
                int firstRow = 4;
                for (int col = 1; col <= colCount; col += 2)
                {
                    var newList = new List<(double, double)>();
                    for (int row = firstRow; row <= rowCount; row++)
                    {
                        // Try to get values
                        var xValue = sheet.Cells[row, col].Value;
                        var yValue = sheet.Cells[row, col + 1].Value;
                        if (xValue == null || yValue == null)
                        {
                            break;
                        }

                        // Add to returnList
                        newList.Add(((double)xValue, (double)yValue));
                    }
                    returnList.Add(newList);
                }
                package.Dispose();
            }
            return returnList;
        }

        public DataReaderTensile(string fileName, string sheetName) : base(fileName, sheetName) { }
        public DataReaderTensile(string fileName) : base(fileName) { }
    }
}