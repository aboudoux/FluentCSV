using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentCsv.Exceptions;

namespace FluentCsv.CsvParser
{
    public class ColumnExtractor<TResult, TMember> : IColumnExtractor
    {
        private PropertyInfo[] _propertiesInfos;
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
            var target = typeof(TResult);
            _propertiesInfos = GetMemberName().Select(CorrespondingPropertyInfo).ToArray();

            string[] GetMemberName() => into.Body.ToString().Split('.').Skip(1).ToArray();

            PropertyInfo CorrespondingPropertyInfo(string memberName)
            {
                var propertyInfo = target.GetProperty(memberName);
                target = propertyInfo.PropertyType;
                return propertyInfo;
            }
        }

        public void SetInThisWay(Func<string, TMember> parseFunc)
           => _inThisWay = parseFunc ?? throw new ArgumentNullException(nameof(parseFunc));

        public void Extract(object result, string columnData)
        {
            var instance = result;
            if (_propertiesInfos.Length > 1)
                _propertiesInfos
                    .WithoutLastElement()
                    .ForEach(SetInstance);

            _propertiesInfos.Last().SetValue(instance, _inThisWay(columnData));

            void SetInstance(PropertyInfo propertyInfo)
            {
                instance = propertyInfo.GetValue(instance);
                if (instance == null)
                    throw new NullPropertyInstanceException(propertyInfo);
            }
        }
    }
}