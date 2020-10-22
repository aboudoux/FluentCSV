using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentCsv.FluentReader;

namespace FluentCsv.CsvParser
{
	public class TupleColumnExtractor<TResult, TMember> : ColumnExtractor<TResult, TMember>
	{
		private FieldInfo[] _tupleFieldInfos;

		public TupleColumnExtractor(int columnIndex, CultureInfo cultureInfo, string columnName = null) : base(columnIndex, cultureInfo, columnName)
		{
		}

		public override void SetInto(Expression<Func<TResult, TMember>> into)
		{
			var target = typeof(TResult);

			_tupleFieldInfos = GetMemberName().Select(CorrespondingFieldInfo).ToArray();

			string[] GetMemberName() => into.Body.ToString().Split('.').Skip(1).ToArray();

			FieldInfo CorrespondingFieldInfo(string memberName) {
				var fieldInfo = target.GetField(memberName);
				return fieldInfo;
			}
		}

		public override Data Extract(object source, string columnData, out  object result)
		{
			var validationResult = Validator(columnData);
			if (validationResult is InvalidData)
			{
				result = default;
				return validationResult;
			}

			object boxInstance = source;
			_tupleFieldInfos.Last().SetValue(boxInstance, InThisWay(columnData));
			result = (TResult) boxInstance;
			return Data.Valid;
		}
	}
}