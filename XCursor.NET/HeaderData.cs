namespace XCursor.NET;

public class HeaderData
{
    /// <summary>
    /// Bytes in chunk header (including type-specific fields)
    /// </summary>
    public uint Header { get; internal set; }
    
    /// <summary>
    /// Version number for this chunk type
    /// </summary>
    public uint Version { get; internal set; }
}