namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;


public class RunCommand : Command, IColor
{


    public PixelColor color{ get; set; }
   
    public RunCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
        Name = "Run";
    }


    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.RunCommand(this);
    }
}