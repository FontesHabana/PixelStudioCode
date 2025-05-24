
namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

public abstract class UnaryExpression : Expression
{   public Expression? Right{get; set;}
    public UnaryExpression(CodeLocation location,Expression right): base(location){
        Right=right;
    }
}
//ok