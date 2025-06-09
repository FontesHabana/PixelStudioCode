namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a logical NOT operation (e.g., !true) in the PixelWallE language AST.
/// </summary>
public class NotOperation : UnaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Bool for logical NOT).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the logical NOT operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this NOT operation.</param>
    /// <param name="right">The expression to negate logically.</param>
    public NotOperation(CodeLocation location, Expression right) : base(location, right)
    {
        Type = ExpressionType.Bool;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the logical NOT operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.NotOperation(this);
    }
}