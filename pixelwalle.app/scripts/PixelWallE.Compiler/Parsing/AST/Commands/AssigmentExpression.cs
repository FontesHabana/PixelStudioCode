namespace PixelWallE.Language.Commands;

using System.Collections.Generic;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

public class AssigmentExpression: ASTNode, IName
{   public Variable Var {get; private set;}
    public Expression Argument {get; private set;}
    public virtual string Name{ get; set; }

    public AssigmentExpression(CodeLocation location, Variable var, Expression argument) : base(location)
    {
        Var = var;
        Argument = argument;
        Name = "Assigment";
    }


    
    public override void Accept(IVisitor<ASTNode> visitor){
       visitor.AssigmentExpression(this);
    }
}
