namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

public abstract class Command: ASTNode, IArgument<Expression>, IName
{
    public  TokenType CommandName {get; set;}
    public  List<Expression> Args {get; set;}
    public  virtual string Name {get; set;}

    public Command(CodeLocation location, TokenType commandName, List<Expression> args) : base(location)
    {
        if (args == null)
        {
            Args = new List<Expression>();
        }
        else
        {
            Args = args;
        }
        CommandName = commandName;

    }
   

    public  override abstract void Accept(IVisitor<ASTNode> visitor);
}