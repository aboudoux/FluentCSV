using System.Reflection;

namespace FluentCsv.Exceptions
{
    public class AssemblyResourceNotFoundException : FluentCsvException
    {
        public AssemblyResourceNotFoundException(Assembly assembly, string resourceName)
            : base($"cannot find the resource ${resourceName} in assembly {assembly.FullName}")
        {
        }
    }
}