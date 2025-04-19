
namespace PixelWallE.Language;
public struct CodeLocation
{
    public string File;
    public int Line{get; set;}
    public int? Column;

    public CodeLocation(string file, int line, int column){
        File=file;
        Line=line;
        Column=column;
    }
}