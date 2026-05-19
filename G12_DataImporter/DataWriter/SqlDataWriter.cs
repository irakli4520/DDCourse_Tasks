using G12_DataImporter.Exceptions;
using G12_DataImporter.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace G12_DataImporter.DataWriter;

public sealed class SqlDataWriter : Interfaces.IDataWriter
{
    private readonly SqlConnection _connection;
    private readonly Interfaces.IDataReader _dataReader;
    private readonly Action<DataImportException>? _logException;
    private readonly List<DataImportException> _exceptions;
    private bool _leaveOpen = true;

    public SqlDataWriter(
        SqlConnection connection,
        Interfaces.IDataReader dataReader,
        Action<DataImportException>? logException = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        _logException = logException;
        _exceptions = new List<DataImportException>();
    }

    public void WriteData()
    {
        IEnumerable<Category> categories = _dataReader.GetData();
        using SqlCommand command = CreateCommand();

        try
        {
            HandleConnectionOpen();

            foreach (var category in categories)
            {
                ProcessCategoryProducts(category, command);
            }

            if (_exceptions.Any())
                throw new AggregateException("One or more errors occurred during data import.", _exceptions);
        }
        finally
        {
            if (!_leaveOpen && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }

    private void HandleConnectionOpen()
    {
        if (_connection.State == ConnectionState.Open) 
            return;
        _connection.Open();
        _leaveOpen = false;
    }

    private void ProcessCategoryProducts(Category category, SqlCommand command)
    {
        foreach (var product in category.Products)
        {
            try
            {
                ProcessData(command, category, product);
            }
            catch (Exception ex)
            {
                if (_logException != null)
                    _logException.Invoke(new DataImportException($"Error processing product '{product.Code}' in category '{category.Name}'.", ex));
                else
                    _exceptions.Add(new DataImportException($"Error processing product '{product.Code}' in category '{category.Name}'.", ex));
            }
        }
    }

    private SqlCommand CreateCommand()
    {
        SqlCommand command = _connection.CreateCommand();
        command.CommandText = "InsertProduct_sp";
        command.CommandType = CommandType.StoredProcedure;
        SetupParameters(command);
        return command;
    }

    private void ProcessData(SqlCommand command, Category category, Product product)
    {
        AssignParameters(command, category, product);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            var importException = new DataImportException("Error occurred while processing data", ex);
            if (_logException != null)
                _logException.Invoke(importException);
            else
                _exceptions.Add(importException);
        }
    }

    private static void AssignParameters(SqlCommand command, Category category, Product product)
    {
        command.Parameters["@CategoryName"].Value = category.Name;
        command.Parameters["@CategoryIsDeleted"].Value = !category.IsActive;
        command.Parameters["@ProductCode"].Value = product.Code;
        command.Parameters["@ProductName"].Value = product.Name;
        command.Parameters["@ProductPrice"].Value = product.Price;
        command.Parameters["@ProductQuantity"].Value = product.Quantity;
        command.Parameters["@ProductIsDeleted"].Value = !product.IsActive;
    }

    private static void SetupParameters(SqlCommand command)
    {
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar);
        command.Parameters.Add("@CategoryIsDeleted", SqlDbType.Bit);
        command.Parameters.Add("@ProductCode", SqlDbType.NVarChar);
        command.Parameters.Add("@ProductName", SqlDbType.NVarChar);
        command.Parameters.Add("@ProductPrice", SqlDbType.Decimal);
        command.Parameters.Add("@ProductQuantity", SqlDbType.Int);
        command.Parameters.Add("@ProductIsDeleted", SqlDbType.Bit);
    }
}