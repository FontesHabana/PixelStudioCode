

namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

public class ColorCommand : Command
{
    public ColorCommand(CodeLocation location, TokenType nameCommand, List<Expression> args):base(location, nameCommand, args){
         Name = "Color";
    }


    

    public override void Accept(IVisitor<ASTNode> visitor)
    {
      visitor.ColorCommand(this);
    }
}