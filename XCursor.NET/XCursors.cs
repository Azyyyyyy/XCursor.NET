namespace XCursor.NET;

public class XCursors
{
    /// <summary>
    /// File version number
    /// </summary>
    public uint Version { get; internal set; }

    /// <summary>
    /// All the cursors contained in the file/stream
    /// </summary>
    public XCursor[] Cursors { get; internal set; }
    
    /// <summary>
    /// All the comments contained in the file/stream
    /// </summary>
    public string[] Comments { get; internal set; }
}