namespace DataProcessing.DataProcessor;

public abstract class AbstractProcessor {
    protected static List<string> Headers;
    protected readonly string Directory;
    protected readonly bool Separate;
    protected Dictionary<string, bool> _headersActive;
    protected string Filter = null!;

    protected AbstractProcessor(string directory, bool separate) {
        Directory = directory;
        Separate = separate;
    }

    public abstract void Process(string outputFile);

    public List<string> GetHeaders() {
        return Headers;
    }

    protected static void SetHeaders(List<string> headers) {
        Headers = headers;
    }

    public void SetHeadersActive(Dictionary<string, bool> headers) {
        _headersActive = headers;
    }

    protected List<int> RemoveFromResults() {
        var returnVal = new List<int>();

        var headers = GetHeaders();
        if (_headersActive == null || _headersActive.Count == 0) return returnVal;

        foreach (var (key, val) in _headersActive)
            if (!val)
                returnVal.Add(headers.IndexOf(key));

        returnVal.Sort((x, y) => y - x);
        return returnVal;
    }
}