using OfficeOpenXml;

namespace DataProcessing.DataReader;

public abstract class AbstractDataReader {
    protected string FileName;
    protected string SheetName;

    protected AbstractDataReader(string fileName, string sheetName) {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        FileName = fileName;
        SheetName = sheetName;
    }

    /// <summary>
    ///     Reads the data from a file
    /// </summary>
    /// <param name="sheetName">Name of the sheet to read from</param>
    /// <returns></returns>
    public abstract List<List<(double, double)>> ReadData(string sheetName = "");
}