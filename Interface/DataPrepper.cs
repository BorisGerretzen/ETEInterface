using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using OfficeOpenXml;

namespace Interface; 

public static class DataPrepper {
    public static void Convert(string path) {
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // Auto-detect format, supports:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                // 2. Use the AsDataSet extension method
                var result = reader.AsDataSet();
                // The result of each spreadsheet is in result.Tables
                var fname = $"{Directory.GetCurrentDirectory()}\\temp\\{Path.GetFileName(path)}x";
                using (ExcelPackage pck = new ExcelPackage(fname)) {
                    foreach (DataTable dt in result.Tables) {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dt.TableName);
                        ws.Cells["A1"].LoadFromDataTable(dt);
                    }
                    pck.Save();
                }
            }
        }
    }

    public static void PrepTensile(string inputDir, bool recursive, Action<double> progressUpdate) {
        Directory.CreateDirectory("temp");
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var enc1252 = Encoding.GetEncoding(1252);
        var so = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = Directory.GetFiles(inputDir, "*.xls", so);
        var idx = 1;
        // var app = new Microsoft.Office.Interop.Excel.Application();
        foreach (var file in files) {
            progressUpdate(0.9 * idx / files.Length);
            Convert(file);
            // Convert(file, app);
            idx++;
        }
    }
}