using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace FluentCsv
{
    public static class Read
    {
        public static CsvReader Csv => new CsvReader();
    }

    public abstract class TechnicalDataContainer
    {
        protected TechnicalData TechnicalData { get; }

        protected TechnicalDataContainer(TechnicalData technicalData)
        {
            TechnicalData = technicalData;
        }
    }

    public class CsvReader
    {
        private TechnicalData _technicalData = new TechnicalData();

        public ChoiceBetweenDelimiterParametersAndResultSetBuilder FromFile(string fileName)
        {
            _technicalData.Source = File.ReadAllText(fileName);
            return new ChoiceBetweenDelimiterParametersAndResultSetBuilder(_technicalData);
        }

        public ChoiceBetweenDelimiterParametersAndResultSetBuilder FromString(string @string)
        {
            _technicalData.Source = @string;
            return new ChoiceBetweenDelimiterParametersAndResultSetBuilder(_technicalData);
        }

        public ChoiceBetweenDelimiterParametersAndResultSetBuilder FromStringArray(string[] stringArray)
        {
            return new ChoiceBetweenDelimiterParametersAndResultSetBuilder(_technicalData);
        }
    }

    public sealed class TechnicalData
    {
        public string Source { get; set; }
        public string ColumnDelimiter { get; set; } = ";";
        public string EndLineDelimiter { get; set; } = "\r\n";

        private Dictionary<int, ColumnInfos> Columns = new Dictionary<int, ColumnInfos>();

        public void AddColumn(int index)
        {
            if(Columns.ContainsKey(index))
                throw new Exception("Column already exists");
            Columns.Add(index, new ColumnInfos());
        }

        public void SetColumnMemberName(int columnIndex, string memberName)
        {
            if(!Columns.ContainsKey(columnIndex))
                throw new Exception($"Column with index {columnIndex} does not exists");
            Columns[columnIndex].MemberName = memberName;
        }

        public void SetColumnInThisWay<T>(int columnIndex, Func<string, T> inThisWay)
        {
            if (!Columns.ContainsKey(columnIndex))
                throw new Exception($"Column with index {columnIndex} does not exists");
            Columns[columnIndex].factory = inThisWay;
        }

        public object GetInThisWay(int columnIndex) => Columns[columnIndex].factory;

        private class ColumnInfos
        {
            public string MemberName { get; set; }
            public object factory { get; set; } = new Func<string, string>(a => a);
        }
    }

    public class ChoiceBetweenDelimiterParametersAndResultSetBuilder : TechnicalDataContainer
    {
        internal ChoiceBetweenDelimiterParametersAndResultSetBuilder(TechnicalData technicalData) : base(technicalData)
        {
        }
        
        public DelimiterParameters Where => new DelimiterParameters(TechnicalData);
        public ResultSetBuilder That => new ResultSetBuilder(TechnicalData);
    }

    public class DelimiterParameters : TechnicalDataContainer
    {
        private readonly ColumnDelimiter _columnDelimiter;
        private readonly EndLineDelimiter _endLineDelimiter;

        internal DelimiterParameters(TechnicalData technicalData) : base(technicalData)
        {
            _columnDelimiter = new ColumnDelimiter(TechnicalData);
            _endLineDelimiter = new EndLineDelimiter(TechnicalData);
        }

        public ChoiceBetweenEndLineDelimiterAndResultsetBuilder ColumnsAreDelimitedBy(string delimiter)
        {
            return _columnDelimiter.ColumnsAreDelimitedBy(delimiter);
        }

        public ChoiceBetweenColumnDelemiterAndResultsetBuilder LinesEndWith(string lineDelimiter)
        {
            return _endLineDelimiter.LinesEndWith(lineDelimiter);
        }
    }

    public class ColumnDelimiter : TechnicalDataContainer
    {
        public ChoiceBetweenEndLineDelimiterAndResultsetBuilder ColumnsAreDelimitedBy(string delimiter)
        {
            TechnicalData.ColumnDelimiter = delimiter;
            return new ChoiceBetweenEndLineDelimiterAndResultsetBuilder(TechnicalData);
        }

        internal ColumnDelimiter(TechnicalData technicalData) : base(technicalData)
        {
        }
    }

    public class EndLineDelimiter : TechnicalDataContainer
    {
        public ChoiceBetweenColumnDelemiterAndResultsetBuilder LinesEndWith(string lineDelimiter)
        {
            TechnicalData.EndLineDelimiter = lineDelimiter;
            return new ChoiceBetweenColumnDelemiterAndResultsetBuilder(TechnicalData);
        }

        internal EndLineDelimiter(TechnicalData technicalData) : base(technicalData)
        {
        }
    }

    public class ChoiceBetweenEndLineDelimiterAndResultsetBuilder : TechnicalDataContainer
    {
        public EndLineDelimiter And { get; }
        public ResultSetBuilder That { get; }

        public ChoiceBetweenEndLineDelimiterAndResultsetBuilder(TechnicalData technicalData) : base(technicalData)
        {
            And = new EndLineDelimiter(technicalData);
            That = new ResultSetBuilder(technicalData);
        }
    }

    public class ChoiceBetweenColumnDelemiterAndResultsetBuilder : TechnicalDataContainer
    {
        public ColumnDelimiter And { get; }
        public ResultSetBuilder That { get; }

        internal ChoiceBetweenColumnDelemiterAndResultsetBuilder(TechnicalData technicalData) : base(technicalData)
        {
            And = new ColumnDelimiter(technicalData);
            That = new ResultSetBuilder(technicalData);
        }
    }

    public class ResultSetBuilder : TechnicalDataContainer
    {
        internal ResultSetBuilder(TechnicalData technicalData) : base(technicalData){}

        public ColumnsBuilder<TLine> ReturnsLinesOf<TLine>() where TLine : new()
        {
            return new ColumnsBuilder<TLine>(TechnicalData);
        }
    }

    public class ColumnsBuilder<TLine> : TechnicalDataContainer
    {
        public ColumnsBuilder(TechnicalData technicalData) : base(technicalData)
        {
            Put = new ColumnBuilder<TLine>(TechnicalData);
        }

        public ColumnBuilder<TLine> Put { get; }
    }

    public class ColumnBuilder<TLine> : TechnicalDataContainer
    {
        internal ColumnBuilder(TechnicalData technicalData) : base(technicalData) { }

        public ChoiceBetweenAsAndInto<TLine> Column(int index)
        {
            TechnicalData.AddColumn(index);
            return new ChoiceBetweenAsAndInto<TLine>(TechnicalData, index);
        }
    }

    public class ChoiceBetweenAsAndInto<TLine> : TechnicalDataContainer
    {
        private readonly IntoBuilder<TLine, string> _intoBuilder;
        private readonly AsBuilder<TLine> _asBuilder;

        public ChoiceBetweenInThisWayAndInto<TLine, T> As<T>()
        {
            return _asBuilder.As<T>();
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, string>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        internal ChoiceBetweenAsAndInto(TechnicalData technicalData, int columnIndex) : base(technicalData)
        {
            _intoBuilder = new IntoBuilder<TLine, string>(TechnicalData, columnIndex);
            _asBuilder = new AsBuilder<TLine>(TechnicalData, columnIndex);
        }
    }

    public class AsBuilder<TLine> : TechnicalDataContainer
    {
        private readonly int _columnIndex;

        internal AsBuilder(TechnicalData technicalData, int columnIndex) : base(technicalData)
        {
            _columnIndex = columnIndex;
        }

        public ChoiceBetweenInThisWayAndInto<TLine, T> As<T>()
        {
            return new ChoiceBetweenInThisWayAndInto<TLine, T>(TechnicalData, _columnIndex);
        }
    }

    public class ChoiceBetweenInThisWayAndInto<TLine, TMember> : TechnicalDataContainer
    {
        private readonly InThisWhayBuilder<TLine, TMember> _inThisWhayBuilder;
        private readonly IntoBuilder<TLine, TMember> _intoBuilder;

        public IntoBuilder<TLine, TMember> InThisWay(Func<string, TMember> factory)
        {
            return _inThisWhayBuilder.InThisWay(factory);
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        internal ChoiceBetweenInThisWayAndInto(TechnicalData technicalData, int columnIndex) : base(technicalData)
        {
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, TMember>(TechnicalData, columnIndex);
            _intoBuilder = new IntoBuilder<TLine, TMember>(TechnicalData, columnIndex);
        }
    }

    public class InThisWhayBuilder<TLine, TMember> : TechnicalDataContainer
    {
        private readonly int _columnIndex;

        internal InThisWhayBuilder(TechnicalData technicalData, int columnIndex) : base(technicalData)
        {
            _columnIndex = columnIndex;
        }

        public IntoBuilder<TLine, TMember> InThisWay(Func<string, TMember> factory)
        {
            TechnicalData.SetColumnInThisWay(_columnIndex, factory);
            return new IntoBuilder<TLine, TMember>(TechnicalData, _columnIndex);
        }
    }

    public class IntoBuilder<TLine, TMember> : TechnicalDataContainer
    {
        private readonly int _columnIndex;
        internal IntoBuilder(TechnicalData technicalData, int columnIndex) : base(technicalData)
        {
            _columnIndex = columnIndex;
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            TechnicalData.SetColumnMemberName(_columnIndex, intoMember.Body.ToString());
            return new ChoiceBetweenPutOrGetAll<TLine>(TechnicalData);
        }

        
    }

    public class ChoiceBetweenPutOrGetAll<TLine> : TechnicalDataContainer
    {
        public ColumnBuilder<TLine> Put { get; }

        public IReadOnlyList<TLine> GetAll()
        {
            var lines = TechnicalData.Source
                .Split(new[] {TechnicalData.EndLineDelimiter}, StringSplitOptions.None)
                .SelectMany(a => a.Split(new[] {TechnicalData.ColumnDelimiter}, StringSplitOptions.None))
                .ToList();

            List<TLine> list = new List<TLine>();

          

            return list;
        }

        internal ChoiceBetweenPutOrGetAll(TechnicalData technicalData) : base(technicalData)
        {
            Put = new ColumnBuilder<TLine>(TechnicalData);
        }
    }
}
