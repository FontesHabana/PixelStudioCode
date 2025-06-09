using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

namespace PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents a variable reference in the PixelWallE language AST.
/// </summary>
public class Variable : AtomExpression
{
    /// <summary>
    /// Gets or sets the name of the variable.
    /// </summary>
    public string VariableName { get; set; }

    /// <summary>
    /// Gets or sets the expression type of the variable.
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the variable.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Variable"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this variable reference.</param>
    /// <param name="name">The name of the variable.</param>
    public Variable(CodeLocation location, string name) : base(location)
    {
        VariableName = name;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the variable node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.Variable(this);
    }
}