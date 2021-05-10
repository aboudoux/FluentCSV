using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
	public class ReadCsvFromStreamShould
	{
		[Fact]
		public void WorksWithSimpleCsvFromMemoryStream()
		{
			var input = "test1;test2\r\ncoucou;test";
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

			var csv = Read.Csv.FromStream(stream)
				.ThatReturns.ArrayOf<TestResult>()
				.Put.Column("test1").Into(a => a.Member1)
				.GetAll();

			csv.ResultSet.First().Member1.Should().Be("coucou");			
		}
	}
}