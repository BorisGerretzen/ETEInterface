using System.Drawing;
using Grapher.Data;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Grapher {
    public static class Demo {
        public static Bitmap demo(DataLoader loader, GraphTemplate template) {
            var myModel = new PlotModel { Title = "Example 1" };
            var series = new ScatterErrorSeries();
            var item = template.Items[0];
            int indexWildcard = item.options1.IndexOf("*");
            if (indexWildcard != -1) {
                throw new ArgumentException("Template item does not contain a * category.");
            }

            string nameWildcard = template.Categories[indexWildcard];
            DataSelector selector = new DataSelector(loader);
            var selectedData = selector.SelectData(item);
            var options = loader.GetCategoryOptions()[nameWildcard];

            foreach (var elem in selectedData) {

            }

            myModel.Series.Add(new ScatterErrorSeries());
            myModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            var pngExporter = new PngExporter { Width = 900, Height = 600 };
            var bitmap = pngExporter.ExportToBitmap(myModel);
            return bitmap;
        }
    }
}