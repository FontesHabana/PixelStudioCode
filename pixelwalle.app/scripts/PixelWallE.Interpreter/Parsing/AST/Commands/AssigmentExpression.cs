namespace PixelWallE.Language.Commands;

using System.Collections.Generic;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents an assignment expression in the PixelWallE language.
/// </summary>
public class AssigmentExpression : ASTNode, IName
{
    /// <summary>
    /// Gets the variable being assigned.
    /// </summary>
    public Variable Var { get; private set; }

    /// <summary>
    /// Gets the expression whose value is assigned to the variable.
    /// </summary>
    public Expression Argument { get; private set; }

    /// <summary>
    /// Gets or sets the name of the assignment expression.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssigmentExpression"/> class.
    /// </summary>
    /// <param name="location">The code location of the assignment.</param>
    /// <param name="var">The variable being assigned.</param>
    /// <param name="argument">The expression to assign.</param>
    public AssigmentExpression(CodeLocation location, Variable var, Expression argument) : base(location)
    {
        Var = var;
        Argument = argument;
        Name = "Assigment";
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the assignment node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.AssigmentExpression(this);
    }
}
