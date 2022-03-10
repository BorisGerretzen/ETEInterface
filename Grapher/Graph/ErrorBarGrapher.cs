using System.Drawing;
using ScottPlot;

namespace Grapher.Graph;

public class ErrorBarGrapher {
    private readonly DataLoader _loader;
    private readonly GraphTemplate _template;
    private Plot _plot;

    /// <summary>
    ///     X axis labels.
    /// </summary>
    private List<string> _xAxis;

    public ErrorBarGrapher(DataLoader loader, GraphTemplate template) {
        _loader = loader;
        _template = template;
        GeneratePlot(0);
    }

    /// <summary>
    ///     Generates all graphs in the supplied GraphTemplate and exports them to the specified output directory.
    /// </summary>
    /// <param name="outputDirectory">The directory where all the exported images will be placed.</param>
    public void GenerateAll(string outputDirectory) {
        foreach (var i in Enumerable.Range(0, _template.Items.Count)) {
            GeneratePlot(i);
            var ouputPath = Path.Join(outputDirectory, $"{i}.png");
            _plot.SaveFig(ouputPath);
        }
    }

    /// <summary>
    ///     Gets a bitmap from the first combination in the template.
    /// </summary>
    /// <returns></returns>
    public Bitmap GetFirst() {
        GeneratePlot(0);
        return GetBitmap();
    }

    /// <summary>
    ///     Generates the ScottPlot Plot using the GraphTemplate provided.
    /// </summary>
    private void GeneratePlot(int idx) {
        // Gets all options for the axis specified in the template and sorts them
        var allOptions = _loader.GetAllCategoryValues();
        _xAxis = allOptions[_template.axis].ToList();
        _xAxis.Sort();

        // Create plot and add series
        _plot = new Plot();
        AddSeries(_template.Items[idx].options1, _template.GraphLayout.color1);
        AddSeries(_template.Items[idx].options2, _template.GraphLayout.color2);

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
    private Bitmap GetBitmap() {
        return _plot.Render();
    }
}