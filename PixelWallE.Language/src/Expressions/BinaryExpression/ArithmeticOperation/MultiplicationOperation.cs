namespace PixelWallE.Language.Parsing.Expressions;

public abstract class MoltiplicationOperation: BinaryExpression
{
    public override ExpressionType Type {get; set;}
    public override object? Value {get; set;}

    public MoltiplicationOperation(CodeLocation location) : base(location){}

    public override void Evaluate()
    {
        Right.Evaluate();
        Left.Evaluate();
        //Revisar ma¡ás adelante división por cero
        Value = (int)Right.Value*(int)Left.Value;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool right = Right.CheckSemantic(context, scope, errors);
        bool left = Left.CheckSemantic(context, scope, errors);
        if (Right.Type != ExpressionType.Number || Left.Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Este texto de error hay que cambiarlo We don't do that here... "));
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = ExpressionType.Number;
        return right && left;
    }

    public override string ToString()
    {
        if (Value == null)
        {
            return String.Format("({0} + {1})", Left, Right);
        }
        return Value.ToString();
    }
}
