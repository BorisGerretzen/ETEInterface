using System.Drawing;
using ScottPlot;

namespace Grapher.Graph;

public class ErrorBarGrapher {
    private readonly DataLoader _loader;
    private Plot _plot;
    private readonly GraphTemplate _template;

    /// <summary>
    ///     X axis labels.
    /// </summary>
    private List<string> _xAxis;

    public ErrorBarGrapher(DataLoader loader, GraphTemplate template) {
        _loader = loader;
        _template = template;
        GeneratePlot();
    }

    /// <summary>
    ///     Generates the ScottPlot Plot using the GraphTemplate provided.
    /// </summary>
    private void GeneratePlot() {
        // Gets all options for the axis specified in the template and sorts them
        var allOptions = _loader.GetAllCategoryValues();
        _xAxis = allOptions[_template.axis].ToList();
        _xAxis.Sort();

        // Create plot and add series
        _plot = new Plot();
        AddSeries(_template.Items[0].options1, _template.GraphLayout.color1);
        AddSeries(_template.Items[0].options2, _template.GraphLayout.color2);

        // Add ticks, headers, grid, etc.
        _plot.XTicks(_xAxis.ToArray());
        _plot.XAxis.Label(_template.GraphLayout.HeaderX);
        _plot.YAxis.Label(_template.GraphLayout.HeaderY);
        _plot.XAxis.Grid(false);
        _plot.Legend(_template.GraphLayout.Legend);
    }

    /// <summary>
    ///     Adds a series to the plot
    /// </summary>
    /// <param name="filters">Filters of this series.</param>
    /// <param name="color">Color of this series.</param>
    private void AddSeries(Dictionary<string, string> filters, Color color) {
        // Get all rows that match the filter
        var results = _loader.GetWithCategories(filters);

        // Init result arrays
        var xs = new double[results.Count];
        var ys = new double[results.Count];
        var yErrPos = new double[results.Count];
        var yErrNeg = new double[results.Count];

        // Store all the rows in the arrays
        var idx = 0;
        foreach (var row in results) {
            xs[idx] = _xAxis.IndexOf((string)row[_template.axis]);
            ys[idx] = (double)row["mean"];
            yErrPos[idx] = (double)row["error top"];
            yErrNeg[idx] = (double)row["error bottom"];
            idx++;
        }

        // Add the series to the plot.
        _plot.AddScatter(xs, ys, color, lineStyle: LineStyle.None);
        _plot.AddErrorBars(xs, ys, new double[results.Count], new double[results.Count], yErrPos, yErrNeg, color);
    }

    /// <summary>
    ///     Exports the current plot as bitmap.
    /// </summary>
    /// <returns>Bitmap export of this plot.</returns>
    public Bitmap GetBitmap() {
        return _plot.Render();
    }
}