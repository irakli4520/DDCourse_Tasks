using DatabaseHelper.Core;
using MySql.Data.MySqlClient;

namespace DatabaseHelper.MySql;

public sealed class Database : CommonDatabase<MySqlConnection, MySqlCommand, MySqlTransaction,MySqlDataReader, MySqlParameter>
{
    public Database(string connectionString) : base(connectionString)
    {
    }

    public Database(Func<MySqlConnection> connectionFactory) : base(connectionFactory)
    {
    }
}