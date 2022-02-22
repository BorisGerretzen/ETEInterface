using System.Data;

namespace Grapher;

public class DataLoader {
    /// <summary>
    ///     Names of the category header
    /// </summary>
    private readonly List<string> _categoryHeaderNames;

    /// <summary>
    ///     Stores all distinct options for each category
    /// </summary>
    private readonly Dictionary<string, HashSet<string>> _categoryOptions;

    private readonly DataTable _table;

    /// <summary>
    ///     Splits the data into the keys (categories) and values of each sample.
    /// </summary>
    /// <param name="table">The DataTable from the excel sheet.</param>
    /// <param name="categoryHeaderNames">A list of the header names of the categories.</param>
    public DataLoader(DataTable table, List<string> categoryHeaderNames) {
        _table = table;
        _categoryHeaderNames = categoryHeaderNames;
        _categoryOptions = _categoryHeaderNames.ToDictionary(name => name, GetCategoryValues);
    }
    //test
    /// <summary>
    ///     Gets a list of all rows where the values in the given categories match the given values.
    /// </summary>
    /// <param name="categoryValues">The column names and values.</param>
    /// <returns>List of all matching rows</returns>
    public List<DataRow> GetWithCategories(Dictionary<string, string> categoryValues) {
        categoryValues = categoryValues.Where(kvp => kvp.Value != "*").ToDictionary(x => x.Key, x => x.Value);
        return _table.AsEnumerable().Where(row => categoryValues.All(kvp => (string)row[kvp.Key] == kvp.Value)).ToList();
        ;
    }

    /// <summary>
    ///     Gets a list of all categories in the table.
    /// </summary>
    /// <returns>List of all categories in the table.</returns>
    public List<string> GetCategories() {
        return _categoryHeaderNames;
    }

    /// <summary>
    ///     Gets all the distinct values in a category.
    /// </summary>
    /// <param name="category">The name of the category</param>
    /// <returns>HashSet of all distinct values found in the category.</returns>
    private HashSet<string> GetCategoryValues(string category) {
        return _table.AsEnumerable().Select(row => (string)row[category]).Distinct().ToHashSet();
    }

    /// <summary>
    ///     Gets all distinct values for all categories.
    /// </summary>
    /// <returns>Dictionary with the name of the category as key and the distinct values as value.</returns>
    public Dictionary<string, HashSet<string>> GetAllCategoryValues() {
        return _categoryOptions;
    }
}