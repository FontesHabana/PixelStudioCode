
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;

namespace PixelWallE.Language.Expressions;

public abstract  class Function: AtomExpression, IArgument<Expression>, IName
{
    public virtual List<Expression> Args {get; set;}
    public TokenType FunctionType {get; set;}
    public  virtual string Name {get; set;}
  

    public Function(CodeLocation location, TokenType functionType, List<Expression>? args) : base(location)
    {
        FunctionType = functionType;
        if (args == null)
        {
            Args = new List<Expression>();
        }
        else
        {
            Args = args;
        }
    }

   
    

    
}