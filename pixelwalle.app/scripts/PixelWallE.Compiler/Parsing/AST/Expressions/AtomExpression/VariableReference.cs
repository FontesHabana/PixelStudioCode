using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

namespace PixelWallE.Language.Parsing.Expressions;

public class Variable :AtomExpression
{
    public string VariableName{get;set;}
    public override ExpressionType Type{ get;  set;}
    public override object? Value{get;set;}

    public Variable( CodeLocation location, string name): base(location){
        VariableName=name;
    }
    public override void Accept(IVisitor<ASTNode> visitor){ 
      visitor.Variable(this);
    }
   

   
}