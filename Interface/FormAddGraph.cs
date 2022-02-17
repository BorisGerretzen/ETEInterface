using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using MoreLinq.Extensions;

namespace Interface {
    public partial class FormAddGraph : Form {
        public delegate void ResultCallback(List<string> options1, List<string> options2);

        private Dictionary<string, ComboBox> combos;
        private List<string> categories;
        private ResultCallback _callback;

        public FormAddGraph(Dictionary<string, HashSet<string>> categoryOptions, ResultCallback callback) {
            InitializeComponent();
            _callback = callback;
            combos = new();
            categories = categoryOptions.Keys.ToList();

            // Place controls for first item
            int i = 0;
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
        /// Get all results from the combos and send it to the callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDone_Click(object sender, EventArgs e) {
            List<string> options1 = new();
            List<string> options2 = new();

            foreach (var category in categories) {
                string option1 = (string)combos[$"{category}1"].SelectedItem;
                string option2 = (string)combos[$"{category}2"].SelectedItem;
                options1.Add(option1);
                options2.Add(option2);
            }

            _callback(options1, options2);
            this.Close();
        }
    }
}