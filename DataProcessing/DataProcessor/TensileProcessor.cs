using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor;

public class TensileProcessor : AbstractProcessor {
    public static TensileProcessor Empty = new("", false, false);

    public TensileProcessor(string directory, bool separate, bool zs2) : base(directory, separate, zs2) {
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
        var dataStrain = new List<(string, List<double>)>();
        var removableColumns = RemoveFromResults();

        // Loop through all target files
        foreach (var file in System.IO.Directory.GetFiles(Directory, Filter)) {
            // Readers and interpreters
            AbstractDataReader reader;
            if (zs2) {
                reader = new DataReaderTensileZs2(file);
            }
            else {
                reader = new DataReaderTensile(file, "Values Series");
            }

            AbstractDataInterpreter interpreterStress = new DataInterpreterTensileStress(reader.ReadData());
            AbstractDataInterpreter interpreterStrain = new DataInterpreterTensileStrain(reader.ReadData());

            // Get data and remove columns we dont want
            var dataRowStress = interpreterStress.GetData();
            var dataRowStrain = interpreterStrain.GetData();
            removableColumns.ForEach(idx => dataRowStress.RemoveAt(idx));
            removableColumns.ForEach(idx => dataRowStrain.RemoveAt(idx));

            // Add it to the lists of rows
            var rowStress = (Path.GetFileNameWithoutExtension(file), dataRowStress);
            var rowStrain = (Path.GetFileNameWithoutExtension(file), dataRowStrain);
            dataStress.Add(rowStress);
            dataStrain.Add(rowStrain);
        }

        // Get target headers and get rid of the rest
        var headersTarget = GetHeaders();
        if (_headersActive is { Count: > 0 }) headersTarget = _headersActive.Where(row => row.Value).Select(row => row.Key).ToList();

        // Write to file
        var writer = new DataWriter(dataStress, headersTarget);
        writer.Write(outputFile, "TensileStress", Separate);
        writer = new DataWriter(dataStrain, headersTarget);
        writer.Write(outputFile, "TensileStrain", Separate);
    }
}