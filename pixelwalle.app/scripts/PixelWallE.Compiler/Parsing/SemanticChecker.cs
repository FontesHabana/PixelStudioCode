namespace PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using PixelWallE.Language;
using System;
using System.Xml;

/// <summary>
/// Performs semantic analysis on the Abstract Syntax Tree (AST).
/// </summary>
public class SemanticChecker : IVisitor<ASTNode>
{
    //Checkear color declarado aislarlo en una sola funcion
    private Scope scope;
    public List<PixelWallEException> errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticChecker"/> class.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="error">The error list.</param>
    public SemanticChecker(Scope scope, List<PixelWallEException> error)
    {
        this.scope = scope;
        errors = error;
    }

    /// <summary>
    /// Performs semantic analysis on the given <see cref="ElementalProgram"/>.
    /// </summary>
    /// <param name="program">The program to analyze.</param>
    public void ElementalProgram(ElementalProgram program)
    {   
        
        foreach (var command in program.Statements)
        {
           command.Accept(this);
        }
    }

    #region Expressions
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ParenthesizedExpression"/>.
    /// </summary>
    /// <param name="exp">The expression to analyze.</param>
    public void ParenthesizedExpression(ParenthesizedExpression exp)
    {
        //Revisar si el checkeo semantico aqui incluye o no parentesis

        exp.InnerExpression.Accept(this);
        //revisar si esto se puede llenar en el parser
        exp.Type = exp.InnerExpression.Type;

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="Variable"/>.
    /// </summary>
    /// <param name="var">The variable to analyze.</param>
    public void Variable(Variable var)
    {
        if (!scope.IsDeclared(var.VariableName,scope.variables))
        {
           
            errors.Add(SemanticException.UndeclaredVariable(var.VariableName, var.Location));
            return;
        }
        var.Type=scope.GetVariableType(var.VariableName);
    }

    private void CheckArguments( List<ExpressionType> argsType, ASTNode node)
    {
        if (node is IArgument<Expression> function)
        {

            string name = (node is IName nameNode)? nameNode.Name : node.GetType().Name;


        foreach (Expression arg in function.Args)
            {
                arg.Accept(this);
            }

            for (int i = 0; i < Math.Min(function.Args.Count, argsType.Count); i++)
            {
                if (function.Args[i].Type != argsType[i])
                {
                    errors.Add(SemanticException.TypeMismatch(name, argsType[i], function.Args[i].Type, node.Location));
                }
            if (function.Args[i].Type==ExpressionType.String)
            {
                 if (!scope.IsDeclared(function.Args[i].ToString(), scope.colors))
            {
                SemanticException.UndeclaredColor(function.Args[i].ToString(), node.Location);
            }
            }
           
        
     }

        if (function.Args.Count != argsType.Count)
        {
            errors.Add(SemanticException.IncorrectArgumentCount(name, argsType.Count, function.Args.Count, node.Location));
        }

        }
        
    }

    
    #region functions
    //Functions
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GetActualXFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void GetActualXFunction(GetActualXFunction function)
    {
        CheckArguments(new List<ExpressionType>(), function);
    }

    /// <summary>
    /// Performs semantic analysis on the given <see cref="GetActualYFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void GetActualYFunction(GetActualYFunction function)
    {

       CheckArguments(new List<ExpressionType>(), function);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GetCanvasSizeFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function)
    {
         CheckArguments(new List<ExpressionType>(), function);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GetColorCountFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void GetColorCountFunction(GetColorCountFunction function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number }, function);
         
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsBrushColorFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsBrushColorFunction(IsBrushColorFunction function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, function);
         
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsBrushSizeFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsBrushSizeFunction(IsBrushSizeFunction function)
    {
        CheckArguments(new List<ExpressionType>(){ExpressionType.Number}, function);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsCanvasColor"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsCanvasColor(IsCanvasColor function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String, ExpressionType.Number, ExpressionType.Number }, function);
        
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsColorFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsColorFunction(IsColorFunction function)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.String, ExpressionType.Number,ExpressionType.Number}, function);
    }
    #endregion


    #region  Unary Expressions
    private void UnaryOperation(UnaryExpression operation, ExpressionType type)
    {   operation.Right.Accept(this);
        if (operation.Right.Type !=type)
        {
            errors.Add(SemanticException.InvalidOperation(operation.ToString(),operation.Right.Type, operation.Location));
        }
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="NotOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void NotOperation(NotOperation operation)
    {
        UnaryOperation(operation, ExpressionType.Bool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="NegationOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void NegationOperation(NegationOperation operation)
    {
        UnaryOperation(operation, ExpressionType.Number);
    }
    #endregion

    #region Binary Expressions

    private void BinaryExpression(BinaryExpression operation, ExpressionType type)
    {
        operation.Right.Accept(this);
        operation.Left.Accept(this);

       if (operation.Left.Type != type)
        {
            errors.Add(SemanticException.InvalidOperation(operation.ToString(), operation.Left.Type, operation.Location));
        }
        if (operation.Right.Type != type)
        {
            errors.Add(SemanticException.InvalidOperation(operation.ToString(), operation.Right.Type, operation.Location));
        }
    }
    
    #region Arithmetic Operation
    /// <summary>
    /// Performs semantic analysis on the given <see cref="AdditionOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void AdditionOperation(AdditionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DivisionOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void DivisionOperation(DivisionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ExponentiationOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void ExponentiationOperation(ExponentiationOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ModuloOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void ModuloOperation(ModuloOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="MultiplicationOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void MultiplicationOperation(MultiplicationOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="SubstractionOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void SubstractionOperation(SubstractionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    #endregion

    #region Logic Operation
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ANDOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void ANDOperation(ANDOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Bool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="OrOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void OrOperation(OrOperation operation)
    {
         BinaryExpression(operation, ExpressionType.Bool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="EqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void EqualToOperation(EqualToOperation operation)
    {   operation.Left.Accept(this);
        operation.Right.Accept(this);
        
        if (operation.Right.Type !=operation.Left.Type)
        {
            errors.Add(SemanticException.TypeMismatch("Equal", operation.Left.Type, operation.Right.Type, operation.Location));
        }
       
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="NotEqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void NotEqualToOperation(NotEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        if (operation.Right.Type !=operation.Left.Type)
        {
            errors.Add(SemanticException.TypeMismatch("NotEqual", operation.Left.Type, operation.Right.Type, operation.Location));
        }
    }

    /// <summary>
    /// Performs semantic analysis on the given <see cref="GreatherThanOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void GreatherThanOperation(GreatherThanOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GreatherThanOrEqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="LessThanOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void LessThanOperation(LessThanOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="LessThanOrEqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    

    #endregion

    #endregion
    #endregion

    #region Command
    /// <summary>
    /// Performs semantic analysis on the given <see cref="AssigmentExpression"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void AssigmentExpression(AssigmentExpression command)
    {
        command.Argument.Accept(this);

        if (scope.IsDeclared(command.Var.VariableName, scope.variables))
        {
            if (scope.GetVariableType(command.Var.VariableName) != command.Argument.Type)
            {
                errors.Add(SemanticException.TypeMismatch("Assigment", command.Var.Type, command.Argument.Type, command.Location));
            }
        }

        scope.DeclareVariable(command.Var.VariableName, null, command.Argument.Type);
        
        

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ColorCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void ColorCommand(ColorCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, command);
        
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DrawCircleCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void DrawCircleCommand(DrawCircleCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number, ExpressionType.Number}, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DrawLineCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void DrawLineCommand(DrawLineCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number, ExpressionType.Number}, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DrawRectangleCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void DrawRectangleCommand(DrawRectangleCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number}, command);

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="FillCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void FillCommand(FillCommand command)
    {
        CheckArguments(new List<ExpressionType>(), command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="SizeCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void SizeCommand(SizeCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number}, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="SpawnCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void SpawnCommand(SpawnCommand command)
    {
        CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number}, command);

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GoToCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void GoToCommand(GoToCommand command)
    {
        CheckArguments(new List<ExpressionType>(){ExpressionType.Bool}, command);
        if (!scope.IsDeclared(command.Label,scope.labels))
        {
            errors.Add(SemanticException.LabelNotFound(command.Label, command.Location));
        }

    }
    #endregion

}