using DatabaseHelper.MsSql;
using Microsoft.Data.SqlClient;

namespace DatabaseHelper.App;

internal class Program
{
    static void Main(string[] args)
    {
        var db = new Database(() => new SqlConnection("Server=localhost;Database=Northwind;UID=sa;PWD=!;Integrated Security=False; TrustServerCertificate=True"));

        using var database = new Database("Server=localhost;Database=Northwind;UID=sa;PWD=!;Integrated Security=False; TrustServerCertificate=True");
        using var connection = database.GetConnection();
        database.OpenConnection();
        database.BeginTransaction();
        try
        {
            using (var reader = database.ExecuteReader("select CategoryID, CategoryName from Categories"))
            {
                while (reader.Read())
                {
                    int categoryId = reader.GetInt32(0);
                    string categoryName = reader.GetString(1);
                    Console.WriteLine($"{categoryId} {categoryName}");
                }
            }

            database.CommitTransaction();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            database.RollbackTransaction();
            throw;
        }
    }
}