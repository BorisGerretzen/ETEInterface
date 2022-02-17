using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ExcelDataReader;

namespace Grapher {
    public class DataLoader {
        /// <summary>
        /// Dictionary containing all data in a given excel sheet
        /// The key is a list of the category values of a sample
        /// </summary>
        private Dictionary<List<string>, List<double>> _data;

        /// <summary>
        /// Dictionary of the headers of each table
        /// The index is the name of the table
        /// </summary>
        private List<string> _headers;

        /// <summary>
        /// Contains all possible options for each category
        /// Might be null if GetCategoryOptions has not been called
        /// </summary>
        private Dictionary<string, HashSet<string>> _categoryOptions;

        /// <summary>
        /// Names of the category header
        /// </summary>
        private List<string> _categoryHeaderNames;

        /// <summary>
        /// Splits the data into the keys (categories) and values of each sample.
        /// </summary>
        /// <param name="table">The DataTable from the excel sheet.</param>
        /// <param name="categoryHeaderNames">A list of the header names of the categories.</param>
        public DataLoader(DataTable table, List<string> categoryHeaderNames) {
            _categoryHeaderNames = categoryHeaderNames;
            GetData(table);
        }

        public Dictionary<List<string>, List<double>> GetData() {
            return _data;
        }

        /// <summary>
        /// Gets a list of all options for each category
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, HashSet<string>> GetCategoryOptions() {
            if (_categoryOptions == null || _categoryOptions.Count == 0) {
                _categoryOptions = new Dictionary<string, HashSet<string>>();
                var keySet = _data.Keys;
                foreach (var header in _categoryHeaderNames) {
                    _categoryOptions.Add(header, new HashSet<string>());
                }
                foreach (var keys in keySet) {
                    for (int i = 0; i < _categoryHeaderNames.Count; i++) {
                        _categoryOptions[_categoryHeaderNames[i]].Add(keys[i]);
                    }
                }
            }

            return _categoryOptions;
        }

        /// <summary>
        /// Gets the data from the table and processes is to categories and values.
        /// </summary>
        /// <exception cref="ArgumentException">Throws exception if no data could be read from the given file.</exception>
        private void GetData(DataTable table) {
            // Store headers and get indices
            var firstRow = table.Rows[0];
            _data = new();
            _headers = firstRow.ItemArray.Select(elem => (string)elem).ToList();
            var categoryColumnIndices = _categoryHeaderNames.Select(name => _headers.IndexOf(name)).ToHashSet();
            _categoryHeaderNames.Sort((s, s1) => _headers.IndexOf(s)-_headers.IndexOf(s1));
            // Loop through all rows the table
            int index = 0;
            foreach (DataRow? row in table.Rows) {
                if (row == null) {
                    throw new Exception($"Got null from row ${index} (starts at 0).");
                }

                if (index == 0) {
                    index++;
                    continue;
                }

                // For each column, check if it is a category or a value, and add it to respective list
                List<string> categories = new();
                List<double> values = new();
                for (int i = 0; i < table.Columns.Count; i++) {
                    if (categoryColumnIndices.Contains(i)) {
                        categories.Add((string)row[i]);
                    }
                    else {
                        values.Add((double)row[i]);
                    }
                }

                _data.Add(categories, values);
                index++;
            }
        }
    }
}