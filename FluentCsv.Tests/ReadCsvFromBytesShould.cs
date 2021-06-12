using System.Linq;
using System.Text;
using FluentAssertions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
	public class ReadCsvFromBytesShould
	{		
		private const string Bom1 = "\uFEFF";
		private const string Bom2 = "\uFFFE";

		private class TestEncodings : TheoryData<(Encoding, string)>
		{
			public TestEncodings()
			{
				Add((new UTF8Encoding(true),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UTF8Encoding(false), $"{Bom1}test;test1\r\ncoucou;test"));			
				Add((new UTF32Encoding(),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UTF7Encoding(),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(true, true),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(true, false),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(false, true),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(false, false),$"{Bom1}test;test1\r\ncoucou;test"));
				Add((new UTF8Encoding(true),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UTF8Encoding(false),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UTF32Encoding(),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UTF7Encoding(),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(true, true),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(true, false),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(false, true),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new UnicodeEncoding(false, false),$"{Bom2}test;test1\r\ncoucou;test"));
				Add((new ASCIIEncoding(),"test;test1\r\ncoucou;test"));
				Add((Encoding.GetEncoding("ISO-8859-1"),"test;test1\r\ncoucou;test"));
			}
		}

		[Theory]
		[ClassData(typeof(TestEncodings))]
		public void WorksWithMultipleEncoding((Encoding encoding, string input) testValue)
		{			
			var input = testValue.encoding.GetBytes(testValue.input);

			var csv = Read.Csv.EncodedIn(testValue.encoding).FromBytes(input)
				.ThatReturns.ArrayOf<TestResult>()
				.Put.Column("test").Into(a => a.Member1)
				.GetAll();

			csv.ResultSet.First().Member1.Should().Be("coucou");
		}
	}
}