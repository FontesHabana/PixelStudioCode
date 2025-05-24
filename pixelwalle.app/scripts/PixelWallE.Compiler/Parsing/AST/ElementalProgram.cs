using System.Runtime.InteropServices;

using System.Collections.Generic;
namespace PixelWallE.Language.Parsing;

public class ElementalProgram: ASTNode{

    public List<CompilingError> Errors {get; set;}
    public List<ASTNode> Statements {get; set;}
    public List<Label>? Labels{get; private set;}

    public ElementalProgram(CodeLocation location,List<CompilingError> errors):base(location){
        Errors = errors;
        Statements=new List<ASTNode>();
        Labels=new List<Label>();
    }

    public override void Accept(IVisitor<ASTNode> visitor){
        visitor.ElementalProgram(this);
    }

}