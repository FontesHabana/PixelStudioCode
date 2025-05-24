namespace PixelWallE.Language.Parsing.Expressions;

public class NegationOperation : UnaryExpression
{   
   
    public override ExpressionType Type { get; set;  }
    public override object? Value{get; set;}

    public NegationOperation(CodeLocation location ,Expression right) : base(location, right)
    {
      Type=ExpressionType.Number;
    }

    

    public override void Accept(IVisitor<ASTNode> visitor)
    {
      visitor.NegationOperation(this);
    }

    
}