using System;
using System.IO;

namespace FluentCsv.Tests
{
    public class GetTestFilePath
    {
        private readonly string _dataPath;

        private GetTestFilePath(string directoryName)
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryName);
        }

        public static GetTestFilePath FromDirectory(string directoryName) => new GetTestFilePath(directoryName);

        public string AndFileName(string fileName) => Path.Combine(_dataPath, fileName);
    }
}