using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    public abstract class AbstractProcessor {
        protected string _filter;
        protected string _directory;
        protected bool _separate;

        protected AbstractProcessor(string directory, bool separate) {
            _directory = directory;
            _separate = separate;
        }

        public abstract void Process(string outputFile);
    }
}
