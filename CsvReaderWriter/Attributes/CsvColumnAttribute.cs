using System;

namespace CsvReaderWriter.Attributes
{
	/// <summary>
	/// Csvを読み込むための属性クラス
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class CsvColumnAttribute : Attribute
	{
		/// <summary>
		/// 列番号
		/// 0から始まる
		/// </summary>
		public int columnIndex;

		/// <summary>
		/// Csvの属性を設定する
		/// </summary>
		/// <param name="index">列番号（0から始まる）</param>
		public CsvColumnAttribute(int index)
		{
			this.columnIndex = index;
		}

		/// <summary>
		/// 列名
		/// </summary>
		public string ColumnName;
		internal string PropertyName;
	}
}
