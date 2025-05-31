namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

/// <summary>
/// Represents the abstract base class for unary expressions in the PixelWallE language AST.
/// A unary expression consists of a single operand (the <see cref="Right"/> expression).
/// </summary>
public abstract class UnaryExpression : Expression
{
    /// <summary>
    /// Gets or sets the operand expression for the unary operation.
    /// </summary>
    public Expression? Right { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnaryExpression"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this unary expression.</param>
    /// <param name="right">The operand expression.</param>
    public UnaryExpression(CodeLocation location, Expression right) : base(location)
    {
        Right = right;
    }
}