namespace DataProcessing.WinForms;

/// <summary>
/// Factory to create OptionsPanels
/// </summary>
public class OptionsPanelFactory {

    /// <summary>
    /// Method to create a new options panel from a list of headers.
    /// </summary>
    /// <param name="headers">List of headers to create checkboxes for</param>
    /// <param name="name">Name of the tab</param>
    /// <returns></returns>
    public static OptionsPanel GetOptionsPanel(List<string> headers, string name) {
        var panel = new OptionsPanel();

        foreach (var header in headers) {
            var checkBox = new CheckBox();
            checkBox.Text = header;
            checkBox.Name = $"check{name}{header}";
            panel.AddCheckbox(header, checkBox);
            Console.WriteLine(header);
        }

        return panel;
    }

    /// <summary>
    /// Class to automate the generation of checkboxes for selecting the headers of a test
    /// </summary>
    public class OptionsPanel {
        private readonly Dictionary<string, CheckBox> _checkBoxes;
        private readonly GroupBox _panel;

        public OptionsPanel() {
            _checkBoxes = new Dictionary<string, CheckBox>();
            _panel = new GroupBox();
            _panel.Text = "Column Options";
            _panel.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Adds a new checkbox to the list
        /// </summary>
        /// <param name="header"></param>
        /// <param name="checkBox"></param>
        public void AddCheckbox(string header, CheckBox checkBox) {
            checkBox.Location = new Point(10 + checkBox.Location.X, 20 + _checkBoxes.Count * 25);
            _checkBoxes.Add(header, checkBox);

            _panel.Controls.Add(checkBox);
        }

        /// <summary>
        /// Gets the GroupBox which can be placed in the form
        /// </summary>
        /// <returns></returns>
        public GroupBox GetPanel() {
            return _panel;
        }

        /// <summary>
        /// Gets the headers and the values for the corresponding checkboxes
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetCheckBoxes() {
            return _checkBoxes.ToDictionary(row => row.Key, row => row.Value.Checked);
        }
    }
}