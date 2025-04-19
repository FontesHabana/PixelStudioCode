namespace PixelWallE.Language.Parsing.Expressions.Literals;

public class Number : Expression
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