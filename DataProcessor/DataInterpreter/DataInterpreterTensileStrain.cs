using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using OfficeOpenXml;

namespace DataProcessor {
    public class DataInterpreterTensileStrain : AbstractDataInterpreter{
        private List<(double, double)> _peaks;

        public DataInterpreterTensileStrain(List<List<(double, double)>> data) : base(data) { }

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
                GetMinPeak(),
                GetMeanPeak(),
                GetMaxPeak(),
                GetMeanPeak() - GetMinPeak(),
                GetMaxPeak() - GetMeanPeak()
            };
        }

        private List<(double, double)> GetPeaks() {
            if (_peaks == null) {
                _peaks = _data.Select(sample => sample.MaxBy(x => x.Item2).First()).ToList();
            }
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
}