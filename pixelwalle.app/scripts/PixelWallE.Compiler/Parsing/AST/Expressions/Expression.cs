namespace PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;


public abstract class Expression: ASTNode
{
    
    public abstract ExpressionType Type { get; set; }

    public abstract object? Value { get; set; }
    

    public Expression(CodeLocation location) : base (location) { }

    public override abstract void Accept(IVisitor<ASTNode> node);
    
}