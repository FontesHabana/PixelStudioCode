namespace PixelWallE.Language.Parsing.Expressions;

public  class SubstractionOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public SubstractionOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
        Type=ExpressionType.Number;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.SubstractionOperation(this);
    }

    
}
