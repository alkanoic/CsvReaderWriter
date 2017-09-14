using System;

namespace CsvReaderWriter.Attributes
{
	/// <summary>
	/// Csv内の一意キーを検証するための属性クラス
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class CsvKeyAttribute : Attribute
	{
		/// <summary>
		/// Keyペアの番号
		/// 一つのプロパティに対して複数のKeyを持てる。
		/// </summary>
		public int KeyPair;

		/// <summary>
		/// Keyペアの名前
		/// 同じ番号で複数のKeyペアが指定される場合はどちらかが採用される。
		/// </summary>
		public string PairName;

		/// <summary>
		/// Csv内の一意キーを設定する。
		/// </summary>
		/// <param name="pair">Keyペアの番号</param>
		/// <param name="name">Keyペアの名前</param>
		public CsvKeyAttribute(int pair, string name)
		{
			this.KeyPair = pair;
			this.PairName = name;
		}
	}
}
