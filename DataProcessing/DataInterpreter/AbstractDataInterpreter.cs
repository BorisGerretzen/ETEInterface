namespace DataProcessing.DataInterpreter;

internal abstract class AbstractDataInterpreter {
    protected readonly List<List<(double, double)>> _data;

    /// <summary>
    ///     Create a new interpreter, these are used to transform the samples into usable data
    /// </summary>
    /// <param name="data"></param>
    protected AbstractDataInterpreter(List<List<(double, double)>> data) {
        _data = data;
    }

    /// <summary>
    ///     Gets the interpreted data
    /// </summary>
    /// <returns></returns>
    public abstract List<double> GetData();
}