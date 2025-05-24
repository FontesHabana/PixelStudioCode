namespace PixelWallE.Language.Parsing.Expressions.Literals;
public class Bool : AtomExpression
{
    public override ExpressionType Type { 
        get{
            return ExpressionType.Bool;
        }
        set{}
    }
    public override object? Value{get; set;}

    public Bool(CodeLocation location, bool value) : base(location)
    { 
        Value = value;
    }

  
    public override void Accept(IVisitor<ASTNode> visitor)
    {
       
    }

}