using System.Linq.Expressions;
using PixelWallE.Language.Commands;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Parsing.Expressions;

namespace PixelWallE.Language.Parsing;

/// <summary>
/// Defines the Visitor interface for traversing and operating on AST nodes.
/// Each method corresponds to a specific node type in the AST.
/// </summary>
/// <typeparam name="T">The return type for the visitor methods (often unused).</typeparam>
public interface IVisitor<T>
{
    /// <summary>
    /// Visits an ElementalProgram node.
    /// </summary>
    void ElementalProgram(ElementalProgram program);

    #region Expressions

    void ParenthesizedExpression(ParenthesizedExpression parenthesizedExpression);
    void Variable(Variable var);

   #region  Functions

    void GetActualXFunction(GetActualXFunction function);
    void GetActualYFunction(GetActualYFunction function);
    void GetCanvasSizeFunction(GetCanvasSizeFunction function);
    void GetColorCountFunction(GetColorCountFunction function);
    void IsBrushColorFunction(IsBrushColorFunction function);
    void IsBrushSizeFunction(IsBrushSizeFunction function);
    void IsCanvasColor(IsCanvasColor function);
    void IsColorFunction(IsColorFunction function);
   #endregion
    // Unary Expressions
    void NotOperation(NotOperation operation);
    void NegationOperation(NegationOperation operation);

    // Binary Expressions
    #region Arithmetic Operation
     void AdditionOperation(AdditionOperation operation);
     void DivisionOperation(DivisionOperation operation);
     void ExponentiationOperation(ExponentiationOperation operation);
     void ModuloOperation(ModuloOperation operation);
     void MultiplicationOperation(MultiplicationOperation operation);
     void SubstractionOperation(SubstractionOperation operation);
    #endregion
    
    #region Logic Operation
    void ANDOperation(ANDOperation operation);
    void OrOperation(OrOperation operation);
    void EqualToOperation(EqualToOperation operation);
    void GreatherThanOperation(GreatherThanOperation operation);
    void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation);
    void LessThanOperation(LessThanOperation operation);
    void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation);
    void NotEqualToOperation(NotEqualToOperation operation);
    #endregion
    #endregion 


 #region Command
  
    void AssigmentExpression(AssigmentExpression command);
    void ColorCommand(ColorCommand command);
    void DrawCircleCommand(DrawCircleCommand command);
    void DrawLineCommand(DrawLineCommand command);
    void DrawRectangleCommand(DrawRectangleCommand command);
    void FillCommand(FillCommand command);
    void SizeCommand(SizeCommand command);
    void SpawnCommand(SpawnCommand command);
    void GoToCommand(GoToCommand command);
    #endregion
   }