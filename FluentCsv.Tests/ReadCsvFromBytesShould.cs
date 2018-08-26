using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using NUnit.Framework;

namespace FluentCsv.Tests
{
	public class ReadCsvFromBytesShould
	{		
		private const string Bom1 = "\uFEFF";
		private const string Bom2 = "\uFFFE";

		private static readonly List<(Encoding,string)> TestEncodings = new List<(Encoding, string)> ()
		{
			(new UTF8Encoding(true),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UTF8Encoding(false),$"{Bom1}test;test1\r\ncoucou;test"),			
			(new UTF32Encoding(),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UTF7Encoding(),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(true, true),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(true, false),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(false, true),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(false, false),$"{Bom1}test;test1\r\ncoucou;test"),
			(new UTF8Encoding(true),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UTF8Encoding(false),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UTF32Encoding(),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UTF7Encoding(),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(true, true),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(true, false),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(false, true),$"{Bom2}test;test1\r\ncoucou;test"),
			(new UnicodeEncoding(false, false),$"{Bom2}test;test1\r\ncoucou;test"),
			(new ASCIIEncoding(),"test;test1\r\ncoucou;test"),
			(Encoding.GetEncoding("ISO-8859-1"),"test;test1\r\ncoucou;test")
		};

		[Test]
		[TestCaseSource("TestEncodings")]
		public void WorksWithMultipleEncoding((Encoding encoding, string input) testValue)
		{			
			var input = testValue.encoding.GetBytes(testValue.input);

			var csv = Read.Csv.FromBytes(input, testValue.encoding)
				.ThatReturns.ArrayOf<TestResult>()
				.Put.Column("test").Into(a => a.Member1)
				.GetAll();

			csv.ResultSet.First().Member1.Should().Be("coucou");
		}
	}
}