namespace PixelWallE.Language.Parsing.Expressions;

public  class OrOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public OrOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
       Type=ExpressionType.Bool;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
      visitor.OrOperation(this);
    }

  

    
}
