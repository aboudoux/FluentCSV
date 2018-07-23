namespace FluentCsv.FluentReader
{
    public abstract class CsvParametersContainer
    {
        protected CsvParameters CsvParameters { get; }

        protected CsvParametersContainer(CsvParameters csvParameters)
        {
            CsvParameters = csvParameters;
        }
    }
}