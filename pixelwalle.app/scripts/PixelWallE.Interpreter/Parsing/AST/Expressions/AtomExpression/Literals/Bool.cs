namespace PixelWallE.Language.Parsing.Expressions.Literals;

/// <summary>
/// Represents a boolean literal (true or false) in the PixelWallE language AST.
/// </summary>
public class Bool : AtomExpression
{
    /// <summary>
    /// Gets the expression type (always Bool for boolean literals).
    /// </summary>
    public override ExpressionType Type
    {
        get { return ExpressionType.Bool; }
        set { } // Setting the type is not supported for literals.
    }

    /// <summary>
    /// Gets or sets the boolean value of the literal.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this boolean literal.</param>
    /// <param name="value">The boolean value (true or false).</param>
    public Bool(CodeLocation location, bool value) : base(location)
    {
        Value = value;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the boolean literal node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        // No specific visitor method for Bool literals.
    }
}