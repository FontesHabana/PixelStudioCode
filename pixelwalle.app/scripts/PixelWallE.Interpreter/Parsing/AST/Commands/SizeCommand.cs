namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

/// <summary>
/// Represents the 'Size' command in the PixelWallE language, which sets the brush size.
/// </summary>
public class SizeCommand : Command
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SizeCommand"/> class.
    /// </summary>
    /// <param name="location">The code location of the command.</param>
    /// <param name="nameCommand">The token type for the command.</param>
    /// <param name="args">The argument expressions for the command.</param>
    public SizeCommand(CodeLocation location, TokenType nameCommand, List<Expression> args):base(location, nameCommand, args){
        Name = "Size";
    }
    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.SizeCommand(this);
    }
}