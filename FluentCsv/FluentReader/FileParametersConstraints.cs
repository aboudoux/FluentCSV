namespace FluentCsv.FluentReader
{
    public class FileParametersConstraints : CsvParametersContainer
    {
        public FluentFileParameters And { get; }
        public ResultSetBuilder ThatReturns { get; }

        internal FileParametersConstraints(CsvParameters csvParameters, FluentFileParameters fileParameters) : base(csvParameters)
        {
            And = fileParameters;
            ThatReturns = new ResultSetBuilder(csvParameters);
        }
    }
}