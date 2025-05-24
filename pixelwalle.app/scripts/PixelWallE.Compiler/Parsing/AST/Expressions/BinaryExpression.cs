

namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
public abstract class BinaryExpression: Expression
{
    public Expression? Left { get; set; }
    public Expression? Right {get; set;}
    

    public BinaryExpression(CodeLocation location , Expression left, Expression right): base (location){
        Left=left;
        Right=right;
    }
    
}