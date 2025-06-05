namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;

/// <summary>
/// Represents the 'Color' command in the PixelWallE language, which sets the brush color.
/// </summary>
public class ColorCommand : Command, IColor
{


    public PixelColor color{ get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where the command appears.</param>
    /// <param name="nameCommand">The token type representing the command name.</param>
    /// <param name="args">The list of argument expressions for the command.</param>
    public ColorCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        Name = "Color";
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ColorCommand(this);
    }
}