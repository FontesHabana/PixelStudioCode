namespace PixelWallE.Language.Parsing.Expressions;

public  class ModuloOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public ModuloOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right){}

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.ModuloOperation(this);
    }
}
