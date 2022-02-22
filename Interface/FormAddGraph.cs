namespace Interface;

public partial class FormAddGraph : Form {
    /// <summary>
    ///     Delegate type that gets called whenever this form reports a new combination to be added.
    /// </summary>
    /// <param name="filters1">Filters for the first series in the graph.</param>
    /// <param name="filters2">Filters for the second series in the graph.</param>
    /// <param name="axis">Name of category that should be placed on the x axis.</param>
    public delegate void ResultCallback(Dictionary<string, string> filters1, Dictionary<string, string> filters2, string axis);

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
        var axis = string.Empty;
        foreach (var category in categories) {
            var option1 = (string)combos[$"{category}1"].SelectedItem;
            var option2 = (string)combos[$"{category}2"].SelectedItem;

            // If both options are * it is the axis
            if (option1 == "*" && option2 == "*") {
                axis = category;
            }
            // If only one is * we have a problem
            else if (option1 == "*" || option2 == "*") {
                MessageBox.Show("'*' must be in the same category for both series.");
                return;
            }
            // If none are * just add it to filters
            else {
                options1[category] = option1;
                options2[category] = option2;
            }
        }

        // If there is no axis found show a message.
        if (axis == string.Empty) {
            MessageBox.Show("No '*' found in any column.\r\n If you need help, there is a tutorial for the graphing utility on the github page, it can be found on the help page on the main menu.");
            return;
        }

        _callback(options1, options2, axis);
        Close();
    }
}