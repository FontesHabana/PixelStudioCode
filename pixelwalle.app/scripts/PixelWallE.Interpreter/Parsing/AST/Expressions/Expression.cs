namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

/// <summary>
/// Represents the abstract base class for all expressions in the PixelWallE language AST.
/// Expressions have a type, a value, and must implement the visitor pattern.
/// </summary>
public abstract class Expression : ASTNode
{
    /// <summary>
    /// Gets or sets the type of the expression (e.g., Number, String, Bool).
    /// </summary>
    public abstract ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the expression.
    /// </summary>
    public abstract object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Expression"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this expression.</param>
    public Expression(CodeLocation location) : base(location) { }

    /// <summary>
    /// Accepts a visitor for traversing or processing the expression node.
    /// </summary>
    /// <param name="node">The visitor instance.</param>
    public override abstract void Accept(IVisitor<ASTNode> node);
}