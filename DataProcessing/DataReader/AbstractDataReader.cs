using OfficeOpenXml;

namespace DataProcessing.DataReader; 

internal abstract class AbstractDataReader {
    protected string FileName;
    protected string SheetName;

    protected AbstractDataReader(string fileName, string sheetName) {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        FileName = fileName;
        SheetName = sheetName;
    }

    public abstract List<List<(double, double)>> ReadData(string sheetName = "");
}