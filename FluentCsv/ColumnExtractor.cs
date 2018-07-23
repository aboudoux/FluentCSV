using System;
using System.Linq;
using System.Linq.Expressions;

namespace FluentCsv
{
    public class ColumnExtractor<TResult, TMember> : IColumnExtractor
    {
        private string _extractMemberName;
        private Func<string, TMember> _inThisWay;

        public ColumnExtractor()
        {
            var memberType = typeof(TMember);
            var conversionType = Nullable.GetUnderlyingType(memberType) ?? memberType;

            _inThisWay = dataType =>
            {
                if (string.IsNullOrWhiteSpace(dataType))
                    return default(TMember);
                return (TMember) Convert.ChangeType(dataType, conversionType);
            };
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
    }
}