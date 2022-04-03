using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor;

public class TearProcessor : AbstractProcessor {
    public static TearProcessor Empty = new("", false, false);

    public TearProcessor(string directory, bool separate, bool zs2) : base(directory, separate, false) {
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
        var dataStress = new List<(string, List<double>)>();
        var removableColumns = RemoveFromResults();

        // Loop through all target files
        foreach (var file in System.IO.Directory.GetFiles(Directory, Filter)) {
            // Read and interpret data
            AbstractDataReader reader;
            if (zs2) {
                reader = new DataReaderTensileZs2(file);
            }
            else {
                reader = new DataReaderTensile(file, "Values Series");
            }

            AbstractDataInterpreter interpreter = new DataInterpreterTensileStress(reader.ReadData());

            // Remove unwanted columns from data
            var dataRowStress = interpreter.GetData();
            removableColumns.ForEach(idx => dataRowStress.RemoveAt(idx));

            // Store data
            var rowStress = (Path.GetFileNameWithoutExtension(file), dataRowStress);
            dataStress.Add(rowStress);
        }

        // Get headers and trim unwanted headers
        var headersTarget = GetHeaders();
        if (_headersActive is { Count: > 0 }) headersTarget = _headersActive.Where(row => row.Value).Select(row => row.Key).ToList();

        // Write data to file
        var writer = new DataWriter(dataStress, headersTarget);
        writer.Write(outputFile, "TearStress", Separate);
    }
}