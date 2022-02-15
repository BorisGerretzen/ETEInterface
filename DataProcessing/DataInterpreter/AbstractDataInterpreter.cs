namespace DataProcessing.DataInterpreter; 

internal abstract class AbstractDataInterpreter {
    protected readonly List<List<(double, double)>> _data;

    protected AbstractDataInterpreter(List<List<(double, double)>> data) {
        _data = data;
    }

    public abstract List<double> GetData();
}