using System.Data;
using System.Diagnostics;
using DataProcessing.DataProcessor;
using DataProcessing.WinForms;
using Grapher;
using Grapher.Data;
using OfficeOpenXml.ConditionalFormatting;

namespace Interface;

public partial class FormMain : Form {
    // Normal processing
    private readonly OptionsPanelFactory.OptionsPanel optionsPanelTear;
    private readonly OptionsPanelFactory.OptionsPanel optionsPanelTensile;
    private string _inputDirectory = null!;
    private string _outputFile = null!;
    private Thread _worker = null!;

    // Graphs
    private string _inputFile;
    private List<DataLoader> _loaders = new();
    private List<string> categoryHeaderNames = new();
    private DataSet _dataSet;
    private DataLoader _dataLoader;
    private List<(List<string>, List<string>)> _graphCombinations = new();

    public FormMain() {
        InitializeComponent();
        var bmp = Demo.demo();
        pictureGraph.Image = bmp;
        pictureGraph.SizeMode = PictureBoxSizeMode.Zoom;

        SetGraphControls(false);

        optionsPanelTensile = OptionsPanelFactory.GetOptionsPanel(TensileProcessor.Empty.GetHeaders(), "rebound");
        optionsPanelTear = OptionsPanelFactory.GetOptionsPanel(TearProcessor.Empty.GetHeaders(), "rebound");
        // var optionsPanelRebound = OptionsPanelFactory.GetOptionsPanel(TensileProcessor.Empty.GetHeaders(), "rebound");

        tableTensile.GetControlFromPosition(0, 0).Controls.Add(optionsPanelTensile.GetPanel());
        tableTear.GetControlFromPosition(0, 0).Controls.Add(optionsPanelTear.GetPanel());
        // tableRebound.GetControlFromPosition(0,0).Controls.Add(optionsPanelRebound.GetPanel());
    }

    /// <summary>
    /// Sets the progress bar to a set point
    /// </summary>
    /// <param name="amount">Point to set the progress bar to (0-1)</param>
    private void ProgressUpdate(double amount) {
        progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (int)Math.Floor(amount * 100); });
    }


    private void btnExport_Click(object sender, EventArgs e) {
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

        // Tensile
        if (tabControlMain.SelectedTab == tabTensile) {
            _worker = new Thread(() => {
                DataPrepper.PrepTensile(_inputDirectory, checkRecursive.Checked, ProgressUpdate);
                AbstractProcessor processor = new TensileProcessor("temp", radioSeparate.Checked);
                processor.SetHeadersActive(optionsPanelTensile.GetCheckBoxes());
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
                processor.SetHeadersActive(optionsPanelTear.GetCheckBoxes());
                processor.Process(_outputFile);
                Directory.Delete("temp", true);
                ProgressUpdate(1);
                MessageBox.Show("Export complete");
            });
            _worker.Start();
        }
    }


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

    private void linkEmail_Click(object sender, EventArgs e) {
        Clipboard.SetText(linkEmail.Text);
    }

    private void radioSeparate_CheckedChanged(object sender, EventArgs e) {
        btnSelectOutput.Text = "Select output file";
        _outputFile = string.Empty;
    }

    private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
        Process.Start("explorer", linkGithub.Text);
    }

    private void groupBox1_Enter(object sender, EventArgs e) { }

    #region Graphs

    /// <summary>
    /// Turns the control elements of the graph generator on and off.
    /// </summary>
    /// <param name="state">On or off.</param>
    private void SetGraphControls(bool state) {
        comboGraphSheet.Enabled = state;
        btnSelectCategories.Enabled = state;
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
                    if (table == null) {
                        throw new Exception($"Read null sheet from {_inputFile}");
                    }

                    comboGraphSheet.Items.Add(table.TableName);
                }

                // Enable controls
                SetGraphControls(true);
            }
        }
    }

    private void comboGraphSheet_SelectedIndexChanged(object sender, EventArgs e) {
        dataGridViewGraph.DataSource = _dataSet.Tables[(string)comboGraphSheet.SelectedItem];
    }


    private void btnSelectCategories_Click(object sender, EventArgs e) {
        // Get selected cells
        var selected = dataGridViewGraph.SelectedCells;

        // Get string values of cells and add to list
        categoryHeaderNames = new List<string>();
        foreach (DataGridViewCell? cell in selected) {
            if (cell == null) {
                continue;
            }

            categoryHeaderNames.Add((string)cell.Value);
        }

        // Create data loader
        _dataLoader = new DataLoader(_dataSet.Tables[(string)comboGraphSheet.SelectedItem], categoryHeaderNames);
    }

    private void AddGraphComboResultCallback(List<string> options1, List<string> options2) {
        _graphCombinations.Add((options1, options2));
        var categories = _dataLoader.GetCategoryOptions();
        var formAddGraph = new FormAddGraph(categories, AddGraphComboResultCallback);
        formAddGraph.Show();
    }

    private void btnCombinations_Click(object sender, EventArgs e) {
        var categories = _dataLoader.GetCategoryOptions();
        var formAddGraph = new FormAddGraph(categories, AddGraphComboResultCallback);
        formAddGraph.Show();
    }

    #endregion
}