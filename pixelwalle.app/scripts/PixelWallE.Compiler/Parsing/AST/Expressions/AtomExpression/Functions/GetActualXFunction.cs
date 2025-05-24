using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;
namespace PixelWallE.Language.Expressions;

public class GetActualXFunction: Function
{
    public override List<Expression> Args {get;  set;}
   
    public override ExpressionType Type{get;set;}

    public override object? Value { get; set; }

  public GetActualXFunction(CodeLocation location, TokenType functionName, List<Expression?> args) : base(location, functionName, args)
  {
    Type = ExpressionType.Number;
    Name = "GetrrActualX";
    }
   

    public override void Accept(IVisitor<ASTNode> visitor)
    {
      visitor.GetActualXFunction(this);
    }


}