﻿using System;
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
using DataProcessing.DataProcessor;

namespace Interface {
    public partial class FormMain : Form {
        private string _inputDirectory = null!;
        private string _outputFile = null!;
        private Thread _worker = null!;
        
        public FormMain() {
            InitializeComponent();
        }

        private void ProgressUpdate(double amount) {
            progressBar.Invoke((MethodInvoker)delegate {
                progressBar.Value = (int)Math.Floor(amount * 100);
            });
        }

        private void btnExport_Click(object sender, EventArgs e) {
            if (_worker != null && _worker.IsAlive) {
                DialogResult dialogResult = MessageBox.Show("An export is already running, do you want to cancel it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Yes) {
                    _worker.Abort();
                    Directory.Delete("temp", true);
                }
                else {
                    return;
                }
            }
            if (tabControl1.SelectedTab == tabTensile) {
                _worker = new Thread(() => {
                    DataPrepper.PrepTensile(_inputDirectory, checkRecursive.Checked, ProgressUpdate);
                    AbstractProcessor processor = new TensileProcessor("temp", radioSeparate.Checked);
                    processor.Process(_outputFile);
                    Directory.Delete("temp", true);
                    ProgressUpdate(1);
                    MessageBox.Show("Export complete");
                });
                _worker.Start();
            }
            else if (tabControl1.SelectedTab == tabTear) {
                _worker = new Thread(() => {
                    DataPrepper.PrepTensile(_inputDirectory, checkRecursive.Checked, ProgressUpdate);
                    AbstractProcessor processor = new TearProcessor("temp", radioSeparate.Checked);
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
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                    _inputDirectory = fbd.SelectedPath;
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
                        _outputFile = sfd.FileName;
                        btnSelectOutput.Text = sfd.FileName;
                    }
                }
            }
            else if (radioSheet.Checked) {
                using (var ofd = new OpenFileDialog()) {
                    ofd.Filter = "Excel workbook|*.xlsx";
                    DialogResult result = ofd.ShowDialog();
                    if (result == DialogResult.OK) {
                        _outputFile = ofd.FileName;
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