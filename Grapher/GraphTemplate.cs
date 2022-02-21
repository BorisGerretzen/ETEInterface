namespace Grapher; 

public class GraphTemplate {
    public GraphTemplate() {
        Items = new List<GraphTemplateItem>();
    }

    public string SheetName { get; set; }
    public List<string> Categories { get; set; }
    public List<GraphTemplateItem> Items { get; set; }

    public void Add(GraphTemplateItem item) {
        Items.Add(item);
    }

    public void Add(Dictionary<string, string> options1, Dictionary<string, string> options2) {
        var item = new GraphTemplateItem();
        item.options1 = options1;
        item.options2 = options2;
        Add(item);
    }

    public class GraphTemplateItem {
        public Dictionary<string, string> options1 { get; set; }
        public Dictionary<string, string> options2 { get; set; }
    }
}