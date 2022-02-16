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

        // Loop through all target files
        foreach (var file in System.IO.Directory.GetFiles(Directory, Filter)) {
            // Read and interpret data
            AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
            AbstractDataInterpreter interpreter = new DataInterpreterTensileStrain(reader.ReadData());

            // Remove unwanted columns from data
            var dataRowStrain = interpreter.GetData();
            removableColumns.ForEach(idx => dataRowStrain.RemoveAt(idx));

            // Store data
            var rowStrain = (Path.GetFileNameWithoutExtension(file), dataRowStrain);
            dataStrain.Add(rowStrain);
        }

        // Get headers and trim unwanted headers
        var headersTarget = GetHeaders();
        if (_headersActive != null && _headersActive.Count > 0) headersTarget = _headersActive.Where(row => row.Value).Select(row => row.Key).ToList();

        // Write data to file
        var writer = new DataWriter(dataStrain, headersTarget);
        writer.Write(outputFile, "TearStrain", Separate);
    }
}