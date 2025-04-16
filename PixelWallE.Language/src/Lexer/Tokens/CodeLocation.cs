
namespace PixelWallE.Language.Tokens;
public struct CodeLocation
{
    public string File;
    public int Line;
    public int? Column;

    public CodeLocation(string file, int line, int column){
        File=file;
        Line=line;
        Column=column;
    }
}