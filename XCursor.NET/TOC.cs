namespace XCursor.NET;

internal class TOC
{
    /// <summary>
    /// Entry type
    /// </summary>
    public Type Type { get; internal set; }
    
    /// <summary>
    /// Type-specific label - size for images
    /// </summary>
    public uint SubType { get; internal set; }
    
    /// <summary>
    /// Absolute byte position of table in file
    /// </summary>
    public uint Position { get; internal set; }
}