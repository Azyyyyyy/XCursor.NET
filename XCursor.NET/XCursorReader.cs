using System;
using System.IO;
using System.Linq;
using System.Text;

namespace XCursor.NET;

//Used https://www.x.org/releases/X11R7.7/doc/man/man3/Xcursor.3.xhtml to get all the info needed for reading
public class XCursorReader
{
    private static readonly Exception BrokenFileException = new Exception("Broken XCursor file");
    
    public static XCursors GetCursors(string file) => GetCursors(File.OpenRead(file));
    public static XCursors GetCursors(Stream stream)
    {
        //Check that we can use the stream
        if (!stream.CanSeek)
        {
            throw new Exception("Provided stream needs to be seekable!!");
        }
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new BinaryReader(stream);

        byte[] magicWord = { 0x58, 0x63, 0x75, 0x72 }; //’Xcur’ (0x58, 0x63, 0x75, 0x72)

        //Check that the magic word exists
        var returnedMagicWord = new byte[4];
        if (stream.Read(returnedMagicWord, 0, 4) != 4
            || !returnedMagicWord.SequenceEqual(magicWord))
        {
            throw new Exception("Invalid magic word");
        }

        //Get header data
        uint byteCount = reader.ReadUInt32(); //bytes in this header
        uint version = reader.ReadUInt32(); //file version number
        uint ntoc = reader.ReadUInt32(); //number of toc entries
        stream.Seek(byteCount, SeekOrigin.Begin);

        //Get the toc's
        var tocs = new TOC[ntoc];
        for (int i = 0; i < ntoc; i++)
        {
            tocs[i] = ReadTOC(reader);
        }

        var comments = new string[tocs.Count(x => x.Type == Type.Comment)];
        var cursorImages = new XCursor[tocs.Count(x => x.Type == Type.Image)];
        var commentCount = 0;
        var cursorImagesCount = 0;
        
        //Grab data
        for (int i = 0; i < ntoc; i++)
        {
            var toc = tocs[i];
            reader.BaseStream.Seek(toc.Position, SeekOrigin.Begin);

            //We don't use the header but we need to read the contents and do checks
            var header = GetHeader(reader, toc);
            switch (toc.Type)
            {
                case Type.Comment:
                    comments[commentCount] = GetComment(reader, toc);
                    commentCount++;
                    break;
                case Type.Image:
                    cursorImages[cursorImagesCount] = GetCursor(reader, toc);
                    cursorImagesCount++;
                    break;
            }
        }
        
        var cursors = new XCursors
        {
            Version = version,
            Comments = comments,
            Cursors = cursorImages
        };
        reader.Dispose();
        return cursors;
    }

    private static HeaderData GetHeader(BinaryReader reader, TOC toc)
    {
        var header = reader.ReadUInt32(); //bytes in chunk header (including type-specific fields)
        var type = (Type)reader.ReadUInt32(); //must match type in TOC for this chunk 
        var subtype = reader.ReadUInt32(); //must match subtype in TOC for this chunk 
        var version = reader.ReadUInt32(); //version number for this chunk type
        
        if (type != toc.Type || subtype != toc.SubType)
        {
            throw BrokenFileException;
        }

        return new HeaderData
        {
            Header = header,
            Version = version
        };
    }

    private static XCursor GetCursor(BinaryReader reader, TOC toc)
    {
        /*header: 36 Image headers are 36 bytes
        type: 0xfffd0002 Image type is 0xfffd0002
        subtype: CARD32 Image subtype is the nominal size
        version: 1*/

        var width = reader.ReadUInt32(); //Must be less than or equal to 0x7fff
        var height = reader.ReadUInt32(); //Must be less than or equal to 0x7fff
        var xhot = reader.ReadUInt32(); //Must be less than or equal to width
        var yhot = reader.ReadUInt32(); //Must be less than or equal to height
        var delay = reader.ReadUInt32(); //Delay between animation frames in milliseconds
        var pixels = reader.ReadBytes((int)((width * height) * sizeof(uint))); //Packed ARGB format pixels

        if (width > 0x7fff || height > 0x7fff || xhot > width || yhot > height)
        {
            throw BrokenFileException;
        }
        
        return new XCursor
        {
            Width = width,
            Height = height,
            Delay = delay,
            Pixels = pixels,
            XHot = xhot,
            YHot = yhot
        };
    }

    private static string GetComment(BinaryReader reader, TOC toc)
    {
        /*header: 20 Comment headers are 20 bytes
        type: 0xfffe0001 Comment type is 0xfffe0001
        subtype: { 1 (COPYRIGHT), 2 (LICENSE), 3 (OTHER) }
        version: 1*/

        var length = (int)reader.ReadUInt32(); //byte length of UTF-8 string
        return Encoding.UTF8.GetString(reader.ReadBytes(length)); //UTF-8 string
    }
    
    private static TOC ReadTOC(BinaryReader reader)
    {
        var type = (Type)reader.ReadUInt32(); //entry type
        var subtype = reader.ReadUInt32(); //type-specific label - size for images
        var position = reader.ReadUInt32(); //absolute byte position of table in file

        return new TOC
        {
            Type = type,
            SubType = subtype,
            Position = position
        };
    }
}