namespace PixelWallE.Language.Parsing.Expressions;

public  class MultiplicationOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public MultiplicationOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
        Type=ExpressionType.Number;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.MultiplicationOperation(this);
    }


}
