

namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

public class SpawnCommand : Command
{
    public SpawnCommand(CodeLocation location, TokenType nameCommand, List<Expression> args):base(location, nameCommand, args){
        
    }


    
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.SpawnCommand(this);
    }
}