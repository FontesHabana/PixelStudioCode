namespace PixelWallE.Language.Parsing.Expressions;

public class NotOperation : UnaryExpression
{   
    
    public override ExpressionType Type { get; set;  }
    public override object? Value{get; set;}
    
    public NotOperation(CodeLocation location, Expression right) : base(location,right)
    {
       Type=ExpressionType.Bool;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.NotOperation(this);
    }

}