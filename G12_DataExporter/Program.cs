using DataExport.DataReader;
using DataExport.DataWriter;
using Microsoft.Data.SqlClient;

namespace DataExport;

class Program
{
    static void Main(string[] args)
    {
        using SqlConnection sqlConnection = new("Server=localhost;Database=Northwind;UID=sa;PWD=!;Integrated Security=False; TrustServerCertificate=True");
        using TsvWriter writer = new(@"/Users/irakli/Downloads/Test.txt");
        string viewName = "select * from ProductsExportToCsv_V";
        
        SqlReader reader = new(sqlConnection, writer, viewName, '\t');
        reader.ExportData();
    }
}