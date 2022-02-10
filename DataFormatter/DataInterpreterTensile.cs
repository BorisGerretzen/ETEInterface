using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using MoreLinq.Extensions;

namespace DataFormatter {
    internal class DataInterpreterTensile {
        private readonly List<List<(double, double)>> _data;
        private List<(double, double)> _peaks;

        public DataInterpreterTensile(List<List<(double, double)>> data) {
            _data = data;
        }

        public List<(double, double)> GetPeaks() {
            if (_peaks == null) {
                _peaks = _data.Select(sample => sample.MaxBy(x => x.Item2)).ToList();
            }
            return _peaks;
        }

        public double GetMeanPeak() {
            return GetPeaks().Average(peak => peak.Item2);
        }

        public double GetMinPeak() {
            return GetPeaks().Min(peak => peak.Item2);
        }

        public double GetMaxPeak() {
            return GetPeaks().Max(peak => peak.Item2);
        }

        public double GetMeanElongation() {
            return GetPeaks().Average(peak => peak.Item1);
        }

        public double GetMinElongation() {
            return GetPeaks().Min(peak => peak.Item1);
        }

        public double GetMaxElongation() {
            return GetPeaks().Max(peak => peak.Item1);
        }
    }
}