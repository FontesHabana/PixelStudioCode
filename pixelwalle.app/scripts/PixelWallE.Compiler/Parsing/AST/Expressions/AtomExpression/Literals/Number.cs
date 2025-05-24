namespace PixelWallE.Language.Parsing.Expressions.Literals;

public class Number : AtomExpression
{
    public override ExpressionType Type { 
        get{
            return ExpressionType.Number;
        }
     set{} }
    public override object? Value { get; set; }

    public Number(CodeLocation location, int value) : base(location)
    { 
        Value = value;
    }
    public override void Accept(IVisitor<ASTNode> visitor)
    {
       
    }
}