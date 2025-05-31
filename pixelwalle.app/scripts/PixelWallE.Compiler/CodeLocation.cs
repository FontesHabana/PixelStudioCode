namespace PixelWallE.Language;
/// <summary>
/// Represents a location in the source code.
/// </summary>
public struct CodeLocation
{
    /// <summary>
    /// The file name.
    /// </summary>
    public string File;
    /// <summary>
    /// The line number.
    /// </summary>
    public int Line{get; set;}
    /// <summary>
    /// The column number.
    /// </summary>
    public int? Column;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeLocation"/> struct.
    /// </summary>
    /// <param name="file">The file name.</param>
    /// <param name="line">The line number.</param>
    /// <param name="column">The column number.</param>
    public CodeLocation(string file, int line, int column){
        File=file;
        Line=line;
        Column=column;
    }
}