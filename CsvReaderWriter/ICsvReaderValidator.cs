namespace CsvReaderWriter
{
	public interface ICsvReaderValidator
	{
		bool Validate(ref string propertyName, ref string errorMessage);
	}
}
