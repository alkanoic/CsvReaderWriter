using CsvReaderWriter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CsvReaderWriter
{
	class CsvAttributeReader<T>
	{

		public CsvAttributeReader()
		{
			this.ReadTypeAttributes();
		}

		/// <summary>
		/// 引数1：ColumnIndex
		/// 引数2：CsvReaderAttribute
		/// </summary>
		public IDictionary<int, CsvColumnAttribute> ReaderAttributes = new SortedDictionary<int, CsvColumnAttribute>();

		private Type type;
		private void ReadTypeAttributes()
		{
			this.type = typeof(T);

			foreach (PropertyInfo info in type.GetProperties())
			{
				this.GetCsvReaderAttributes(info);
				this.GetCsvKeyAttributes(info);
			}
		}

		/// <summary>
		/// CsvReaderAttributeを読み込む
		/// </summary>
		/// <param name="info"></param>
		private void GetCsvReaderAttributes(MemberInfo info)
		{
			Attribute[] attrs = Attribute.GetCustomAttributes(info, typeof(CsvColumnAttribute));
			foreach (Attribute attr in attrs)
			{
				CsvColumnAttribute csvattr = attr as CsvColumnAttribute;
				if (csvattr != null)
				{
					csvattr.PropertyName = info.Name;
					ReaderAttributes.Add(csvattr.columnIndex, csvattr);
				}
			}
		}

		/// <summary>
		/// 列番号に対するPropertyInfoを返却する。
		/// </summary>
		/// <param name="columnIndex">列番号</param>
		public PropertyInfo GetPropertyInfoByColumnIndex(int columnIndex)
		{
			return this.type.GetProperty(this.ReaderAttributes[columnIndex].PropertyName);
		}

		/// <summary>
		/// 列番号に対する列名を返却する。
		/// </summary>
		/// <param name="columnIndex">列番号</param>
		public string GetColumnNameByColumnIndex(int columnIndex)
		{
			return this.ReaderAttributes[columnIndex].ColumnName;
		}

		/// <summary>
		/// インスタンスの指定のプロパティを返却する。
		/// </summary>
		/// <param name="result">インスタンス</param>
		/// <param name="propertyName">プロパティ名</param>
		public object GetValueByPropertyName(T result, string propertyName)
		{
			return this.type.GetProperty(propertyName).GetValue(result, null);
		}

		/// <summary>
		/// プロパティ名に対する列番号を返却する。
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		public int GetColumnIndexByPropertyName(string propertyName)
		{
			return ReaderAttributes.Values.Where(_ => _.PropertyName == propertyName).First().columnIndex;
		}


		/// <summary>
		/// 引数1：KeyPair
		/// 引数2：PropertyNames
		/// </summary>
		public IDictionary<int, IList<string>> KeyPairs = new Dictionary<int, IList<string>>();

		/// <summary>
		/// 引数1：KeyPair
		/// 引数2：KeyPairName
		/// </summary>
		public IDictionary<int, string> KeyPairNames = new Dictionary<int, string>();

		/// <summary>
		/// CsvKeyAttributeを読み込む
		/// </summary>
		/// <param name="info"></param>
		private void GetCsvKeyAttributes(MemberInfo info)
		{
			Attribute[] attrs = Attribute.GetCustomAttributes(info, typeof(CsvKeyAttribute));
			foreach (Attribute attr in attrs)
			{
				CsvKeyAttribute csvattr = attr as CsvKeyAttribute;
				if (csvattr != null)
				{
					if (KeyPairs.ContainsKey(csvattr.KeyPair) == false)
					{
						KeyPairs.Add(csvattr.KeyPair, new List<string>() { info.Name });
						KeyPairNames.Add(csvattr.KeyPair, csvattr.PairName);
					}
					else
					{
						KeyPairs[csvattr.KeyPair].Add(info.Name);
						if (string.IsNullOrEmpty(KeyPairNames[csvattr.KeyPair]))
						{
							KeyPairNames[csvattr.KeyPair] = csvattr.PairName;
						}
					}
				}
			}
		}
	}
}
