using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DataProcessing.DataProcessor;
using DataProcessing.WinForms;
using Grapher;
using Grapher.Data;
using Grapher.Graph;
using Newtonsoft.Json;

namespace Interface;

public partial class FormMain : Form {
    private readonly OptionsPanelFactory.OptionsPanel _optionsPanelTear;
    private readonly OptionsPanelFactory.OptionsPanel _optionsPanelTensile;
    private DataLoader _dataLoader;
    private DataSet _dataSet;
    private string _inputDirectory = null!;
    private string _inputFile;
    private string _outputFile = null!;
    private GraphTemplate _template;
    private Thread _worker = null!;

    public FormMain() {
        InitializeComponent();
        SetGraphControls(false);

        _optionsPanelTensile = OptionsPanelFactory.GetOptionsPanel(TensileProcessor.Empty.GetHeaders(), "rebound");
        _optionsPanelTear = OptionsPanelFactory.GetOptionsPanel(TearProcessor.Empty.GetHeaders(), "rebound");
        // var optionsPanelRebound = OptionsPanelFactory.GetOptionsPanel(TensileProcessor.Empty.GetHeaders(), "rebound");

        tableTensile.GetControlFromPosition(0, 0).Controls.Add(_optionsPanelTensile.GetPanel());
        tableTear.GetControlFromPosition(0, 0).Controls.Add(_optionsPanelTear.GetPanel());
        // tableRebound.GetControlFromPosition(0,0).Controls.Add(optionsPanelRebound.GetPanel());
    }

