using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace DataProcessor
{
    internal abstract class AbstractDataReader {
        protected string FileName;
        protected string SheetName;

        protected AbstractDataReader(string fileName, string sheetName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileName = fileName;
            SheetName = sheetName;
        }

        protected AbstractDataReader(string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileName = fileName;
        }

        public abstract List<List<(double, double)>> ReadData(string sheetName = "");
    }
}
