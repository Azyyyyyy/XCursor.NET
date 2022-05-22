namespace XCursor.NET;

public class XCursor
{
    /// <summary>
    /// Cursor Width
    /// </summary>
    public uint Width { get; internal set; }

    /// <summary>
    /// Cursor Height
    /// </summary>
    public uint Height { get; internal set; }
    
    /// <summary>
    /// ???
    /// </summary>
    public uint XHot { get; internal set; }

    /// <summary>
    /// ???
    /// </summary>
    public uint YHot { get; internal set; }
    
    /// <summary>
    /// Delay between animation frames in milliseconds
    /// </summary>
    public uint Delay { get; internal set; }
    
    /// <summary>
    /// Packed ARGB format pixels
    /// </summary>
    public byte[] Pixels { get; internal set; }
}