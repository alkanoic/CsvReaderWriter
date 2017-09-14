using System;

namespace CsvReaderWriter
{
	public class CsvError
	{
		public int ColumnIndex;
		public int RowIndex;
		public string ColumnName;
		public string FieldValue;
		public string ErrorMessage;
		public Exception Exception;
	}
}
