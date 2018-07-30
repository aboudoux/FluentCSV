using System;
using System.Linq;
using System.Linq.Expressions;

namespace FluentCsv.CsvParser
{
    public class ColumnExtractor<TResult, TMember> : IColumnExtractor
    {
        private string _extractMemberName;
        private Func<string, TMember> _inThisWay;

        public string ColumnName { get; }
        public int ColumnIndex { get; }

        public ColumnExtractor(int columnIndex, string columnName = null)
        {
            ColumnIndex = columnIndex;
            ColumnName = columnName;

            var memberType = typeof(TMember);
            var conversionType = Nullable.GetUnderlyingType(memberType) ?? memberType;

            _inThisWay = dataType => dataType.IsEmpty() ? 
                                     default(TMember) : 
                                     (TMember) Convert.ChangeType(dataType, conversionType);
        }

        public void SetInto(Expression<Func<TResult, TMember>> into)
        {
            _extractMemberName = GetMemberName();
            string GetMemberName() => into.Body.ToString().Split('.').Last();
        }

        public void SetInThisWay(Func<string, TMember> parseFunc)
        {
            _inThisWay = parseFunc ?? throw new ArgumentNullException(nameof(parseFunc));
        }

        public void Extract(object result, string columnData)
        {
            result.GetType().GetProperty(_extractMemberName)?.SetValue(result, _inThisWay(columnData));
        }
    }

    public interface IColumnExtractor
    {
        void Extract(object result, string columnData);

        int ColumnIndex { get; }
        string ColumnName { get; }
    }
}