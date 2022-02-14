using MoreLinq;

namespace DataProcessing.DataInterpreter; 

internal class DataInterpreterTensileElongation : AbstractDataInterpreter {
    private List<(double, double)>? _peaks;

    public DataInterpreterTensileElongation(List<List<(double, double)>> data) : base(data) { }

    public override List<string> GetHeaders() {
        return new List<string> {
            "min",
            "mean",
            "max",
            "error under",
            "error over"
        };
    }

    public override List<double> GetData() {
        return new List<double> {
            GetMinElongation(),
            GetMeanElongation(),
            GetMaxElongation(),
            GetMeanElongation() - GetMinElongation(),
            GetMaxElongation() - GetMeanElongation()
        };
    }

    private List<(double, double)>? GetPeaks() {
        if (_peaks == null) _peaks = _data.Select(sample => sample.MaxBy(x => x.Item2).First()).ToList();

        return _peaks;
    }

    private double GetMeanElongation() {
        return GetPeaks().Average(peak => peak.Item1);
    }

    private double GetMinElongation() {
        return GetPeaks().Min(peak => peak.Item1);
    }

    private double GetMaxElongation() {
        return GetPeaks().Max(peak => peak.Item1);
    }
}