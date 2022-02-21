namespace Interface; 

public partial class FormAddGraph : Form {
    public delegate void ResultCallback(Dictionary<string, string> options1, Dictionary<string, string> options2);

    private readonly ResultCallback _callback;
    private readonly List<string> categories;

    private readonly Dictionary<string, ComboBox> combos;

    public FormAddGraph(Dictionary<string, HashSet<string>> categoryOptions, ResultCallback callback) {
        InitializeComponent();
        _callback = callback;
        combos = new Dictionary<string, ComboBox>();
        categories = categoryOptions.Keys.ToList();

        // Place controls for first item
        var i = 0;
        foreach (var kvp in categoryOptions) {
            // Label for category
            var label = new Label();
            label.Text = kvp.Key;
            label.Location = new Point(10, 10 + i * 25);
            panel1.Controls.Add(label);

            // Combo for options
            var combo = new ComboBox();
            combo.Items.AddRange(kvp.Value.ToArray());
            combo.Items.Add("*");
            combo.SelectedItem = "*";
            combo.Location = new Point(200, 10 + i * 25);
            panel1.Controls.Add(combo);
            combos.Add($"{kvp.Key}1", combo);
            i++;
        }

        // Place controls for 2nd item
        i++;
        foreach (var kvp in categoryOptions) {
            // Label for category
            var label = new Label();
            label.Text = kvp.Key;
            label.Location = new Point(10, 10 + i * 25);
            panel1.Controls.Add(label);

            // Combo for options
            var combo = new ComboBox();
            combo.Items.AddRange(kvp.Value.ToArray());
            combo.Items.Add("*");
            combo.SelectedItem = "*";
            combo.Location = new Point(200, 10 + i * 25);
            panel1.Controls.Add(combo);
            combos.Add($"{kvp.Key}2", combo);

            i++;
        }
    }

    /// <summary>
    ///     Get all results from the combos and send it to the callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDone_Click(object sender, EventArgs e) {
        Dictionary<string, string> options1 = new();
        Dictionary<string, string> options2 = new();

        foreach (var category in categories) {
            var option1 = (string)combos[$"{category}1"].SelectedItem;
            var option2 = (string)combos[$"{category}2"].SelectedItem;
            options1[category] = option1;
            options2[category] = option2;
        }

        _callback(options1, options2);
        Close();
    }
}