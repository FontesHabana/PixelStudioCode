namespace PixelWallE.Language.Parsing.Expressions;

public  class LessThanOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public LessThanOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
         Type=ExpressionType.Bool;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.LessThanOperation(this);
    }

}