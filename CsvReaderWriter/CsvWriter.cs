using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvReaderWriter
{
	public class CsvWriter<T> : CsvFileAbst
	{
		/// <summary>
		/// CSV書き込み時のエラーメッセージクラス
		/// </summary>
		public CsvReaderMessage CsvReaderMessage = new CsvReaderMessage();

		private CsvAttributeReader<T> attributeReader = new CsvAttributeReader<T>();

		/// <summary>
		/// ファイルに出力する。
		/// </summary>
		/// <param name="writer">出力ファイル</param>
		/// <param name="e">出力内容</param>
		public void Write(TextWriter writer, IEnumerable<T> e)
		{
			if (base.ColumnTitleRow)
			{
				this.WriteHeader(writer);
			}
			foreach (var r in e)
			{
				this.WriteLine(writer, r);
			}
			writer.Flush();
		}

		/// <summary>
		/// ファイルに出力する。
		/// </summary>
		/// <param name="writer">出力ファイル</param>
		/// <param name="e">出力内容</param>
		public void Write(TextWriter writer, IEnumerable<IEnumerable<T>> e)
		{
			if (base.ColumnTitleRow)
			{
				this.WriteHeader(writer);
			}
			foreach (var r in e)
			{
				foreach(var rr in r)
				{
					this.WriteLine(writer, rr);
				}
			}
			writer.Flush();
		}

		private void WriteHeader(TextWriter writer)
		{
			var firstColumn = true;
			var builder = new StringBuilder();
			foreach(var r in this.attributeReader.ReaderAttributes.Values)
			{
				var writeValue = this.WriteValue(r.ColumnName);
				if (firstColumn == false)
				{
					writeValue = ',' + writeValue;
				}
				builder.Append(writeValue);
				firstColumn = false;
			}
			writer.WriteLine(builder.ToString());
		}

		private void WriteRows(TextWriter writer, IEnumerable<T> rows)
		{
			foreach (var r in rows)
			{
				this.WriteLine(writer, r);
			}
		}

		private void WriteLine(TextWriter writer, T r)
		{
			var firstColumn = true;
			var builder = new StringBuilder();
			foreach (var a in this.attributeReader.ReaderAttributes.Values)
			{
				var writeValue = this.WriteValue(this.attributeReader.GetValueByPropertyName(r, a.PropertyName));
				if (firstColumn == false)
				{
					writeValue = ',' + writeValue;
				}
				builder.Append(writeValue);
				firstColumn = false;
			}
			writer.WriteLine(builder.ToString());
		}

		private string WriteValue(object value)
		{
			string result;
			if(value == null)
			{
				result = string.Empty;
			}
			else
			{
				result = value.ToString();
			}
			if (this.DoubleQuotations)
			{
				result = '"' + result + '"';
			}
			return result;
		}

	}
}
