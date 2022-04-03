using MoreLinq;

namespace DataProcessing.DataInterpreter;

internal class DataInterpreterTensileStress : AbstractDataInterpreter {
    private List<(double, double)>? _peaks;

    public DataInterpreterTensileStress(List<List<(double, double)>> data) : base(data) { }

    public override List<double> GetData() {
        return new List<double> {
            GetMinPeak(),
            GetMeanPeak(),
            GetMaxPeak(),
            GetMeanPeak() - GetMinPeak(),
            GetMaxPeak() - GetMeanPeak()
        };
    }

    private List<(double, double)>? GetPeaks() {
        if (_peaks == null) _peaks = _data.Select(sample => sample.MaxBy(x => x.Item2).First()).ToList();
        return _peaks;
    }

    private double GetMeanPeak() {
        return GetPeaks().Average(peak => peak.Item2);
    }

    private double GetMinPeak() {
        return GetPeaks().Min(peak => peak.Item2);
    }

    private double GetMaxPeak() {
        return GetPeaks().Max(peak => peak.Item2);
    }
}