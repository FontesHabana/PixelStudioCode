namespace PixelWallE.Language.Parsing.Expressions;

public class DivisionOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public DivisionOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
        Type=ExpressionType.Number;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.DivisionOperation(this);
    }

    
  
}
