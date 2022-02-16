using System.Diagnostics;
using DataProcessing.DataProcessor;
using DataProcessing.WinForms;

namespace Interface;

public partial class FormMain : Form {
    private readonly OptionsPanelFactory.OptionsPanel optionsPanelTear;
    private readonly OptionsPanelFactory.OptionsPanel optionsPanelTensile;
    private string _inputDirectory = null!;
    private string _outputFile = null!;
    private Thread _worker = null!;

    public FormMain() {
        InitializeComponent();
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
            var dialogResult = MessageBox.Show("An export is already running, do you want to cancel it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult != DialogResult.Yes) return;

            _worker.Abort();
            Directory.Delete("temp", true);
        }

        // Check if input and output paths are set
        if (string.IsNullOrEmpty(_inputDirectory) || string.IsNullOrEmpty(_outputFile)) {
            MessageBox.Show("Please specify an input directory and output file using the labeled buttons,", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Tensile
        if (tabControl1.SelectedTab == tabTensile) {
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
        else if (tabControl1.SelectedTab == tabTear) {
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
}