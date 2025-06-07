using System.Collections.Generic;

namespace PixelWallE.Language.Parsing.Expressions.Literals;


public class ListElement : AtomExpression
{
   
    public override ExpressionType Type{ get; set; }

 
    public override object? Value { get; set; }

    public string ListReference{ get; set; }
   public Expression? Index { get; set; }


    public ListElement(CodeLocation location, string listReference, Expression index) : base(location)
    {
        ListReference = listReference;
        Index = index;
       
    }

    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.ListElement(this);
    }
}