namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;

/// <summary>
/// Represents a base class for commands that operate on lists.
/// </summary>
public class ListCommand : Command, IListReference
{

    /// <summary>
    /// Gets or sets the reference to the list being manipulated.
    /// </summary>
    public virtual string ListReference { get;  set; }
    

    /// <summary>
    /// Initializes a new instance of the <see cref="ListCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where this command is defined.</param>
    /// <param name="nameCommand">The token representing the command name.</param>
    /// <param name="args">The arguments passed to the command.</param>
    public ListCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        ListReference = "";
    }

    /// <summary>
    /// Accepts a visitor to traverse this node.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
    }

    

   
}