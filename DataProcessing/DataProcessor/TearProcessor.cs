using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor
{
    public class TearProcessor : AbstractProcessor
    {
        public TearProcessor(string directory, bool separate) : base(directory, separate) {
            Filter = "*.xlsx";
        }
        public override void Process(string outputFile) {
            List<(string, List<double>)> dataStrain = new List<(string, List<double>)>();
            List<string> headers = new List<string>();

            foreach (string file in System.IO.Directory.GetFiles(Directory, Filter))
            {
                AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
                AbstractDataInterpreter interpreterStrain = new DataInterpreterTensileStrain(reader.ReadData());
                var rowStrain = (Path.GetFileNameWithoutExtension(file), interpreterStrain.GetData());
                dataStrain.Add(rowStrain);
                headers = interpreterStrain.GetHeaders();
            }

            DataWriter writer = new DataWriter(dataStrain, headers);
            writer.Write(outputFile, "TearStrain", Separate);
        }
    }
}
