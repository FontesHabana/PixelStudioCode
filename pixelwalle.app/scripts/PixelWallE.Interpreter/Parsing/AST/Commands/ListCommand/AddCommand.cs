namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;

/// <summary>
/// Represents a command to add an element to a list.
/// </summary>
public class AddCommand : ListCommand
{
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AddCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where this command is defined.</param>
    /// <param name="nameCommand">The token representing the command name.</param>
    /// <param name="args">The arguments passed to the command (list reference and element to add).</param>
    public AddCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        Name = "Add";
    
    }

    /// <summary>
    /// Accepts a visitor to traverse this node.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.AddCommand(this);
    }
}