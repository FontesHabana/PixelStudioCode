namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a parenthesized expression in the PixelWallE language AST.
/// This allows grouping expressions to control precedence.
/// </summary>
public class ParenthesizedExpression : AtomExpression
{
    /// <summary>
    /// Gets or sets the inner expression enclosed in parentheses.
    /// </summary>
    public Expression InnerExpression { get; set; }

    /// <summary>
    /// Gets or sets the expression type of the parenthesized expression.
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the parenthesized expression.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParenthesizedExpression"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this parenthesized expression.</param>
    /// <param name="innerExpression">The expression enclosed in parentheses.</param>
    public ParenthesizedExpression(CodeLocation location, Expression innerExpression) : base(location)
    {
        InnerExpression = innerExpression;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the parenthesized expression node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ParenthesizedExpression(this);
    }
}