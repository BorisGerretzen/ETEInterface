using Grapher;

namespace Interface; 

public partial class FormViewCombinations : Form {
    private readonly GraphTemplate _template;

    public FormViewCombinations(GraphTemplate template, Dictionary<string, HashSet<string>> categoryOptions) {
        InitializeComponent();
        _template = template;
        LoadTemplate(template, categoryOptions);
    }

    /// <summary>
    ///     Creates a DataTable of the items of this GraphTemplate.
    /// </summary>
    /// <returns>A DataTable of the items in this GraphTemplate.</returns>
    public void LoadTemplate(GraphTemplate template, Dictionary<string, HashSet<string>> categoryOptions) {
        // Add the categories twice, once for each series.
        for (var i = 0; i < 2; i++)
            foreach (var category in template.Categories) {
                var combo = new DataGridViewComboBoxColumn();
                var options = categoryOptions[category].ToList();
                options.Add("*");
                combo.Name = category + i;
                combo.HeaderText = category + i;
                combo.DataSource = options;
                dataGridView1.Columns.Add(combo);
            }

        foreach (var item in template.Items) {
            var row = new object[template.Categories.Count * 2];
            var idx = 0;

            foreach (var category in template.Categories) {
                row[idx] = item.options1[category];
                idx++;
            }

            foreach (var category in template.Categories) {
                row[idx] = item.options2[category];
                idx++;
            }

            dataGridView1.Rows.Add(row);
        }
    }
}