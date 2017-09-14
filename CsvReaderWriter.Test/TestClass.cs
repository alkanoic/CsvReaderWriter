using System.ComponentModel.DataAnnotations;

namespace CsvReaderWriter
{
	class TestClass : CsvReaderWriter.ICsvReaderValidator
	{
		[CsvReaderWriter.Attributes.CsvColumn(0, ColumnName ="Name1")]
		[CsvReaderWriter.Attributes.CsvKey(3, "Column1Key")]
		public string Column1 { get; set; }

		[CsvReaderWriter.Attributes.CsvColumn(1, ColumnName = "Name2")]
		[Required()]
		[StringLength(4,MinimumLength =4)]
		[CsvReaderWriter.Attributes.CsvKey(1, "Column2AndColumn3Key")]
		public string Column2 { get; set; }

		[CsvReaderWriter.Attributes.CsvColumn(2, ColumnName ="Name3")]
		[CsvReaderWriter.Attributes.CsvKey(1, "Column3Key")]
		[CsvReaderWriter.Attributes.CsvKey(2, "Column2AndColumn3Key")]
		public decimal Column3 { get; set; }

		[CsvReaderWriter.Attributes.CsvColumn(3, ColumnName = "Name4")]
		public int? Column4 { get; set; }

		public bool Validate(ref string propertyName, ref string errorMessage)
		{
			return true;
		}
	}
}
