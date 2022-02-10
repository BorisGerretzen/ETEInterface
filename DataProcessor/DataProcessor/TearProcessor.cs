using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.DataProcessor
{
    public class TearProcessor : AbstractProcessor
    {
        public TearProcessor(string directory, bool separate) : base(directory, separate) {
            _filter = "*.xlsx";
        }
        public override void Process(string outputFile) {
            List<(string, List<double>)> dataStrain = new List<(string, List<double>)>();
            List<string> headers = new List<string>();

            foreach (string file in Directory.GetFiles(_directory, _filter))
            {
                AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
                AbstractDataInterpreter interpreterStrain = new DataInterpreterTensileStrain(reader.ReadData());
                var rowStrain = (Path.GetFileNameWithoutExtension(file), interpreterStrain.GetData());
                dataStrain.Add(rowStrain);
                headers = interpreterStrain.GetHeaders();
            }

            DataWriter writer = new DataWriter(dataStrain, headers);
            writer.Write(outputFile, "TensileStrain", _separate);
        }
    }
}
