namespace PixelWallE.Language.Parsing.Expressions;

public  class GreatherThanOrEqualToOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public GreatherThanOrEqualToOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
         Type=ExpressionType.Bool;
    }
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.GreatherThanOrEqualToOperation(this);
    }

}