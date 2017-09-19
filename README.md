# CsvReaderWriter
## 概要
Csvファイルを読み書きするライブラリ  
.NET Framework 4.5以降をサポート

## インストール
``` ps
PM> Install-Package CsvReaderWriter
```

## 使い方
### C#
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

### VB.NET
``` vb
Public Class TestClass
    Implements ICsvReaderValidator

    <CsvReaderWriter.Attributes.CsvColumn(0, ColumnName:="Name1")>
    <CsvReaderWriter.Attributes.CsvKey(1, "Column1Key")>
    Public Property Column1 As String

    <CsvReaderWriter.Attributes.CsvColumn(1, ColumnName:="Name2")>
    <Required>
    Public Property Column2 As String

    <CsvReaderWriter.Attributes.CsvColumn(2, ColumnName:="Name3")>
    Public Property Column3 As Decimal

    <CsvReaderWriter.Attributes.CsvColumn(3, ColumnName:="Name4")>
    Public Property Column4 As Integer?

    Public Function Validate(ByRef propertyName As String, ByRef errorMessage As String) As Boolean Implements ICsvReaderValidator.Validate
        Return True
    End Function
End Class

Module Module
    Sub Main()

        Dim reader = New CsvReaderWriter.CsvReader(Of TestClass)
        Using stream As New StreamReader("hoge.csv")
            reader.ReadWithAllValidate(stream)

            For Each er In reader.Errors
                Console.WriteLine("行:{0} 列:{1} 列名:{2} 内容:{3} エラー:{4} 例外:{5}",
                				  er.RowIndex, er.ColumnIndex, er.ColumnName, er.FieldValue, er.ErrorMessage, er.Exception?.Message)
            Next

            For Each c In reader.RawResults.Keys
                Dim r = reader.RawResults(c)
                Console.WriteLine("行:{0} 列1:{1} 列2:{2} 列3:{3} 列4:{4}",
                				  c, r.Column1, r.Column2, r.Column3, r.Column4)
            Next

            For Each c In reader.Results.Keys
                Dim r = reader.Results(c)
                Console.WriteLine("行:{0} 列1:{1} 列2:{2} 列3:{3} 列4:{4}",
                				  c, r.Column1, r.Column2, r.Column3, r.Column4)
            Next
        End Using

        Dim writer As New CsvReaderWriter.CsvWriter(Of TestClass)
        Using stream As New StreamWriter("huga.csv")
            writer.Write(stream, results)
        End Using
    End Sub
End Module
```

## License
This library is under the [MIT License](https://opensource.org/licenses/mit-license.html).
