namespace FluentCsv.Tests.Results
{
    public class TestResultWithMultiline
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }

        public static TestResultWithMultiline Create(string firstname=null, string lastname=null, string address=null)
            => new TestResultWithMultiline(){ Firstname = firstname, Lastname = lastname, Address = address};
    }
}