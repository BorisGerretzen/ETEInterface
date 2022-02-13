using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataProcessor;
using DataProcessor.DataProcessor;

namespace ETEInterface {
    public partial class FormMain : Form {
        private string inputDirectory;
        private string outputFile;
        private Thread worker;

        public FormMain() {
            InitializeComponent();
        }

        private void ProgressUpdate(double amount) {
            progressBar.Invoke((MethodInvoker)delegate {
                progressBar.Value = (int)Math.Floor(amount * 100);
            });
        }

        private void btnExport_Click(object sender, EventArgs e) {
            if (worker != null && worker.IsAlive) {
                DialogResult dialogResult = MessageBox.Show("An export is already running, do you want to cancel it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Yes) {
                    worker.Abort();
                    Directory.Delete("temp", true);
                }
                else {
                    return;
                }
            }
            if (tabControl1.SelectedTab == tabTensile) {
                worker = new Thread(() => {
                    DataPrepper.PrepTensile(inputDirectory, checkRecursive.Checked, ProgressUpdate);
                    AbstractProcessor processor = new TensileProcessor("temp", radioSeparate.Checked);
                    processor.Process(outputFile);
                    Directory.Delete("temp", true);
                    ProgressUpdate(1);
                    MessageBox.Show("Export complete");
                });
            }
            else if (tabControl1.SelectedTab == tabTear) {
                worker = new Thread(() => {
                    DataPrepper.PrepTensile(inputDirectory, checkRecursive.Checked, ProgressUpdate);
                    AbstractProcessor processor = new TearProcessor("temp", radioSeparate.Checked);
                    processor.Process(outputFile);
                    Directory.Delete("temp", true);
                    ProgressUpdate(1);
                    MessageBox.Show("Export complete");
                });
            }
            worker.Start();
        }

        private void btnSelectInput_Click(object sender, EventArgs e) {
            using (var fbd = new FolderBrowserDialog()) {
                fbd.SelectedPath = Directory.GetCurrentDirectory();
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                    inputDirectory = fbd.SelectedPath;
                    btnSelectInput.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnSelectOutput_Click(object sender, EventArgs e) {
            if (radioSeparate.Checked) {
                using (var sfd = new SaveFileDialog()) {
                    sfd.Filter = "Excel workbook|*.xlsx";
                    DialogResult result = sfd.ShowDialog();
                    if (result == DialogResult.OK) {
                        outputFile = sfd.FileName;
                        btnSelectOutput.Text = sfd.FileName;
                    }
                }
            }
            else if (radioSheet.Checked) {
                using (var ofd = new OpenFileDialog()) {
                    ofd.Filter = "Excel workbook|*.xlsx";
                    DialogResult result = ofd.ShowDialog();
                    if (result == DialogResult.OK) {
                        outputFile = ofd.FileName;
                        btnSelectOutput.Text = ofd.FileName;
                    }
                }
            }
        }

        private void linkEmail_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(linkEmail.Text);
        }
    }
}