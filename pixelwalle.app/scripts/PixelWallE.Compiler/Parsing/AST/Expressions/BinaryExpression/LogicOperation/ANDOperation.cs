namespace PixelWallE.Language.Parsing.Expressions;

public  class ANDOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public ANDOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
         Type=ExpressionType.Bool;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.ANDOperation(this);
    }

   
}
