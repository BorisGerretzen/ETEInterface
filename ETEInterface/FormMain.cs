using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataProcessor;
using DataProcessor.DataProcessor;

namespace ETEInterface {
    public partial class FormMain : Form {
        private string inputDirectory;
        private string outputFile;

        public FormMain() {
            InitializeComponent();
        }



        private void btnExport_Click(object sender, EventArgs e) {
            if (tabControl1.SelectedTab == tabTensile) {
                DataPrepper.PrepTensile(inputDirectory, checkRecursive.Checked);
                AbstractProcessor processor = new TensileProcessor("temp", radioSeparate.Checked);
                processor.Process(outputFile);
                Directory.Delete("temp", true);
            } else if (tabControl1.SelectedTab == tabTear) {
                DataPrepper.PrepTensile(inputDirectory, checkRecursive.Checked);
                AbstractProcessor processor = new TearProcessor("temp", radioSeparate.Checked);
                processor.Process(outputFile);
                Directory.Delete("temp", true);
            }
        }

        private void btnSelectInput_Click(object sender, EventArgs e) {
            using (var fbd = new FolderBrowserDialog()) {
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
                    sfd.Filter = "Excel file|*.xlsx";
                    DialogResult result = sfd.ShowDialog();
                    if (result == DialogResult.OK) {
                        outputFile = sfd.FileName;
                        btnSelectOutput.Text = sfd.FileName;
                    }
                }
            }
            else if (radioSheet.Checked) {
                using (var ofd = new OpenFileDialog()) {
                    ofd.Filter = "Excel file|*.xlsx";
                    DialogResult result = ofd.ShowDialog();
                    if (result == DialogResult.OK) {
                        outputFile = ofd.FileName;
                        btnSelectOutput.Text = ofd.FileName;
                    }
                }
            }
        }
    }
}