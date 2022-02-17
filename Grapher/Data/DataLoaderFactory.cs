using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ExcelDataReader;

namespace Grapher.Data {
    public static class DataLoaderFactory {
        /// <summary>
        /// Get a dataset from an excel file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Dataset containing the sheets in the excel file.</returns>
        /// <exception cref="ArgumentException">Throws exception if no DataSet is read.</exception>
        public static DataSet GetDataSet(string path) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
                using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                    var result = reader.AsDataSet();
                    if (result == null) {
                        throw new ArgumentException($"Could not read from '{path}', does the file exist?");
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Creates a DataLoader from a filepath, sheetname, and list of category names.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="categoryHeaderNames">List of names of the categories.</param>
        /// <returns>A dataloader object.</returns>
        /// <exception cref="ArgumentException">Throws exception if sheet doesn't exist or data could not be read.</exception>
        public static DataLoader FromFile(string path, string sheetName, List<string> categoryHeaderNames) {
            DataSet dataSet = GetDataSet(path);
            if (dataSet.Tables[sheetName] == null) {
                throw new ArgumentException($"Sheet '{sheetName}' does not exist in selected file.");
            }

            return new DataLoader(dataSet.Tables[sheetName], categoryHeaderNames);
        }

        /// <summary>
        /// Creates a list of DataLoaders from a filepath, one for each sheet.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="categoryHeaderNames">List of names of the categories</param>
        /// <returns>A list of DataLoader objects</returns>
        /// <exception cref="ArgumentException">Throws exception if not sheet can be found or no data can be read.</exception>
        public static List<DataLoader> FromAllSheets(string path, List<string> categoryHeaderNames) {
            DataSet dataSet = GetDataSet(path);
            List<DataLoader> dataLoaders = new();
            foreach (DataTable? dataTable in dataSet.Tables) {
                if (dataTable == null) {
                    throw new ArgumentException($"Read null sheet from '{path}'.");
                }
                dataLoaders.Add(new DataLoader(dataTable, categoryHeaderNames));
            }

            return dataLoaders;
        }
    }
}