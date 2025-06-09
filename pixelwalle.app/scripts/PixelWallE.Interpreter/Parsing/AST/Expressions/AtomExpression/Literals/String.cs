namespace PixelWallE.Language.Parsing.Expressions.Literals;

/// <summary>
/// Represents a string literal in the PixelWallE language AST.
/// </summary>
public class StringLiteral : AtomExpression
{
    /// <summary>
    /// Gets the expression type (always String for string literals).
    /// </summary>
    public override ExpressionType Type
    {
        get { return ExpressionType.String; }
        set { } // Setting the type is not supported for literals.
    }

    /// <summary>
    /// Gets or sets the string value of the literal.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLiteral"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this string literal.</param>
    /// <param name="value">The string value.</param>
    public StringLiteral(CodeLocation location, string value) : base(location)
    {
        Value = value;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the string literal node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        // No specific visitor method for String literals.
    }
}