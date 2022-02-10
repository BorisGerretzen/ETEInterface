using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor {
    public abstract class AbstractDataInterpreter {
        protected readonly List<List<(double, double)>> _data;

        protected AbstractDataInterpreter(List<List<(double, double)>> data) {
            _data = data;
        }

        public abstract List<string> GetHeaders();

        public abstract List<double> GetData();
    }
}