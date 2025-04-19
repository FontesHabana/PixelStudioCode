using System.Linq.Expressions;

namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
public abstract class BinaryExpression: Expression
{
    public Expression? Left { get; set; }
    public Expression? Right {get; set;}


    public BinaryExpression(CodeLocation location): base (location){}
    
}