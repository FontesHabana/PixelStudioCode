

namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

public class DrawCircleCommand : Command
{
    public DrawCircleCommand(CodeLocation location, TokenType nameCommand, List<Expression> args):base(location, nameCommand, args){
        
    }


    
    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.DrawCircleCommand(this);
    }
}