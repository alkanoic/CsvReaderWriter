using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CsvReaderWriter
{
	public class CsvReader<T> : CsvFileAbst where T : new()
	{

		/// <summary>
		/// CSV読み込み時のエラーメッセージクラス
		/// </summary>
		public CsvReaderMessage CsvReaderMessage = new CsvReaderMessage();

		private CsvAttributeReader<T> attributeReader = new CsvAttributeReader<T>();

		private IDictionary<int, T> rawResults = new Dictionary<int, T>();

		/// <summary>
		/// 取得したCSVの結果
		/// 変換エラーが発生した行も含まれます。
		/// 変換エラーが発生した場合は列要素が欠けることになります。
		/// Keys:行番号　1行目より開始
		/// </summary>
		public IReadOnlyDictionary<int, T> RawResults
		{
			get => new ReadOnlyDictionary<int, T>(this.rawResults);
		}

		private IDictionary<int, T> results = new Dictionary<int, T>();

		/// <summary>
		/// 取得したCSVの結果
		/// 検証エラーが発生した行は除かれます。
		/// Keys:行番号　1行目より開始
		/// </summary>
		public IReadOnlyDictionary<int, T> Results
		{
			get => new ReadOnlyDictionary<int, T>(this.results);
		}

		private IList<CsvError> errors = new List<CsvError>();

		/// <summary>
		/// 取得したCSVの検証結果
		/// </summary>
		public IEnumerable<CsvError> Errors { get => errors; }

		/// <summary>
		/// 成功した場合True
		/// エラーが発生した場合False
		/// </summary>
		public bool Success
		{
			get => errors is null ? true : errors.Count() == 0;
		}

		/// <summary>
		/// ファイルを読み込みます。
		/// 検証も読み込みと同時に行います。
		/// </summary>
		public void ReadWithAllValidate(TextReader reader, bool validateDataAnnotations = true, bool validateCsvReaderValidator = true, bool validateAllKeyPairs = true)
		{
			if (this.ColumnTitleRow == true)
			{
				reader.ReadLine();
			}

			var currentRowIndex = 0;
			while (reader.Peek() > -1)
			{
				currentRowIndex++;
				var line = reader.ReadLine();
				var contents = this.SeparateLine(line);

				var result = new T();

				if (this.ConvertLineContentsToClass(ref result, contents, currentRowIndex) == false)
				{
					continue;
				}

				if (validateDataAnnotations)
				{
					if (this.ValidateDataAnnotations(result, currentRowIndex) == false)
					{
						continue;
					}
				}

				if (validateCsvReaderValidator)
				{
					if (this.ValidateCsvReaderValidator(result, currentRowIndex) == false)
					{
						continue;
					}
				}

				this.results.Add(currentRowIndex, result);
			}

			if (validateAllKeyPairs)
			{
				this.ValidateAllKeyPairs();
			}
		}

		private bool ConvertLineContentsToClass(ref T result, string[] contents, int currentRowIndex)
		{
			var isSuccess = true;
			int currentColumnIndex = -1;

			try
			{
				foreach (var index in this.attributeReader.ReaderAttributes.Keys)
				{
					currentColumnIndex = index;
					PropertyInfo info = this.attributeReader.GetPropertyInfoByColumnIndex(index);
					if (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						if (string.IsNullOrWhiteSpace(contents[index]) == false)
						{
							info.SetValue(result, Convert.ChangeType(contents[index], Nullable.GetUnderlyingType(info.PropertyType)));
						}
					}
					else
					{
						info.SetValue(result, Convert.ChangeType(contents[index], info.PropertyType));
					}
				}
			}
			catch (Exception ex)
			{
				this.AddCsvError(currentRowIndex,
					currentColumnIndex,
					contents[currentColumnIndex],
					this.CsvReaderMessage.TypeConvertErrorMessage,
					ex);
				isSuccess = false;
			}
			this.rawResults.Add(currentRowIndex, result);

			return isSuccess;
		}

		/// <summary>
		/// クラスに対して検証を実行するに
		/// </summary>
		/// <param name="target">Csvを読み込んだインスタンス</param>
		/// <param name="targetRowIndex">現在の行番号</param>
		/// <returns>成功した場合True</returns>
		public bool ValidateDataAnnotations(T target, int targetRowIndex = -1)
		{
			var validateContext = new ValidationContext(target, null, null);
			var validateResult = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(target, validateContext, validateResult, true);
			if (isValid == false)
			{
				if(targetRowIndex == -1)
				{
					return false;
				}

				foreach (var vr in validateResult)
				{
					foreach (var name in vr.MemberNames)
					{
						this.AddCsvError(targetRowIndex,
							this.attributeReader.GetColumnIndexByPropertyName(name),
							this.attributeReader.GetValueByPropertyName(target, name)?.ToString(),
							vr.ErrorMessage);
					}
				}
				return false;
			}

			return true;
		}

		/// <summary>
		/// CsvReaderValidatorで実装された検証メソッドを実行する
		/// </summary>
		/// <param name="target">Csvを読み込んだインスタンス</param>
		/// <param name="targetRowIndex">対象の行番号</param>
		/// <returns></returns>
		private bool ValidateCsvReaderValidator(T target, int targetRowIndex = -1)
		{
			var tar = target as ICsvReaderValidator;

			if(tar is null)
			{
				return true;
			}

			string propertyName = null;
			string message = null;

			if (tar.Validate(ref propertyName, ref message) == false)
			{
				if (targetRowIndex == -1)
				{
					return false;
				}

				this.AddCsvError(targetRowIndex,
					this.attributeReader.GetColumnIndexByPropertyName(propertyName),
					this.attributeReader.GetValueByPropertyName(target, propertyName)?.ToString(),
					message);

				return false;
			}

			return true;
		}

		/// <summary>
		/// Keyペアの重複検証
		/// </summary>
		/// <returns>成功した場合True</returns>
		public bool ValidateAllKeyPairs()
		{
			bool success = true;
			foreach (var kp in this.attributeReader.KeyPairs.Keys)
			{
				if (this.ValidateKeyPair(kp))
				{
					success = false;
				}
			}
			return success;
		}

		/// <summary>
		/// Keyペアの重複検証
		/// </summary>
		/// <param name="keyPairNo">KeyPairNo</param>
		/// <returns>成功した場合True</returns>
		public bool ValidateKeyPair(int keyPairNo)
		{
			if (this.attributeReader.KeyPairs.ContainsKey(keyPairNo) == false) return false;

			var success = true;
			var dict = new Dictionary<IEnumerable<object>, int>();
			foreach (var r in this.results.Keys)
			{
				var list = new List<object>();
				string firstPropertyName = string.Empty;
				foreach (var propertyName in this.attributeReader.KeyPairs[keyPairNo])
				{
					list.Add(this.attributeReader.GetValueByPropertyName(this.results[r], propertyName));
					if (string.IsNullOrEmpty(firstPropertyName))
					{
						firstPropertyName = propertyName;
					}
				}

				var equal = false;
				foreach (var k in dict.Keys)
				{
					if (k.SequenceEqual(list))
					{
						var firstRowIndex = dict[k];
						this.AddCsvError(r,
							this.attributeReader.GetColumnIndexByPropertyName(firstPropertyName),
							this.attributeReader.GetValueByPropertyName(this.results[r], firstPropertyName).ToString(),
							string.Format(CsvReaderMessage.KeyPairValueErrorMessage, firstRowIndex, r, this.attributeReader.KeyPairNames[keyPairNo]));
						success = false;
						equal = true;
					}
				}

				if (equal)
				{
					dict.Add(list, r);
				}

				if (dict.Count == 0)
				{
					dict.Add(list, r);
					continue;
				}

			}

			return success;
		}

		private void AddCsvError(int rowIndex, int columnIndex, string value, string message, Exception ex = null)
		{
			var e = new CsvError();
			e.RowIndex = rowIndex;
			e.ColumnIndex = columnIndex;
			e.ColumnName = this.attributeReader.GetColumnNameByColumnIndex(columnIndex);
			e.FieldValue = value;
			e.ErrorMessage = message;
			e.Exception = ex;

			this.errors.Add(e);
		}

		/// <summary>
		/// エラーをクリアします。
		/// </summary>
		public void ClearErrors()
		{
			this.errors.Clear();
		}

		/// <summary>
		/// 読み込み結果をクリアします。
		/// </summary>
		public void ClearResults()
		{
			this.rawResults.Clear();
			this.results.Clear();
		}

		private string[] SeparateLine(string line)
		{
			if (this.DoubleQuotations)
			{
				var split = line.Split(new string[] { $"\"{this.SeparationChars}\"" }, StringSplitOptions.None);
				if (split != null)
				{
					if (split[0].StartsWith("\""))
					{
						split[0] = split[0].Remove(0, 1);
					}
					var lastIndex = split.Length - 1;
					if(split[lastIndex].EndsWith("\""))
					{
						split[lastIndex] = split[lastIndex].Remove(split[lastIndex].Length - 1);
					}
				}
				return split;
			}
			else
			{
				return line.Split(this.SeparationChars);
			}
		}

	}
}
