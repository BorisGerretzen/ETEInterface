namespace DataProcessing.DataProcessor
{
    public abstract class AbstractProcessor {
        protected string Filter = null!;
        protected readonly string Directory;
        protected readonly bool Separate;

        protected AbstractProcessor(string directory, bool separate) {
            Directory = directory;
            Separate = separate;
        }

        public abstract void Process(string outputFile);
    }
}
