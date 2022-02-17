using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Grapher
{
    public class GraphTemplate {
        public string SheetName { get; set; }
        public List<string> Categories { get; set; }
        public List<GraphTemplateItem> Items { get; set; }

        public GraphTemplate() {
            Items = new();
        }

        public void Add(GraphTemplateItem item) {
            Items.Add(item);
        }

        public void Add(List<string> options1, List<string> options2) {
            var item = new GraphTemplateItem();
            item.options1 = options1;
            item.options2 = options2;
            Add(item);
        }

        public class GraphTemplateItem {
            public List<string> options1 { get; set; }
            public List<string> options2 { get; set; }
        }
    }
}
