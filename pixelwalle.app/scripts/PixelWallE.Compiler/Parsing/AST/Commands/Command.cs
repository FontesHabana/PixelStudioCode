namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

/// <summary>
/// Represents the abstract base class for all command nodes in the PixelWallE language.
/// </summary>
public abstract class Command : ASTNode, IArgument<Expression>, IName
{
    /// <summary>
    /// Gets or sets the token type representing the command name.
    /// </summary>
    public TokenType CommandName { get; set; }

    /// <summary>
    /// Gets or sets the list of argument expressions for the command.
    /// </summary>
    public List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the name of the command.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="location">The code location where the command appears.</param>
    /// <param name="commandName">The token type representing the command name.</param>
    /// <param name="args">The list of argument expressions for the command.</param>
    public Command(CodeLocation location, TokenType commandName, List<Expression> args) : base(location)
    {
        if (args == null)
        {
            Args = new List<Expression>();
        }
        else
        {
            Args = args;
        }
        CommandName = commandName;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override abstract void Accept(IVisitor<ASTNode> visitor);
}