    /// <summary>
    ///     Sets the progress bar to a set point
    /// </summary>
    /// <param name="amount">Point to set the progress bar to (0-1)</param>
    private void ProgressUpdate(double amount) {
        progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (int)Math.Floor(amount * 100); });
    }


    private void btnExport_Click(object sender, EventArgs e) {
        // Dont check options when graph tab is selected
        if (tabControlMain.SelectedTab != tabGraphs) {
            // Check if an export is already running
            if (_worker != null && _worker.IsAlive) {
                var dialogResult = MessageBox.Show("An export is already running, do you want to cancel it?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialogResult != DialogResult.Yes) return;

                _worker.Abort();
                Directory.Delete("temp", true);
            }

            // Check if input and output paths are set
            if (string.IsNullOrEmpty(_inputDirectory) || string.IsNullOrEmpty(_outputFile)) {
                MessageBox.Show("Please specify an input directory and output file using the labeled buttons,", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Tensile
        if (tabControlMain.SelectedTab == tabTensile) {
            _worker = new Thread(() => {
                DataPrepper.PrepTensile(_inputDirectory, checkRecursive.Checked, ProgressUpdate);
                AbstractProcessor processor = new TensileProcessor("temp", radioSeparate.Checked);
                processor.SetHeadersActive(_optionsPanelTensile.GetCheckBoxes());
                processor.Process(_outputFile);
                Directory.Delete("temp", true);
                ProgressUpdate(1);
                MessageBox.Show("Export complete");
            });
            _worker.Start();
        }
        // Tear
        else if (tabControlMain.SelectedTab == tabTear) {
            _worker = new Thread(() => {
                DataPrepper.PrepTensile(_inputDirectory, checkRecursive.Checked, ProgressUpdate);
                AbstractProcessor processor = new TearProcessor("temp", radioSeparate.Checked);
                processor.SetHeadersActive(_optionsPanelTear.GetCheckBoxes());
                processor.Process(_outputFile);
                Directory.Delete("temp", true);
                ProgressUpdate(1);
                MessageBox.Show("Export complete");
            });
            _worker.Start();
        }
        // Graphs
        else if (tabControlMain.SelectedTab == tabGraphs) {
            GenerateGraph();
        }
    }

    /// <summary>
    ///     Called when the user presses the select input directory button.
    ///     Opens a FolderBrowserDialog where the user can select the directory where the files are located.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSelectInput_Click(object sender, EventArgs e) {
        using (var fbd = new FolderBrowserDialog()) {
            fbd.SelectedPath = Directory.GetCurrentDirectory();
            var result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                _inputDirectory = fbd.SelectedPath;
                btnSelectInput.Text = fbd.SelectedPath;
            }
        }
    }

    /// <summary>
    ///     Called when the user presses the select output file button.
    ///     Either opens a SaveFileDialog or an OpenFileDialog depending on which radio is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSelectOutput_Click(object sender, EventArgs e) {
        if (radioSeparate.Checked)
            using (var sfd = new SaveFileDialog()) {
                sfd.Filter = "Excel workbook|*.xlsx";
                var result = sfd.ShowDialog();
                if (result == DialogResult.OK) {
                    _outputFile = sfd.FileName;
                    btnSelectOutput.Text = sfd.FileName;
                }
            }
        else if (radioSheet.Checked)
            using (var ofd = new OpenFileDialog()) {
                ofd.Filter = "Excel workbook|*.xlsx";
                var result = ofd.ShowDialog();
                if (result == DialogResult.OK) {
                    _outputFile = ofd.FileName;
                    btnSelectOutput.Text = ofd.FileName;
                }
            }
    }

    /// <summary>
    ///     Called when the email link is pressed, copies the email to the clipboard.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void linkEmail_Click(object sender, EventArgs e) {
        Clipboard.SetText(linkEmail.Text);
    }

    /// <summary>
    ///     Called whenever the separate file/add as sheets radios change.
    ///     Clears the output file.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radioSeparate_CheckedChanged(object sender, EventArgs e) {
        btnSelectOutput.Text = "Select output file";
        _outputFile = string.Empty;
    }

    /// <summary>
    ///     Called when the user presses the github link.
    ///     Opens the page in their default browser.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
        Process.Start("explorer", linkGithub.Text);
    }

    /// <summary>
    ///     Called when the main tab changes.
    ///     Disables export options for the graphs export.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e) {
        if (tabControlMain.SelectedTab == tabGraphs)
            groupExport.Enabled = false;
        else
            groupExport.Enabled = true;
    }

    #region Graphs

    /// <summary>
    ///     Turns the control elements of the graph generator on and off.
    /// </summary>
    /// <param name="state">On or off.</param>
    private void SetGraphControls(bool state) {
        comboGraphSheet.Enabled = state;
        btnSelectCategories.Enabled = state;
        btnSelectCombinations.Enabled = state;
        txtGraphExportFilename.Enabled = state;
        btnSaveGraphTemplate.Enabled = state;
        btnLoadGraphTemplate.Enabled = state;
        tabControlGraph.Enabled = state;
    }

    private void btnSelectGraphFile_Click(object sender, EventArgs e) {
        // Clear sheet combo
        comboGraphSheet.Items.Clear();

        // Ask user to pick file 
        using (var ofd = new OpenFileDialog()) {
            ofd.Filter = "Excel workbook|*.xlsx";
            var result = ofd.ShowDialog();
            if (result == DialogResult.OK) {
                // Load file
                _inputFile = ofd.FileName;
                btnSelectGraphFile.Text = ofd.FileName;
                _dataSet = DataLoaderFactory.GetDataSet(_inputFile);

                // Add tables to combobox
                foreach (DataTable? table in _dataSet.Tables) {
                    if (table == null) throw new Exception($"Read null sheet from {_inputFile}");

                    comboGraphSheet.Items.Add(table.TableName);
                }

                // Enable controls
                comboGraphSheet.Enabled = true;

                // Clear fields
                ResetGraphFields();
            }
        }
    }

    /// <summary>
    ///     Called whenever the user changes the selected value in the sheet selection dropdown.
    ///     Loads the new sheet into the DataGridView and sets the selection mode to FullColumns.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void comboGraphSheet_SelectedIndexChanged(object sender, EventArgs e) {
        dataGridViewGraph.SelectionMode = DataGridViewSelectionMode.CellSelect;
        dataGridViewGraph.DataSource = _dataSet.Tables[(string)comboGraphSheet.SelectedItem];
        foreach (DataGridViewColumn col in dataGridViewGraph.Columns) col.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewGraph.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
        SetGraphControls(true);
        ResetGraphFields();
    }

    private void ResetGraphFields() {
        btnSelectCategories.Text = "Select categories";
    }

    /// <summary>
    ///     Is called whenever the user presses the select categories button
    ///     Gets the currently selected columns from the DataGridView and stores their names.
    ///     These are used to determine the filters for the graphs.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSelectCategories_Click(object sender, EventArgs e) {
        // Get string values of columns and add to list
        _template = new GraphTemplate();
        _template.Categories = new List<string>();
        foreach (DataGridViewColumn? col in dataGridViewGraph.SelectedColumns) {
            if (col == null) continue;

            _template.Categories.Add(col.Name);
        }

        btnSelectCategories.Text = string.Join(", ", _template.Categories);
        _dataLoader = new DataLoader(_dataSet.Tables[(string)comboGraphSheet.SelectedItem], _template.Categories);
        _template.SheetName = (string)comboGraphSheet.SelectedItem;
    }

    /// <summary>
    ///     Is called whenever FormAddGraph reports a new combination to be added to the list.
    ///     Adds the combination to the template.
    /// </summary>
    /// <param name="filters1">Filters for the first series in the graph.</param>
    /// <param name="filters2">Filters for the second series in the graph.</param>
    /// <param name="axis">The name of the category that will be the x axis.</param>
    private void AddGraphComboResultCallback(Dictionary<string, string> filters1, Dictionary<string, string> filters2, string axis) {
        if (!string.IsNullOrEmpty(_template.axis) && _template.axis != axis) {
            MessageBox.Show($"Because you already created a graph in this template, '*' can only be chosen for category {axis}");
            return;
        }

        _template.axis = axis;
        _template.Add(filters1, filters2);
        var categories = _dataLoader.GetAllCategoryValues();
        var formAddGraph = new FormAddGraph(categories, AddGraphComboResultCallback);
        formAddGraph.Show();
    }

    /// <summary>
    ///     Called whenever the user presses the button to add a new combination.
    ///     Opens a new FormAddGraph window where the user can fill in their desired combination.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCombinations_Click(object sender, EventArgs e) {
        var categories = _dataLoader.GetAllCategoryValues();
        var formAddGraph = new FormAddGraph(categories, AddGraphComboResultCallback);
        formAddGraph.Show();
    }

    /// <summary>
    ///     Called whenever the user presses the button to load a GraphTemplate.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoadGraphTemplate_Click(object sender, EventArgs e) {
        using (var ofd = new OpenFileDialog()) {
            ofd.Filter = "JSON templates|*.json";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\templates";
            var result = ofd.ShowDialog();
            if (result == DialogResult.OK) {
                var stream = ofd.OpenFile();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                var template = JsonConvert.DeserializeObject<GraphTemplate>(json);
                _template = template;
                comboGraphSheet.SelectedItem = template.SheetName;
                btnSelectCategories.Text = string.Join(", ", template.Categories);
                _dataLoader = new DataLoader(_dataSet.Tables[(string)comboGraphSheet.SelectedItem], _template.Categories);
                FillFields();
                MessageBox.Show($"Template '{ofd.SafeFileName}' loaded");
            }
        }
    }

    /// <summary>
    ///     Called whenever the user presses the button to save the current GraphTemplate.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveGraphTemplate_Click(object sender, EventArgs e) {
        if (!Directory.Exists("templates")) Directory.CreateDirectory("templates");

        File.WriteAllText($"templates/{txtGraphExportFilename.Text}.json", JsonConvert.SerializeObject(_template));
        MessageBox.Show($"Template '{txtGraphExportFilename.Text}' has been successfully exported.");
    }

    /// <summary>
    ///     Checks if a string is a valid hex color.
    ///     eg. #123456.
    /// </summary>
    /// <param name="hex">Hex string.</param>
    /// <returns>True if valid.</returns>
    private bool CheckHexValid(string hex) {
        return Regex.Match(hex, "^#[a-fA-F0-9]{6}$").Success;
    }

    /// <summary>
    ///     Called whenever the color1 textbox changes.
    ///     Checks if the current value is a valid hex color.
    ///     If it is, add it to template.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtGraphLayoutColor1_TextChanged(object sender, EventArgs e) {
        if (CheckHexValid(txtGraphLayoutColor1.Text)) {
            labelGraphLayoutColor1Incorrect.Visible = false;
            _template.GraphLayout.color1 = ColorTranslator.FromHtml(txtGraphLayoutColor1.Text);
        }
        else {
            labelGraphLayoutColor1Incorrect.Visible = true;
        }
    }

    /// <summary>
    ///     Called whenever the color2 textbox changes.
    ///     Checks if the current value is a valid hex color.
    ///     If it is, add it to template.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtGraphLayoutColor2_TextChanged(object sender, EventArgs e) {
        if (CheckHexValid(txtGraphLayoutColor2.Text)) {
            labelGraphLayoutColor2Incorrect.Visible = false;
            _template.GraphLayout.color2 = ColorTranslator.FromHtml(txtGraphLayoutColor2.Text);
        }
        else {
            labelGraphLayoutColor2Incorrect.Visible = true;
        }
    }

    /// <summary>
    ///     Called whenever the graph tabcontrol changes tabs.
    ///     Renders the graph whenever the graph tab is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControlGraph_SelectedIndexChanged(object sender, EventArgs e) {
        if (tabControlGraph.SelectedTab == tabGraph) {
            if (_template == null || _dataLoader == null) {
                MessageBox.Show(
                    "Cannot show graph without selecting categories first.\nHint: hold the control button on your keyboard and select your category columns, then press the 'Select categories' button.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(_template.axis)) {
                MessageBox.Show(
                    "Cannot show graph without any combinations.\nHint: Press the 'Select combinations' button to add new combinations, leave one category on '*' for both samples, this will be the variable on the x axis.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GenerateGraph();
        }
    }

    /// <summary>
    ///     Generates the graph en renders it to the PictureBox on the graph tab.
    /// </summary>
    private void GenerateGraph() {
        _template.GraphLayout.HeaderX = txtGraphHeaderX.Text;
        _template.GraphLayout.HeaderY = txtGraphHeaderY.Text;
        _template.GraphLayout.Legend = checkGraphLayoutLegend.Checked;
        _template.GraphLayout.color1 = ColorTranslator.FromHtml(txtGraphLayoutColor1.Text);
        _template.GraphLayout.color2 = ColorTranslator.FromHtml(txtGraphLayoutColor2.Text);

        var grapher = new ErrorBarGrapher(_dataLoader, _template);
        pictureGraph.Image = grapher.GetBitmap();
        pictureGraph.SizeMode = PictureBoxSizeMode.Zoom;
    }

    /// <summary>
    ///     Fills all form elements with the values of the currently active template.
    /// </summary>
    private void FillFields() {
        txtGraphHeaderX.Text = _template.GraphLayout.HeaderX;
        txtGraphHeaderY.Text = _template.GraphLayout.HeaderY;
        checkGraphLayoutLegend.Checked = _template.GraphLayout.Legend;
        txtGraphLayoutColor1.Text = HexConverter(_template.GraphLayout.color1);
        txtGraphLayoutColor2.Text = HexConverter(_template.GraphLayout.color2);
    }

    /// <summary>
    ///     Converts a color to hex string.
    /// </summary>
    /// <param name="c">The color.</param>
    /// <returns>Hex color string starting with #.</returns>
    private static string HexConverter(Color c) {
        return c.Name.Replace("ff", "#");
    }

    #endregion
}