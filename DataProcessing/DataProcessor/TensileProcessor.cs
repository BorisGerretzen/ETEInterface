using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;

namespace DataProcessing.DataProcessor;

public class TensileProcessor : AbstractProcessor {
    public static TensileProcessor Empty = new("", false);

    public TensileProcessor(string directory, bool separate) : base(directory, separate) {
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
        var dataElongation = new List<(string, List<double>)>();
        var removableColumns = RemoveFromResults();

        // Loop through all target files
        foreach (var file in System.IO.Directory.GetFiles(Directory, Filter)) {
            // Readers and interpreters
            AbstractDataReader reader = new DataReaderTensile(file, "Values Series");
            AbstractDataInterpreter interpreterStrain = new DataInterpreterTensileStrain(reader.ReadData());
            AbstractDataInterpreter interpreterElongation = new DataInterpreterTensileElongation(reader.ReadData());

            // Get data and remove columns we dont want
            var dataRowStrain = interpreterStrain.GetData();
            var dataRowElongation = interpreterElongation.GetData();
            removableColumns.ForEach(idx => dataRowStrain.RemoveAt(idx));
            removableColumns.ForEach(idx => dataRowElongation.RemoveAt(idx));

            // Add it to the lists of rows
            var rowStrain = (Path.GetFileNameWithoutExtension(file), dataRowStrain);
            var rowElongation = (Path.GetFileNameWithoutExtension(file), dataRowElongation);
            dataStrain.Add(rowStrain);
            dataElongation.Add(rowElongation);
        }

        // Get target headers and get rid of the rest
        var headersTarget = GetHeaders();
        if (_headersActive != null && _headersActive.Count > 0) headersTarget = _headersActive.Where(row => row.Value).Select(row => row.Key).ToList();

        // Write to file
        var writer = new DataWriter(dataStrain, headersTarget);
        writer.Write(outputFile, "TensileStrain", Separate);
        writer = new DataWriter(dataElongation, headersTarget);
        writer.Write(outputFile, "TensileElongation", Separate);
    }
}