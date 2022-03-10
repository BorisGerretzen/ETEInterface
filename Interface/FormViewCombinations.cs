using Grapher;

namespace Interface;

public partial class FormViewCombinations : Form {
    public delegate void TemplateUpdateCallback(GraphTemplate template);

    private readonly TemplateUpdateCallback _callback;
    private readonly GraphTemplate _template;
    private HashSet<string> columns;
    private bool firstEnter = true;

    public FormViewCombinations(GraphTemplate template, Dictionary<string, HashSet<string>> categoryOptions, TemplateUpdateCallback callback) {
        InitializeComponent();
        _template = template;
        LoadTemplate(template, categoryOptions);
        _callback = callback;
    }

    /// <summary>
    ///     Creates a DataTable of the items of this GraphTemplate.
    /// </summary>
    /// <returns>A DataTable of the items in this GraphTemplate.</returns>
    public void LoadTemplate(GraphTemplate template, Dictionary<string, HashSet<string>> categoryOptions) {
        columns = new HashSet<string>();
        // Add the categories twice, once for each series.
        for (var i = 0; i < 2; i++)
            foreach (var category in template.Categories) {
                var combo = new DataGridViewComboBoxColumn();
                var options = categoryOptions[category].ToList();
                options.Add("*");
                var colName = category + i;
                combo.Name = colName;
                combo.HeaderText = colName;
                combo.DataSource = options;
                dataGridView1.Columns.Add(combo);
                columns.Add(category);
            }

        // Add rows to table
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

    /// <summary>
    ///     Updates the items in the current GraphTemplate to the new changes.
    /// </summary>
    private bool TemplateFromGrid() {
        var newItems = new List<GraphTemplate.GraphTemplateItem>();
        var axis = "";
        foreach (DataGridViewRow row in dataGridView1.Rows) {
            if (row.Index == dataGridView1.Rows.Count - 1) continue;

            var item = new GraphTemplate.GraphTemplateItem();

            var options1 = new Dictionary<string, string>();
            var options2 = new Dictionary<string, string>();

            // Update both sets of options, return false if the row is not fully valid yet
            foreach (var column in columns) {
                var col1Name = column + "0";
                var col2Name = column + "1";
                var col1 = (string)row.Cells[col1Name].Value;
                var col2 = (string)row.Cells[col2Name].Value;

                if (axis == "" && (col1 is "*" || col2 is "*")) axis = column;

                if (axis == column && (col1 != null && col1 != "*" || col2 != null && col2 != "*") && !dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.StartsWith(axis))
                    MessageBox.Show($"One column has to be * in the entire table, it is currently '{axis}'.");

                if (axis != column && (col1 is "*" || col2 is "*") && !dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.StartsWith(column))
                    MessageBox.Show($"Only one column can be * in the entire table, it is currently '{axis}'.");

                if (col1 == null || col2 == null) return false;


                options1.Add(column, col1);
                options2.Add(column, col2);
            }

            item.options1 = options1;
            item.options2 = options2;
            newItems.Add(item);
        }

        _template.Items.Clear();
        _template.axis = axis;
        _template.Items.AddRange(newItems);
        return true;
    }

    private void Save() {
        var valid = TemplateFromGrid();
        if (valid) _callback(_template);
    }

    private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
        Save();
    }

    private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
        dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
        dataGridView1.BeginEdit(false);
        if (dataGridView1.EditingControl is ComboBox box) box.DroppedDown = true;
    }

    private void FormViewCombinations_FormClosing(object sender, FormClosingEventArgs e) {
        Save();
    }
}