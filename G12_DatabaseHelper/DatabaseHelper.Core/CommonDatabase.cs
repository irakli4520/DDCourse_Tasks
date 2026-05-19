using System.Data;
using System.Runtime.CompilerServices;

namespace DatabaseHelper.Core;

public abstract class CommonDatabase<TConnection, TCommand, TTransaction, TDataReader, TParameter> : IDisposable
    where TConnection : class, IDbConnection, new()
    where TCommand : class, IDbCommand
    where TTransaction : class, IDbTransaction
    where TDataReader : class, IDataReader
    where TParameter : class, IDbDataParameter
{
    private TConnection? _connection;
    private TTransaction? _transaction;
    private string _connectionString;
    private readonly Func<TConnection>? _connectionFactory;
    private bool _disposed = false;

    protected CommonDatabase(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    protected CommonDatabase(Func<TConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public TConnection GetConnection()
    {
        ThrowIfDisposed();
        if (_connection != null)
            return _connection;

        if (_connectionFactory == null)
        {
            _connection = new TConnection();
            _connection.ConnectionString = _connectionString;
        }
        else
        {
            _connection = _connectionFactory();
            _connectionString = _connection.ConnectionString;
        }
        return _connection;
    }

    public void OpenConnection()
    {
        ThrowIfDisposed();
        TConnection connection = GetConnection();
        if (connection.State != ConnectionState.Open)
            connection.Open();
    }

    public void CloseConnection()
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new InvalidOperationException(
                "Cannot close connection while a transaction is in progress. Commit or rollback the transaction first.");

        if (_connection?.State != ConnectionState.Closed)
            _connection?.Close();
    }

    public TCommand GetCommand(string commandText, CommandType commandType, params TParameter[] parameters)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(commandText, nameof(commandText));

        var command = (TCommand)GetConnection().CreateCommand();
        command.CommandText = commandText;
        command.CommandType = commandType;
        foreach (var parameter in parameters)
            command.Parameters.Add(parameter);
        if (_transaction != null)
            command.Transaction = _transaction;

        return command;
    }

    public TCommand GetCommand(string commandText, params TParameter[] parameters)
        => GetCommand(commandText, CommandType.Text, parameters);

    public int ExecuteNonQuery(string commandText, CommandType commandType, params TParameter[] parameters)
    {
        using TCommand command = GetCommand(commandText, commandType, parameters);
        return command.ExecuteNonQuery();
    }

    public int ExecuteNonQuery(string commandText, params TParameter[] parameters)
        => ExecuteNonQuery(commandText, CommandType.Text, parameters);

    public object? ExecuteScalar(string commandText, CommandType commandType, params TParameter[] parameters)
    {
        using TCommand command = GetCommand(commandText, commandType, parameters);
        return command.ExecuteScalar();
    }

    public object? ExecuteScalar(string commandText, params TParameter[] parameters)
        => ExecuteScalar(commandText, CommandType.Text, parameters);

    public TDataReader ExecuteReader(string commandText, CommandType commandType, params TParameter[] parameters)
    {
        TCommand command = GetCommand(commandText, commandType, parameters);
        return (TDataReader)command.ExecuteReader();
    }

    public TDataReader ExecuteReader(string commandText, params TParameter[] parameters)
        => ExecuteReader(commandText, CommandType.Text, parameters);

    public void BeginTransaction()
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new InvalidOperationException(
                "Transaction is already started. Commit or rollback the current transaction before starting a new one.");
        _transaction = (TTransaction)GetConnection().BeginTransaction();
    }

    public void CommitTransaction()
    {
        ThrowIfDisposed();
        HandleTransaction(() => _transaction!.Commit());
    }

    public void RollbackTransaction()
    {
        ThrowIfDisposed();
        HandleTransaction(() => _transaction!.Rollback());
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection?.Dispose();
            _connection = null;
        }

        _disposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleTransaction(Action action)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction is in progress.");

        try
        {
            action();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(CommonDatabase<TConnection, TCommand, TTransaction, TDataReader, TParameter>));
    }

    ~CommonDatabase()
    {
        Dispose(false);
    }
}