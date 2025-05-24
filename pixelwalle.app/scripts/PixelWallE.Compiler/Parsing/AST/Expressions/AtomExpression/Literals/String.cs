namespace PixelWallE.Language.Parsing.Expressions.Literals;

public class StringLiteral : AtomExpression
{
    public override ExpressionType Type { 
        get{
            return ExpressionType.String;
        }
        set{}
    }
    public override object? Value{get; set;}

    public StringLiteral(CodeLocation location, string value) : base(location)
    { 
        Value = value;
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       
    }
}