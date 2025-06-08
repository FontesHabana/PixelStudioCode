namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;

/// <summary>
/// Represents a command to clear all elements from a list.
/// </summary>
public class ClearCommand : ListCommand
{

   
    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where this command is defined.</param>
    /// <param name="nameCommand">The token representing the command name.</param>
    /// <param name="args">The arguments passed to the command (list reference).</param>
    public ClearCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        Name = "Clear";
    }

    /// <summary>
    /// Accepts a visitor to traverse this node.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ClearCommand(this);
    }
}