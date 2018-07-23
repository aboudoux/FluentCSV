using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FluentCsv
{
    public static class Read2
    {
        public static CsvReader2 Csv => new CsvReader2();
    }

    public class CsvReader2
    {
        public FileOptions FromString(string source)
        {
            return new FileOptions(source);
        }
    }

    public class FileOptions
    {
        private readonly string _source;
        private readonly GlobalOptions _globalOptions;
        private readonly ResultSetOptions _resultSetOptions;

        public FileOptions(string source)
        {
            _source = source;
            _resultSetOptions = new ResultSetOptions();
            _globalOptions = new GlobalOptions(_resultSetOptions);
        }

        public IGlobalOptions Where => _globalOptions;
        public ResultSetOptions That => _resultSetOptions;
    }

    public class GlobalOptions : IGlobalOptionsCoordinating, IGlobalOptions
    {
        public IGlobalOptions And => this;
        public ResultSetOptions That { get; }

        public GlobalOptions(ResultSetOptions resultSetOptions)
        {
            That = resultSetOptions;
        }

        public IGlobalOptionsCoordinating LineEndWith(string delimiter)
        {
            return this;
        }

        public IGlobalOptionsCoordinating ColumnsAreDelimitedBy(string delimiter)
        {
            return this;
        }
    }

    public class ResultSetOptions
    {
        public ColumnsOptions<TResult> ReturnsListOf<TResult>() => new ColumnsOptions<TResult>();
    }

    public class ColumnsOptions<TResult>
    {
        public IColumnsOptions PutColumn(int index)
        {
            return null;
        }
        public void As<TColumn>() { }
        public void InThisWay<TMember>(Func<string, TMember> factory) { }
        public void Into<TMember>(Expression<Func<TResult, TMember>> intoMember) { }
    }

    public interface IColumnsOptions
    {
        void As<TColumn>();
        void InThisWay<TMember>(Func<string, TMember> factory);
    }

    public class ColumnReaderReader<TColumn, TResut> : IColumnReader
    {
        private readonly TResut _result;

        public ColumnReaderReader(TResut result)
        {
            _result = result;
        }

        public int Index { get; }
        public Func<string, TColumn> TransfromInThisWay { get; }
        public Expression<Func<TResut, TColumn>> Into { get; }


        public void Read(string[] data, object result)
        {
            throw new NotImplementedException();
        }
    }

    public interface IColumnReader
    {
        void Read(string[] data, object result);
    }


    public interface IGlobalOptionsCoordinating
    {
        IGlobalOptions And { get; }
        ResultSetOptions That { get; }
    }

    public interface IGlobalOptions
    {
        IGlobalOptionsCoordinating LineEndWith(string delimiter);
        IGlobalOptionsCoordinating ColumnsAreDelimitedBy(string delimiter);
    }
}
