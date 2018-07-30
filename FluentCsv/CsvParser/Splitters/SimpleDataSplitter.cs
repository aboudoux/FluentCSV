﻿using System;

namespace FluentCsv.CsvParser.Splitters
{
    public class SimpleDataSplitter : IDataSplitter
    {
        public string[] SplitColumns(string input, string columnDelimiter)        
            => input.Split(new[] { columnDelimiter }, StringSplitOptions.None);

        public string[] SplitLines(string input, string lineDelimiter)
            => input.Split(new[] { lineDelimiter }, StringSplitOptions.None);
    }
}