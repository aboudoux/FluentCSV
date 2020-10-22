using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;

namespace FluentCsv.CsvParser
{
    public class ColumnExtractor<TResult, TMember> : IColumnExtractor<TResult>
    {
        private PropertyInfo[] _propertiesInfos;

        protected Func<string, TMember> InThisWay;
        protected Func<string, Data> Validator = s => Data.Valid;

        public string ColumnName { get; }

        public int ColumnIndex { get; }

        public ColumnExtractor(int columnIndex, CultureInfo cultureInfo, string columnName = null)
        {
            ColumnIndex = columnIndex;
            ColumnName = columnName;

            var memberType = typeof(TMember);
            var conversionType = Nullable.GetUnderlyingType(memberType) ?? memberType;

            InThisWay = dataType => dataType.IsEmpty() ? 
                                     default(TMember) : 
                                     (TMember) Convert.ChangeType(dataType, conversionType, cultureInfo);
        }

        public virtual void SetInto(Expression<Func<TResult, TMember>> into)
        {
	        var target = typeof(TResult);
	        _propertiesInfos = GetMemberName().Select(CorrespondingPropertyInfo).ToArray();

	        string[] GetMemberName() => into.Body.ToString().Split('.').Skip(1).ToArray();

	        PropertyInfo CorrespondingPropertyInfo(string memberName) {
		        var propertyInfo = target.GetProperty(memberName);
		        target = propertyInfo.PropertyType;
		        return propertyInfo;
	        }
        }

        public void SetInThisWay(Func<string, TMember> parseFunc)
           => InThisWay = parseFunc ?? throw new ArgumentNullException(nameof(parseFunc));

        public void SetValidator(Func<string, Data> dataValidator)
			=> Validator = dataValidator ?? throw new ArgumentNullException(nameof(dataValidator));

        public virtual Data Extract(object source, string columnData, out object result)
        {
	        var validationResult = Validator(columnData);
	        if (validationResult is InvalidData)
	        {
		        result = default;
		        return validationResult;
	        }

	        var instance = source;
            if (_propertiesInfos.Length > 1)
                _propertiesInfos
                    .WithoutLastElement()
                    .ForEach(SetInstance);

            _propertiesInfos.Last().SetValue(instance, InThisWay(columnData));
            result = source;
            return Data.Valid;

            void SetInstance(PropertyInfo propertyInfo)
            {
                instance = propertyInfo.GetValue(instance);
                if (instance == null)
                    throw new NullPropertyInstanceException(propertyInfo);
            }
        }
    }
}