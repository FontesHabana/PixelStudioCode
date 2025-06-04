namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

/// <summary>
/// Represents a command to print a value to the console.
/// </summary>
public class PrintCommand : Command
{
   
    public PrintCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        Name = "Color";
    }

   
    /// <summary>
    /// Accepts a visitor to perform specific operations on this command.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.PrintCommand(this);
    }
}