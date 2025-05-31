namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

/// <summary>
/// Represents the 'DrawRectangle' command in the PixelWallE language, which draws a rectangle on the canvas.
/// </summary>
public class DrawRectangleCommand : Command
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawRectangleCommand"/> class.
    /// </summary>
    /// <param name="location">The code location of the command.</param>
    /// <param name="nameCommand">The token type for the command.</param>
    /// <param name="args">The argument expressions for the command.</param>
    public DrawRectangleCommand(CodeLocation location, TokenType nameCommand, List<Expression> args):base(location, nameCommand, args){
         Name = "DrawRectangle";
    }



    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.DrawRectangleCommand(this);
    }
}