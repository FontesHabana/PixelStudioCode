namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents an exponentiation operation (e.g., 2 ** 3) in the PixelWallE language AST.
/// </summary>
public class ExponentiationOperation : BinaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Number for arithmetic operations).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the exponentiation operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentiationOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this exponentiation operation.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public ExponentiationOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right)
    {
        Type = ExpressionType.IntegerOrBool;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the exponentiation operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ExponentiationOperation(this);
    }
}
