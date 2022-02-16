namespace DataProcessing.WinForms;

public class OptionsPanelFactory {
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

    public class OptionsPanel {
        private readonly Dictionary<string, CheckBox> _checkBoxes;
        private readonly GroupBox _panel;

        public OptionsPanel() {
            _checkBoxes = new Dictionary<string, CheckBox>();
            _panel = new GroupBox();
            _panel.Text = "Column Options";
            _panel.Dock = DockStyle.Fill;
        }

        public void AddCheckbox(string header, CheckBox checkBox) {
            checkBox.Location = new Point(10 + checkBox.Location.X, 20 + _checkBoxes.Count * 25);
            _checkBoxes.Add(header, checkBox);

            _panel.Controls.Add(checkBox);
        }

        public GroupBox GetPanel() {
            return _panel;
        }

        public Dictionary<string, bool> GetCheckBoxes() {
            return _checkBoxes.ToDictionary(row => row.Key, row => row.Value.Checked);
        }
    }
}