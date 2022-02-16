using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor;

public class TearProcessor : AbstractProcessor {
    public static TearProcessor Empty = new("", false);

    public TearProcessor(string directory, bool separate) : base(directory, separate) {
        Filter = "*.xlsx";
        SetHeaders(new List<string> {
            "min",
            "mean",
            "max",
            "error bottom",
            "error top"
        });
    }

    public override void Process(string outputFile) {
        var dataStrain = new List<(string, List<double>)>();
        var removableColumns = RemoveFromResults();

        foreach (var file in System.IO.Directory.GetFiles(Directory, Filter)) {
            AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
            AbstractDataInterpreter interpreter = new DataInterpreterTensileStrain(reader.ReadData());
            var dataRowStrain = interpreter.GetData();
            removableColumns.ForEach(idx => dataRowStrain.RemoveAt(idx));
            var rowStrain = (Path.GetFileNameWithoutExtension(file), dataRowStrain);
            dataStrain.Add(rowStrain);
        }

        var headersTarget = GetHeaders();
        if (_headersActive != null && _headersActive.Count > 0) headersTarget = _headersActive.Where(row => row.Value).Select(row => row.Key).ToList();

        var writer = new DataWriter(dataStrain, headersTarget);
        writer.Write(outputFile, "TearStrain", Separate);
    }
}