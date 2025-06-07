namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;


public class RemoveAtCommand : ListCommand
{

    public RemoveAtCommand(CodeLocation location, TokenType nameCommand, List<Expression> args)
        : base(location, nameCommand, args)
    {
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.RemoveAtCommand(this);
    }
}