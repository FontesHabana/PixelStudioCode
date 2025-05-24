namespace PixelWallE.Language.Parsing.Expressions;

public  class EqualToOperation: BinaryExpression
{

    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public EqualToOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){
        Type=ExpressionType.Bool;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.EqualToOperation(this);
    }

    
}
