namespace PixelWallE.Language.Commands;

using System.Collections.Generic;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

public class AssigmentExpression: ASTNode
{   public Variable Var {get; private set;}
    public Expression Argument {get; private set;}

    public AssigmentExpression(CodeLocation location, Variable var, Expression argument): base(location){
        Var=var;
        Argument=argument;
    }


    
    public override void Accept(IVisitor<ASTNode> visitor){
       visitor.AssigmentExpression(this);
    }
}
