namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents an "equal to" comparison operation (e.g., 5 == 5) in the PixelWallE language AST.
/// </summary>
public class EqualToOperation : BinaryExpression
{
    /// <summary>
    /// Gets or sets the expression type (always Bool for comparison operations).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the comparison operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualToOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this operation.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public EqualToOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right)
    {
        Type = ExpressionType.IntegerOrBool;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the "equal to" operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.EqualToOperation(this);
    }
}
