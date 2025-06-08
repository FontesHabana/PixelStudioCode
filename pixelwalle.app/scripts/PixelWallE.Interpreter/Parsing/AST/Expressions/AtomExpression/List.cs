using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

namespace PixelWallE.Language.Expressions;

/// <summary>
/// Represents a list expression.
/// </summary>
public class List : AtomExpression, IArgument<Expression>, IName
{

    /// <summary>
    /// Gets or sets the arguments (elements) of the list.
    /// </summary>
    public virtual List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the expression type of the list.
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the list.
    /// </summary>
    public override object? Value { get; set; }

    
    /// <summary>
    /// Gets or sets the name of the list.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="List"/> class.
    /// </summary>
    /// <param name="location">The code location where this list is defined.</param>
    /// <param name="args">The initial arguments (elements) of the list.</param>
    /// <param name="type">The expression type of the list.</param>
    public List(CodeLocation location, List<Expression>? args, ExpressionType type) : base(location)
    {
       
        Name = $"List<{type}>";
        Type = type;
        if (args == null)
        {
            Args = new List<Expression>();
        }
        else
        {
            Args = args;
        }
    }

    /// <summary>
    /// Accepts a visitor to traverse this node.
    /// </summary>
    /// <param name="node">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> node)
    {
        node.List(this);
    }
}