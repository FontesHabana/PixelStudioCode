namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a logical AND operation (e.g., true && false) in the PixelWallE language AST.
/// </summary>
public class ANDOperation : BinaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Bool for logical operations).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the logical AND operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ANDOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this operation.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public ANDOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right)
    {
        Type = ExpressionType.Bool;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the logical AND operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ANDOperation(this);
    }
}
