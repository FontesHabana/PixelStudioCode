namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

/// <summary>
/// Represents the 'Spawn' command in the PixelWallE language, which sets the initial position of the robot.
/// </summary>
public class SpawnCommand : Command
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpawnCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where the command appears.</param>
    /// <param name="nameCommand">The token type representing the command name.</param>
    /// <param name="args">The list of argument expressions for the command.</param>
    public SpawnCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        Name = "Spawn";
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.SpawnCommand(this);
    }
}