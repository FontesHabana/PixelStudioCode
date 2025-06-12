namespace PixelWallE.Language.Parsing.Expressions.Literals;

/// <summary>
/// Represents a numeric literal (integer) in the PixelWallE language AST.
/// </summary>
public class Number : AtomExpression
{
    /// <summary>
    /// Gets the expression type (always Number for numeric literals).
    /// </summary>
    public override ExpressionType Type
    {
        get { return ExpressionType.IntegerOrBool; }
        set { } // Setting the type is not supported for literals.
    }

    /// <summary>
    /// Gets or sets the numeric value of the literal.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Number"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this numeric literal.</param>
    /// <param name="value">The integer value.</param>
    public Number(CodeLocation location, IntegerOrBool value) : base(location)
    {
        Value = value;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the number literal node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        // No specific visitor method for Number literals.
    }
}