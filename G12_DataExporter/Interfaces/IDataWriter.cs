namespace DataExport.Interfaces;

public interface IDataWriter : IDisposable
{
    void WriteData(string line);
}