using DataProcessing.DataInterpreter;
using DataProcessing.DataReader;
namespace DataProcessing.DataProcessor; 

public abstract class AbstractProcessor {
    protected string Filter = null!;
    protected readonly string Directory;
    protected readonly bool Separate;
    protected Dictionary<string, bool> _headers;
    
    protected AbstractProcessor(string directory, bool separate) {
        Directory = directory;
        Separate = separate;
    }

    public abstract void Process(string outputFile);

    public abstract List<string> GetHeaders();

    public void SetHeaders(Dictionary<string, bool> headers) {
        _headers = headers;
    }

    protected List<int> RemoveFromResults() {
        var returnVal = new List<int>();

        var headers = GetHeaders();
        if (_headers == null || _headers.Count == 0) {
            return returnVal;
        }

        foreach ((var key, var val) in _headers) {
            if (!val) {
                returnVal.Add(headers.IndexOf(key));
            }
        }

        returnVal.Sort((x, y) => y - x);
        return returnVal;
    }
}