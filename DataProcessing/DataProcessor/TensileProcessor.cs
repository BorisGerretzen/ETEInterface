using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor {
    public class TensileProcessor : AbstractProcessor {
        public TensileProcessor(string directory, bool separate) : base(directory, separate) {
            Filter = "*.xlsx";
        }

        public override void Process(string outputFile) {
            List<(string, List<double>)> dataStrain = new List<(string, List<double>)>();
            List<(string, List<double>)> dataElongation = new List<(string, List<double>)>();
            List<string> headers = new List<string>();

            foreach (string file in System.IO.Directory.GetFiles(Directory,Filter)) {
                AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
                AbstractDataInterpreter interpreterStrain = new DataInterpreterTensileStrain(reader.ReadData());
                AbstractDataInterpreter interpreterElongation = new DataInterpreterTensileElongation(reader.ReadData());
                var rowStrain = (Path.GetFileNameWithoutExtension(file), interpreterStrain.GetData());
                var rowElongation = (Path.GetFileNameWithoutExtension(file), interpreterElongation.GetData());
                dataStrain.Add(rowStrain);
                dataElongation.Add(rowElongation);
                headers = interpreterStrain.GetHeaders();
            }

            DataWriter writer = new DataWriter(dataStrain, headers);
            writer.Write(outputFile, "TensileStrain", Separate);
            writer = new DataWriter(dataElongation, headers);
            writer.Write(outputFile, "TensileElongation", Separate);
        }
    }
}