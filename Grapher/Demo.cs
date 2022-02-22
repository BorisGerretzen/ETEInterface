using System.Drawing;
using ScottPlot;

namespace Grapher;

public static class Demo {
    private static void AddSeries(ref Plot plot, Dictionary<string, string> filters, DataLoader loader, Color color, string axis) {
        var results = loader.GetWithCategories(filters);
        var allOptions = loader.GetAllCategoryValues();
        var xAxisList = allOptions[axis].ToList();
        xAxisList.Sort();

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

        plot.AddScatter(xs, ys, color, lineStyle: LineStyle.None);
        plot.AddErrorBars(xs, ys, new double[results.Count], new double[results.Count], yErrPos, yErrNeg, color);
    }

    public static Bitmap demo(DataLoader loader, GraphTemplate template) {
        var allOptions = loader.GetAllCategoryValues();
        var xAxisList = allOptions[template.axis].ToList();

        var plt = new Plot(900);
        AddSeries(ref plt, template.Items[0].options1, loader, template.GraphLayout.color1, template.axis);
        AddSeries(ref plt, template.Items[0].options2, loader, template.GraphLayout.color2, template.axis);
        plt.XTicks(xAxisList.ToArray());
        plt.XAxis.Label(template.GraphLayout.HeaderX);
        plt.YAxis.Label(template.GraphLayout.HeaderY);
        plt.XAxis.Grid(false);
        plt.Legend(template.GraphLayout.Legend);
        var bitmap = plt.Render();
        return bitmap;
    }
}