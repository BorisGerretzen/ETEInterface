using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Grapher
{
    public static class Demo
    {
        public static Bitmap demo() {
            var myModel = new PlotModel { Title = "Example 1" };
            myModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            var pngExporter = new PngExporter { Width = 900, Height = 600 };
            var bitmap = pngExporter.ExportToBitmap(myModel);
            return bitmap;
        }
    }
}