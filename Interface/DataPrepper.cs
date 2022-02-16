using System.Data;
using System.Text;
using ExcelDataReader;
using OfficeOpenXml;

namespace Interface;

public static class DataPrepper {
    public static void Convert(string path) {
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
            using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                var result = reader.AsDataSet();
                var fname = $"{Directory.GetCurrentDirectory()}\\temp\\{Path.GetFileName(path)}x";
                using (var pck = new ExcelPackage(fname)) {
                    foreach (DataTable dt in result.Tables) {
                        var ws = pck.Workbook.Worksheets.Add(dt.TableName);
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
        foreach (var file in files) {
            progressUpdate(0.9 * idx / files.Length);
            Convert(file);
            idx++;
        }
    }
}