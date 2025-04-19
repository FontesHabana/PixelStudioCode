
namespace PixelWallE.Language.Parsing.Expressions
;

public class ParenthesizedExpression : Expression
{
    public Expression InnerExpression { get; set; }
    public override ExpressionType Type{get;set;}

    public override object? Value { get; set; }

    public ParenthesizedExpression(CodeLocation location, Expression innerExpression) : base(location)
    {
        InnerExpression = innerExpression;
    }
    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool check=InnerExpression.CheckSemantic(context,scope,errors);
        Type= InnerExpression.Type;
        return check;
    }

    public override void Evaluate()
    {
        InnerExpression.Evaluate();
        Value=InnerExpression.Value;
       
    }



}