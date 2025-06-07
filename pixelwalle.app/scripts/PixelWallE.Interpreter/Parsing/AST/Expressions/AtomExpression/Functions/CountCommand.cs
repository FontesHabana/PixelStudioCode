namespace PixelWallE.Language.Commands;

using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;
using System.ComponentModel.Design;

public class CountCommand : Function, IListReference
{
    public virtual string ListReference { get;  set; }
     public override List<Expression> Args { get; set; }

    
    public override ExpressionType Type { get; set; }

    
    public override object? Value { get; set; }
    public CountCommand(CodeLocation location, TokenType functionName, List<Expression> args)
        : base(location, functionName, args)
    {
        Name = "Count";
        Type = ExpressionType.Number;
        ListReference = "";
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.CountCommand(this);
    }
}