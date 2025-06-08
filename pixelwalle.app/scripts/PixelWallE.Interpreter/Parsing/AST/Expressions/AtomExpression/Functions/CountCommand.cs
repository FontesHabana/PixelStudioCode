namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;
using System.ComponentModel.Design;

/// <summary>
/// Represents a command to count the number of elements in a list.
/// </summary>
public class CountCommand : Function, IListReference
{
    /// <summary>
    /// Gets or sets the reference to the list to be counted.
    /// </summary>
    public virtual string ListReference { get;  set; }
    /// <summary>
    /// Gets or sets the arguments of the count command.
    /// </summary>
    public override List<Expression> Args { get; set; }

    
    /// <summary>
    /// Gets or sets the expression type of the count command (always Number).
    /// </summary>
    public override ExpressionType Type { get; set; }

    
    /// <summary>
    /// Gets or sets the value of the count command.
    /// </summary>
    public override object? Value { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="CountCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where this command is defined.</param>
    /// <param name="functionName">The token representing the function name.</param>
    /// <param name="args">The arguments passed to the count command.</param>
    public CountCommand(CodeLocation location, TokenType functionName, List<Expression> args)
        : base(location, functionName, args)
    {
        Name = "Count";
        Type = ExpressionType.Number;
        ListReference = "";
    }

    /// <summary>
    /// Accepts a visitor to traverse this node.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.CountCommand(this);
    }
}