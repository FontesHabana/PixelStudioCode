namespace PixelWallE.Language.Parsing.Expressions.Literals;

public class Bool : Expression
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

    public override bool CheckSemantic(Context context, Scope table, List<CompilingError> errors)
    {
        return true;
    }

    public override void Evaluate()
    {
        // No evaluation needed for literals
    }

    public override string ToString()
    {
        return String.Format("{0}",Value);
    }
}