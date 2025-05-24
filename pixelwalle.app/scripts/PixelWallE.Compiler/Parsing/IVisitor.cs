
using System.Linq.Expressions;
using PixelWallE.Language.Commands;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Parsing.Expressions;

namespace PixelWallE.Language.Parsing;

public interface IVisitor<T>{
    
    public void ElementalProgram(ElementalProgram program);

    #region Expressions
    public void ParenthesizedExpression(ParenthesizedExpression parenthesizedExpression);
    public void Variable(Variable var);
    #region functions
    //Functions
    public void GetActualXFunction(GetActualXFunction function);
    public void GetActualYFunction(GetActualYFunction function);
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function);
    public void GetColorCountFunction(GetColorCountFunction function);
    public void IsBrushColorFunction(IsBrushColorFunction function);
    public void IsBrushSizeFunction(IsBrushSizeFunction function);
    public void IsCanvasColor(IsCanvasColor function);
    public void IsColorFunction(IsColorFunction function);
    #endregion


    #region  Unary Expressions
    public void NotOperation(NotOperation operation);
    public void NegationOperation(NegationOperation operation);
    #endregion
   
    #region Binary Expressions
    
    #region Arithmetic Operation
     public void AdditionOperation(AdditionOperation operation);
     public void DivisionOperation(DivisionOperation operation);
     public void ExponentiationOperation(ExponentiationOperation operation);
     public void ModuloOperation(ModuloOperation operation);
     public void MultiplicationOperation(MultiplicationOperation operation);
     public void SubstractionOperation(SubstractionOperation operation);
    #endregion
    
    #region Logic Operation
    public void ANDOperation(ANDOperation operation);
    public void OrOperation(OrOperation operation);
    public void EqualToOperation(EqualToOperation operation);
    public void GreatherThanOperation(GreatherThanOperation operation);
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation);
    public void LessThanOperation(LessThanOperation operation);
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation);
    public void NotEqualToOperation(NotEqualToOperation operation);


    #endregion
   
    #endregion
    #endregion 

 #region Command
    public void AssigmentExpression(AssigmentExpression command);
    public void ColorCommand(ColorCommand command);
    public void DrawCircleCommand(DrawCircleCommand command);
    public void DrawLineCommand(DrawLineCommand command);
    public void DrawRectangleCommand(DrawRectangleCommand command);
    public void FillCommand(FillCommand command);
    public void SizeCommand(SizeCommand command);
    public void SpawnCommand(SpawnCommand command);
    public void GoToCommand(GoToCommand command);
    #endregion
   }