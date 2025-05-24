
namespace PixelWallE.Language.Parsing.Expressions
;

public class ParenthesizedExpression : AtomExpression
{
    public Expression InnerExpression { get; set; }
    public override ExpressionType Type{get;set;}

    public override object? Value { get; set; }

    public ParenthesizedExpression(CodeLocation location, Expression innerExpression) : base(location)
    {
        InnerExpression = innerExpression;
    }

    
    

    public override void Accept(IVisitor<ASTNode> visitor){
        visitor.ParenthesizedExpression(this);
    }



}