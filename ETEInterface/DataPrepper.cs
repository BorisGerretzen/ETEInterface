using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ETEInterface
{
    public static class DataPrepper
    {
        private static void Convert(string file, Microsoft.Office.Interop.Excel.Application app)
        {
            var fname = $"{Directory.GetCurrentDirectory()}\\temp\\{Path.GetFileName(file)}x";
            if (File.Exists(fname))
            {
                File.Delete(fname);
            }

            var wb = app.Workbooks.Open(file);
            wb.SaveAs(fname, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            wb.Close();
        }

        public static void PrepTensile(string inputDir, bool recursive, Action<double> progressUpdate) {
            Directory.CreateDirectory("temp");
            SearchOption so = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(inputDir, "*.xls", so);
            int idx = 1;
            var app = new Microsoft.Office.Interop.Excel.Application();
            foreach (string file in files)
            {
                progressUpdate(0.9*idx/files.Length);
                Convert(file, app);
                idx++;
            }
            app.Quit();
        }
    }
}
