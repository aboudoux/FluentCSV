namespace FluentCsv.FluentReader
{
	public class Data  
	{
		protected Data() { }
		public static Data Valid => new ValidData();
		public static Data Invalid(string reason) => new InvalidData(reason);

	}

	public class ValidData : Data {
	}
	public class InvalidData : Data {
		public InvalidData(string reason) {
			Reason = reason;
		}
		public string Reason { get; }
	}
}