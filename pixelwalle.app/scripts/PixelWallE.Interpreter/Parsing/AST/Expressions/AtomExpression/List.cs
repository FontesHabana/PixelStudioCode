using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

namespace PixelWallE.Language.Expressions;


public class List : AtomExpression, IArgument<Expression>, IName
{

    public virtual List<Expression> Args { get; set; }

    public override ExpressionType Type { get; set; }

    public override object? Value { get; set; }

    
    public virtual string Name { get; set; }


    public List(CodeLocation location, List<Expression>? args, ExpressionType type) : base(location)
    {
       
        Name = $"List<{type}>";
        Type = type;
        if (args == null)
        {
            Args = new List<Expression>();
        }
        else
        {
            Args = args;
        }
    }

    public override void Accept(IVisitor<ASTNode> node)
    {
        node.List(this);
    }
}