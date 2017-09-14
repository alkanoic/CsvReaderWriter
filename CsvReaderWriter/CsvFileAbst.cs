using System;

namespace CsvReaderWriter
{
	public abstract class CsvFileAbst
	{
		/// <summary>
		/// 最初の1行目を除く場合True
		/// デフォルトTrue
		/// </summary>
		public bool ColumnTitleRow = true;

		/// <summary>
		/// 要素がダブルクォーテーションで囲まれている場合True
		/// デフォルトTrue
		/// </summary>
		public bool DoubleQuotations = true;

		/// <summary>
		/// 区切り文字
		/// デフォルト「,」
		/// </summary>
		public char SeparationChars = ',';

	}
}
