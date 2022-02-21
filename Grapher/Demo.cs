using System.Drawing;
using ScottPlot;

namespace Grapher; 

public static class Demo {
    public static Bitmap demo(DataLoader loader, GraphTemplate template) {
        var categoryFilters = template.Items[0].options1;
        var results = loader.GetWithCategories(categoryFilters);
        var axis = categoryFilters.Where(kvp => kvp.Value == "*").Select(kvp => kvp.Key).First();
        var allOptions = loader.GetAllCategoryValues();
        var xAxisList = allOptions[axis].ToList();
        xAxisList.Sort();

        var plt = new Plot(900);
        var xs = new double[results.Count];
        var ys = new double[results.Count];
        var yErrPos = new double[results.Count];
        var yErrNeg = new double[results.Count];

        var idx = 0;
        foreach (var row in results) {
            xs[idx] = xAxisList.IndexOf((string)row[axis]);
            ys[idx] = (double)row["mean"];
            yErrPos[idx] = (double)row["error top"];
            yErrNeg[idx] = (double)row["error bottom"];
            idx++;
        }

        plt.AddScatter(xs, ys, Color.Blue, lineStyle: LineStyle.None);
        plt.AddErrorBars(xs, ys, new double[results.Count], new double[results.Count], yErrPos, yErrNeg, Color.Blue);
        plt.XTicks(xAxisList.ToArray());
        var bitmap = plt.Render();
        return bitmap;
    }
}