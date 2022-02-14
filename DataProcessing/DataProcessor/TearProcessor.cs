using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor; 

public class TearProcessor : AbstractProcessor {
    public TearProcessor(string directory, bool separate) : base(directory, separate) {
        Filter = "*.xlsx";
    }

    public override void Process(string outputFile) {
        var dataStrain = new List<(string, List<double>)>();
        var headers = new List<string>();

        foreach (var file in System.IO.Directory.GetFiles(Directory, Filter)) {
            AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
            AbstractDataInterpreter interpreterStrain = new DataInterpreterTensileStrain(reader.ReadData());
            var rowStrain = (Path.GetFileNameWithoutExtension(file), interpreterStrain.GetData());
            dataStrain.Add(rowStrain);
            headers = interpreterStrain.GetHeaders();
        }

        var writer = new DataWriter(dataStrain, headers);
        writer.Write(outputFile, "TearStrain", Separate);
    }
}