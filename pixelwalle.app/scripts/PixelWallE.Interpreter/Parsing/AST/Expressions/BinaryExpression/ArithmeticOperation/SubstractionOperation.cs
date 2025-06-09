namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a subtraction operation (e.g., 8 - 3) in the PixelWallE language AST.
/// </summary>
public class SubstractionOperation : BinaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Number for arithmetic operations).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the subtraction operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstractionOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this subtraction operation.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public SubstractionOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right)
    {
        Type = ExpressionType.Number;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the subtraction operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.SubstractionOperation(this);
    }
}
