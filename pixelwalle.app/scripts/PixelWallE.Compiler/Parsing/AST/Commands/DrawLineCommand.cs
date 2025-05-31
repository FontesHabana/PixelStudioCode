namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

/// <summary>
/// Represents the 'DrawLine' command in the PixelWallE language, which draws a line on the canvas.
/// </summary>
public class DrawLineCommand : Command
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawLineCommand"/> class.
    /// </summary>
    /// <param name="location">The code location of the command.</param>
    /// <param name="nameCommand">The token type for the command.</param>
    /// <param name="args">The argument expressions for the command.</param>
    public DrawLineCommand(CodeLocation location, TokenType nameCommand, List<Expression> args):base(location, nameCommand, args){
         Name = "DrawLine";
    }


    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.DrawLineCommand(this);
    }
}