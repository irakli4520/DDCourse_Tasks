using System.Data.Common;
using DataExport.Interfaces;

namespace DataExport.DataReader;

public sealed class SqlReader : IDataReader
{
    private readonly DbConnection _connection;
    private readonly IDataWriter _writer;
    private readonly string _viewName;
    private readonly char _separator;
    
    public SqlReader(DbConnection connection, IDataWriter writer, string viewName,char separator)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _viewName = viewName ?? throw new ArgumentNullException(nameof(viewName));
        _separator = separator;
    }

    public void ExportData()
    {
        using DbCommand command = CreateCommand();
        _connection.Open();
        try
        {
            using DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // string categoryName = reader.GetString(0);
                // int categoryIsActive = reader.GetInt32(1);
                // string productCode = Convert.ToString(reader.GetInt32(2));
                // string productName = reader.GetString(3);
                // decimal productPrice = Math.Round((decimal)reader["ProductPrice"], 2);
                // int productQuantity = reader.GetInt16(5);
                // int productIsActive = reader.GetInt32(6);
                // string line = $"{categoryName}\t{categoryIsActive}\t{productCode}\t{productName}\t{productPrice}\t{productQuantity}\t{productIsActive}";
                
                string[] values = new string[reader.FieldCount];
                
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = reader.GetValue(i).ToString()!;
                }
                string line = string.Join(_separator, values);
                
                _writer.WriteData(line);
            }
        }
        finally 
        {
            _connection.Close();
        }
    }
    
    private DbCommand CreateCommand()
    {
        DbCommand command = _connection.CreateCommand();
        command.CommandText = _viewName;    
        return command;
    }
}