using DatabaseHelper.Core;
using Microsoft.Data.SqlClient;

namespace DatabaseHelper.MsSql;

public sealed class Database : CommonDatabase<SqlConnection, SqlCommand, SqlTransaction, SqlDataReader, SqlParameter>
{
    public Database(string connectionString) : base(connectionString)
    {
    }

    public Database(Func<SqlConnection> connectionFactory) : base(connectionFactory)
    {
    }
}