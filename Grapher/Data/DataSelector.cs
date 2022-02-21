using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Grapher.Data
{
    public class DataSelector {
        private DataLoader _loader;

        public DataSelector(DataLoader loader) {
            _loader = loader;
        }

        /// <summary>
        /// Selects the data for a single template item from the dataloader
        /// </summary>
        /// <param name="templateItem">The target templateitem</param>
        /// <returns></returns>
        public List<List<double>> SelectData(GraphTemplate.GraphTemplateItem templateItem) {
            var selected = new List<List<double>>();
            foreach(var kvp in _loader.GetData()) {
                bool match = true;
                for (int i = 0; i < templateItem.options1.Count; i++) {
                    if (kvp.Key[i] != templateItem.options1[i] && kvp.Key[i] != "*") {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    selected.Add(kvp.Value);
                }
            }

            return selected;
        }
    }
}
