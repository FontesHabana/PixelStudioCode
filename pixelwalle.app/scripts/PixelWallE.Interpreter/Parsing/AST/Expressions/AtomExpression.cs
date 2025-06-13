namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

/// <summary>
/// Represents the abstract base class for atomic expressions (literals, variables, etc.).
/// </summary>
public abstract class AtomExpression : Expression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtomExpression"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this atomic expression.</param>
    public AtomExpression(CodeLocation location) : base(location) { }
}
