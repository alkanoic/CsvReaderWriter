# CsvReaderWriter
## 概要
Csvファイルを読み書きするライブラリ  
.NET Framework 4.5以降をサポート

## インストール
``` ps
PM> Install-Package CsvReaderWriter
```

## 使い方
``` cs
class TestClass : CsvReaderWriter.ICsvReaderValidator
{
    //1列目、列名Name1
    //1列目、重複チェック
    [CsvReaderWriter.Attributes.CsvColumn(0, ColumnName ="Name1")]
	[CsvReaderWriter.Attributes.CsvKey(1, "Column1Key")]
    public string Column1 { get; set; }

    //２列目、列名Name2
    //DataAnnotationによる必須入力検証
    [CsvReaderWriter.Attributes.CsvColumn(1, ColumnName = "Name2")]
	[Required()]
    public string Column2 { get; set; }

    [CsvReaderWriter.Attributes.CsvColumn(2, ColumnName ="Name3")]
    public decimal Column3 { get; set; }

    [CsvReaderWriter.Attributes.CsvColumn(3, ColumnName = "Name4")]
    public int? Column4 { get; set; }

    //ICsvReaderValidator
    //読み込み時に検証を追加する
    public bool Validate(ref string propertyName, ref string errorMessage)
    {
        return true;
    }
}


class Program
{
    static void Main(string[] args)
    {
        var reader = new CsvReaderWriter.CsvReader<TestClass>();
        using (var stream = new StreamReader("hoge.csv"))
        {
            //取り込み検証（型変換、DataAnnotation、重複チェック）
            reader.ReadWithAllValidate(stream);
            //エラー情報（型変換、DataAnnotation、重複チェック）
            foreach (var er in reader.Errors)
            {
                Console.WriteLine($"行:{er.RowIndex} 列:{er.ColumnIndex} 列名:{er.ColumnName} 内容:{er.FieldValue} エラー:{er.ErrorMessage} 例外:{er.Exception?.Message}");
            }
            //取り込み失敗を含む取り込み結果
            foreach(var c in reader.RawResults.Keys)
            {
                var r = reader.RawResults[c];
                Console.WriteLine($"行:{c} 列1:{r.Column1} 列2:{r.Column2} 列3:{r.Column3} 列:{r.Column4}");
            }
            //取り込み成功の結果（重複検証エラーのものは含む）
            foreach (var c in reader.Results.Keys)
            {
                var r = reader.Results[c];
                Console.WriteLine($"行:{c} 列1:{r.Column1} 列2:{r.Column2} 列3:{r.Column3} 列:{r.Column4}");
            }
        }

        var writer = new CsvReaderWriter.CsvWriter<TestClass>();
        using (var stream = new StreamWriter(@"huga.csv"))
        {
            writer.Write(stream, reader.Results.Values);
        }
    }
}
```

## License
This library is under the [MIT License](https://opensource.org/licenses/mit-license.html).
