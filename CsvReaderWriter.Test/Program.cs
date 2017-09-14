using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CsvReaderWriter.Test
{
	class Program
	{
		static void Main(string[] args)
		{

			var results = CsvReaderFile(@"TestCsv\TestCsvFile.csv", false, true, ',');

			CsvReaderFile(@"TestCsv\TestCsvFile_DoubleQuotation.csv", true, true, ',');

			CsvReaderFile(@"TestCsv\TestCsvFile_NoneTitle.csv", true, false, ',');

			CsvWriterFile(@"TestCsv\TestCsvFileWrite.csv", false, true, ',', results);

			CsvWriterFile(@"TestCsv\TestCsvFileWrite_DoubleQuotation.csv", true, true, ',', results);

			CsvWriterFile(@"TestCsv\TestCsvFileWrite_NoneTitle.csv", true, false, ',', results);

			Console.Read();
		}

		static IEnumerable<TestClass> CsvReaderFile(string fileName, bool doubleQuatation, bool columnTitle, char separateChar)
		{
			var sw = new Stopwatch();
			sw.Start();
			Console.WriteLine($"ReaderStart StopWatch:{sw.ElapsedMilliseconds}[msec]");
			var reader = new CsvReaderWriter.CsvReader<TestClass>();
			using (var stream = new StreamReader(fileName))
			{

				reader.DoubleQuotations = doubleQuatation;
				reader.ColumnTitleRow = columnTitle;
				reader.SeparationChars = separateChar;

				reader.ReadWithAllValidate(stream);
				Console.WriteLine($"StopWatch:{sw.ElapsedMilliseconds}[msec]");
				Console.WriteLine("Errors");
				foreach (var er in reader.Errors)
				{
					Console.WriteLine($"行:{er.RowIndex} 列:{er.ColumnIndex} 列名:{er.ColumnName} 内容:{er.FieldValue} エラー:{er.ErrorMessage} 例外:{er.Exception?.Message}");
				}
				Console.WriteLine($"StopWatch:{sw.ElapsedMilliseconds}[msec]");
				Console.WriteLine("RowResults");
				foreach(var c in reader.RawResults.Keys)
				{
					var r = reader.RawResults[c];
					Console.WriteLine($"行:{c} 列1:{r.Column1} 列2:{r.Column2} 列3:{r.Column3} 列:{r.Column4}");
				}
				Console.WriteLine($"StopWatch:{sw.ElapsedMilliseconds}[msec]");
				Console.WriteLine("Results");
				foreach (var c in reader.Results.Keys)
				{
					var r = reader.Results[c];
					Console.WriteLine($"行:{c} 列1:{r.Column1} 列2:{r.Column2} 列3:{r.Column3} 列:{r.Column4}");
				}
				Console.WriteLine($"StopWatch:{sw.ElapsedMilliseconds}[msec]");
			}
			Console.WriteLine($"ReaderEnd StopWatch:{sw.ElapsedMilliseconds}[msec]");
			sw.Stop();

			return reader.Results.Values;
		}

		static void CsvWriterFile(string fileName, bool doubleQuatation, bool columnTitle, char separateChar, IEnumerable<TestClass> results)
		{
			var sw = new Stopwatch();
			sw.Start();
			Console.WriteLine($"WriterStart StopWatch:{sw.ElapsedMilliseconds}[msec]");
			using (var stream = new StreamWriter(fileName))
			{
				var writer = new CsvReaderWriter.CsvWriter<TestClass>();
				writer.DoubleQuotations = doubleQuatation;
				writer.ColumnTitleRow = columnTitle;
				writer.SeparationChars = separateChar;

				writer.Write(stream, results);
			}
			Console.WriteLine($"WriterEnd StopWatch:{sw.ElapsedMilliseconds}[msec]");

			sw.Stop();
		}
	}
}
