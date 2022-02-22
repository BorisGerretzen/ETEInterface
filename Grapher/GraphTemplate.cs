using System.Drawing;
using Grapher.Graph;
using ScottPlot;

namespace Grapher;

public class GraphTemplate {
    public GraphTemplate() {
        Items = new List<GraphTemplateItem>();
        GraphLayout = new GraphLayoutTemplate();
    }

    public GraphLayoutTemplate GraphLayout { get; set; }

    public string SheetName { get; set; }

    public List<string> Categories { get; set; }
    public List<GraphTemplateItem> Items { get; set; }
    public string axis { get; set; }

    public void Add(GraphTemplateItem item) {
        Items.Add(item);
    }

    public void Add(Dictionary<string, string> options1, Dictionary<string, string> options2) {
        var item = new GraphTemplateItem();
        item.options1 = options1;
        item.options2 = options2;
        Add(item);
    }

    public class GraphLayoutTemplate {
        public Color color1 { get; set; }
        public Color color2 { get; set; }

        public string HeaderX { get; set; }
        public string HeaderY { get; set; }

        public bool Legend { get; set; }
    }

    public class GraphTemplateItem {
        public Dictionary<string, string> options1 { get; set; }
        public Dictionary<string, string> options2 { get; set; }
    }
}