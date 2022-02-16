namespace DataProcessing.DataProcessor;

public abstract class AbstractProcessor {
    protected List<string> Headers;
    protected readonly string Directory;
    protected readonly bool Separate;
    protected Dictionary<string, bool> _headersActive;
    protected string Filter = null!;

    protected AbstractProcessor(string directory, bool separate) {
        Directory = directory;
        Separate = separate;
    }

    /// <summary>
    /// Processes the data in the input directory and writes the aggregated results to the output file.
    /// </summary>
    /// <param name="outputFile">File to write the data to.</param>
    public abstract void Process(string outputFile);

    /// <summary>
    /// Gets a list of all possible headers.
    /// </summary>
    /// <returns></returns>
    public List<string> GetHeaders() {
        return Headers;
    }

    /// <summary>
    /// Sets the list of possible headers
    /// </summary>
    /// <param name="headers"></param>
    protected void SetHeaders(List<string> headers) {
        Headers = headers;
    }

    /// <summary>
    /// Sets the list of headers that need to appear in the output file.
    /// </summary>
    /// <param name="headers"></param>
    public void SetHeadersActive(Dictionary<string, bool> headers) {
        _headersActive = headers;
    }

    /// <summary>
    /// Gets a list of indices that need to be removed from the data because they were not selected.
    /// </summary>
    /// <returns></returns>
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