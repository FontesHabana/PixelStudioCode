namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a negation operation (e.g., -5) in the PixelWallE language AST.
/// </summary>
public class NegationOperation : UnaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Number for negation).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the negation operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NegationOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this negation operation.</param>
    /// <param name="right">The expression to negate.</param>
    public NegationOperation(CodeLocation location, Expression right) : base(location, right)
    {
        Type = ExpressionType.Number;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the negation operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.NegationOperation(this);
    }
}