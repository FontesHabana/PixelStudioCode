namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a division operation (e.g., 10 / 2) in the PixelWallE language AST.
/// </summary>
public class DivisionOperation : BinaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Number for arithmetic operations).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the division operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DivisionOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this division operation.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public DivisionOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right)
    {
        Type = ExpressionType.Number;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the division operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.DivisionOperation(this);
    }
}
