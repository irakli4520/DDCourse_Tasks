using DataExport.Interfaces;
namespace DataExport.DataWriter;

public sealed class TsvWriter : IDataWriter
{
    private readonly StreamWriter _writer;

    public TsvWriter(string path) : this(new FileInfo(path))
    {
        
    }
    
    public TsvWriter(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        if (!fileInfo.Exists)
            fileInfo.Create().Dispose();
        if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            throw new ArgumentException("The provided path is a directory, not a file.", nameof(fileInfo));
        
        
        _writer = new StreamWriter(fileInfo.OpenWrite());
    }

    public TsvWriter(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanWrite)
            throw new ArgumentException("The provided stream cannot be read.", nameof(stream));
        if (stream.CanSeek && stream.Position != 0)
            stream.Seek(0, SeekOrigin.Begin);
        _writer = new StreamWriter(stream, leaveOpen:true);
    }
    
    public void WriteData(string line)
    {
        _writer.WriteLine(line);
    }
    
    public void Dispose()
    {
        _writer.Dispose();
    }
}