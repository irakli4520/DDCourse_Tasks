using System.Data;
using DatabaseHelper.MsSql;
using Microsoft.Data.SqlClient;

namespace DatabaseHelperTests;

[TestFixture]
public class MsSqlDatabaseTests
{
    private string _connectionString;
    private Database _database;
    private Database _tempDatabase;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _tempDatabase = new Database("Server=localhost;Database=master;UID=sa;PWD=!;Integrated Security=False; TrustServerCertificate=True");
        _tempDatabase.OpenConnection();
        _tempDatabase.ExecuteNonQuery("create database TemporaryDb;");
        _tempDatabase.ExecuteNonQuery("use TemporaryDb;");
        _tempDatabase.ExecuteNonQuery(
            "create table Persons( ID int primary key identity (1, 1), Name nvarchar(100) unique)");
        _tempDatabase.CloseConnection();
    }

    [SetUp]
    public void Setup()
    {
        _connectionString =
            "Server=localhost;Database=TemporaryDb;UID=sa;PWD=!;Integrated Security=False; TrustServerCertificate=True";
        _database = new Database(_connectionString);
    }

    [Test]
    public void Constructor_WithConnectionString_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => new Database(_connectionString));
    }

    [Test]
    public void Constructor_NullConnectionString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Database(connectionString: null!));
    }

    [Test]
    public void GetConnection_WhenCalled_ReturnsClosedConnection()
    {
        var connection = _database.GetConnection();
        Assert.That(connection, Is.Not.Null);
        Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
    }

    [Test]
    public void OpenConnection_WhenCalled_SetsConnectionStateToOpen()
    {
        _database.OpenConnection();
        var connection = _database.GetConnection();
        Assert.That(connection, Is.Not.Null);
        Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
    }

    [Test]
    public void ExecuteScalar_ValidQuery_ReturnsExpectedResult()
    {
        _database.OpenConnection();
        var result = (int)_database.ExecuteScalar("select 1")!;
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void GetCommand_ValidQuery_ReturnsProperlyConfiguredCommand()
    {
        _database.OpenConnection();
        var command = _database.GetCommand("select 1");
        Assert.That(command, Is.Not.Null);
        Assert.That(command.CommandType, Is.EqualTo(CommandType.Text));
        Assert.That(command.CommandText, Is.EqualTo("select 1"));
        Assert.That(command.Connection, Is.Not.Null);
        Assert.That(command.Parameters.Count, Is.EqualTo(0));
    }

    [Test]
    public void ExecuteNonQuery_InsertQuery_ReturnsAffectedRowsCount()
    {
        _database.OpenConnection();

        int rowsAffected = _database.ExecuteNonQuery(
            "INSERT INTO Persons (name) VALUES (@name)",
            new SqlParameter("@name", "Keti"));
        Assert.That(rowsAffected, Is.EqualTo(1));
    }

    [Test]
    public void CommitTransaction_AfterInsert_SavesData()
    {
        _database.OpenConnection();
        _database.BeginTransaction();
        int rowsAffected = _database.ExecuteNonQuery(
            "INSERT INTO Persons (name) VALUES (@name)",
            new SqlParameter("@name", "Nino"));
        Assert.That(rowsAffected, Is.EqualTo(1));
        _database.CommitTransaction();
        var count = _database.ExecuteScalar("select count(*) from Persons where name = 'Nino'");
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void RollbackTransaction_AfterInsert_DoesNotSaveData()
    {
        _database.OpenConnection();
        _database.BeginTransaction();
        int rowsAffected = _database.ExecuteNonQuery(
            "INSERT INTO Persons (name) VALUES (@name)",
            new SqlParameter("@name", "Irakli"));
        Assert.That(rowsAffected, Is.EqualTo(1));

        _database.RollbackTransaction();
        var count = _database.ExecuteScalar("select COUNT(*) from Persons where name = 'Irakli'");
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void ExecuteReader_ValidQuery_ReturnsExpectedData()
    {
        _database.OpenConnection();
        int rowsAffected = _database.ExecuteNonQuery(
            "INSERT INTO Persons (name) VALUES (@name)",
            new SqlParameter("@name", "Tamo"));
        Assert.That(rowsAffected, Is.EqualTo(1));

        var reader = _database.ExecuteReader("select Name from Persons where name = 'Tamo'");
        string name = "";
        while (reader.Read())
        {
            name = reader.GetString(0);
        }

        Assert.That(name, Is.EqualTo("Tamo"));
    }

    [Test]
    public void BeginTransaction_WhenTransactionAlreadyExists_ThrowsInvalidOperationException()
    {
        _database.OpenConnection();
        _database.BeginTransaction();
        Assert.Throws<InvalidOperationException>(() => _database.BeginTransaction());
    }

    [Test]
    public void CommitOrRollback_WhenNoTransactionExists_ThrowsInvalidOperationException()
    {
        _database.OpenConnection();
        Assert.Throws<InvalidOperationException>(() => _database.CommitTransaction());
        Assert.Throws<InvalidOperationException>(() => _database.RollbackTransaction());
    }

    [Test]
    public void Methods_WhenDatabaseDisposed_ThrowObjectDisposedException()
    {
        _database.OpenConnection();
        _database.Dispose();
        Assert.Throws<ObjectDisposedException>(() => _database.GetConnection());
    }

    [TearDown]
    public void TearDown()
    {
        _database.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _tempDatabase.OpenConnection();
        _tempDatabase.ExecuteNonQuery("use master;");
        _tempDatabase.ExecuteNonQuery("alter database TemporaryDb set SINGLE_USER with rollback immediate;");
        _tempDatabase.ExecuteNonQuery("drop database TemporaryDb;");
        _tempDatabase.CloseConnection();
        _tempDatabase.Dispose();
    }
}