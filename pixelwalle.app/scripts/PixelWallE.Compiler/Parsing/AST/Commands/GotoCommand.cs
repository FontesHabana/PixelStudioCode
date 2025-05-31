using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

namespace PixelWallE.Language.Commands;

public class GoToCommand : Command
{   public string Label { get; set; }
public int InfinteCycle{ get; set; }

    public GoToCommand(CodeLocation location, TokenType commandName, List<Expression> args, string label) : base(location, commandName, args)
    {
        Label = label;
        Name = "GoTo";
        InfinteCycle = 0;
    }

   

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.GoToCommand(this);
    }
}