using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETEInterface
{
    public static class DataPrepper
    {
        private static void Convert(string file)
        {
            var fname = $"{Directory.GetCurrentDirectory()}\\temp\\{Path.GetFileName(file)}x";
            if (File.Exists(fname))
            {
                File.Delete(fname);
            }

            var app = new Microsoft.Office.Interop.Excel.Application();
            var wb = app.Workbooks.Open(file);
            wb.SaveAs(fname, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            wb.Close();
            app.Quit();
        }

        public static void PrepTensile(string inputDir, bool recursive) {
            Directory.CreateDirectory("temp");
            SearchOption so = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (string file in Directory.GetFiles(inputDir, "*.xls", so))
            {
                Convert(file);
            }

        }
    }
}
