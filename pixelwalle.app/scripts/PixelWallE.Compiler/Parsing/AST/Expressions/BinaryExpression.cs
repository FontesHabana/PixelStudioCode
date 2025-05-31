namespace PixelWallE.Language.Parsing.Expressions;



/// <summary>
/// Represents the abstract base class for binary expressions in the PixelWallE language AST.
/// A binary expression consists of two operands: a left operand (<see cref="Left"/>) and a right operand (<see cref="Right"/>).
/// </summary>
public abstract class BinaryExpression : Expression
{
    /// <summary>
    /// Gets or sets the left operand expression.
    /// </summary>
    public Expression? Left { get; set; }

    /// <summary>
    /// Gets or sets the right operand expression.
    /// </summary>
    public Expression? Right { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryExpression"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this binary expression.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public BinaryExpression(CodeLocation location, Expression left, Expression right) : base(location)
    {
        Left = left;
        Right = right;
    }
}