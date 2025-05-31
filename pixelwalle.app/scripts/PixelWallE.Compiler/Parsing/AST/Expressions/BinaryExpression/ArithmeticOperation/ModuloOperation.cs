namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a modulo operation (e.g., 10 % 3) in the PixelWallE language AST.
/// </summary>
public class ModuloOperation : BinaryExpression
{
    /// <summary>
    /// Gets or sets the expression type.
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the modulo operation.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuloOperation"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this modulo operation.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public ModuloOperation(CodeLocation location, Expression left, Expression right) : base(location, left, right)
    {
        Type = ExpressionType.Number; // Assuming Modulo operation results in a number
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the modulo operation node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ModuloOperation(this);
    }
}
