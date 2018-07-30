using System;

namespace FluentCsv.Tests.Results
{
    public class TestResult
    {
        public string Member1 { get; set; }
        public int Member2 { get; set; }
        public DateTime Member3 { get; set; }
        public decimal? Member4 { get; set; }
        public TimeSpan Member5 { get; set; }

        public static TestResult Create(string member1 = null, int member2 = 0, DateTime member3 = default(DateTime), decimal? member4 = null, TimeSpan member5 = default(TimeSpan))
            => new TestResult() {Member1 = member1, Member2 = member2, Member3 = member3, Member4 = member4, Member5 = member5};
    }
}