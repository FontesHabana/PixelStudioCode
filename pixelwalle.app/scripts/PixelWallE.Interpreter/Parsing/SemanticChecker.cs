namespace PixelWallE.Language.Parsing;

using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using PixelWallE.Language;
using System;
using PixelWallE.Core;
using PixelWallE.Language.Parsing.Expressions.Literals;

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
        if (!scope.IsDeclared(var.VariableName, scope.variables))
        {

            errors.Add(SemanticException.UndeclaredVariable(var.VariableName, var.Location));
            return;
        }
        var.Type = scope.GetVariableType(var.VariableName);
    }
 /// <summary>
    /// Gets the expression type based on the given list type.
    /// </summary>
    /// <param name="type">The list type.</param>
    /// <returns>The expression type.</returns>
    private ExpressionType GetListExpressionType(ExpressionType type)
    {
        ExpressionType argType = ExpressionType.Anytype;
        if (type == ExpressionType.ListIntegerOrBool)
            argType = ExpressionType.IntegerOrBool;
        if (type == ExpressionType.ListString)
            argType = ExpressionType.String;
        return argType;
    }

  /// <summary>
    /// Performs semantic analysis on a list, checking the types of its arguments.
    /// </summary>
    /// <param name="list">The list to analyze.</param>
  
    public void List(List list)
    {
        List<ExpressionType> typeExpression = new List<ExpressionType>();
        ExpressionType argType = GetListExpressionType(list.Type);


        foreach (Expression expression in list.Args)
        {

            typeExpression.Add(argType);
        }
        CheckArguments(typeExpression, list);
    }
  /// <summary>
    /// Performs semantic analysis on a list element, ensuring the list is declared and the index is a number.
    /// </summary>
    /// <param name="element">The list element to analyze.</param>
   
    public void ListElement(ListElement element)
    {

        if (!scope.IsDeclared(element.ListReference, scope.variables)) // Verifica si la variable de la lista está declarada
        {
            errors.Add(SemanticException.UndeclaredVariable(element.ListReference, element.Location));
            return; // Detiene verificaciones adicionales si la lista no está declarada
        }
        bool declaredVar = scope.IsDeclared(element.ListReference, scope.variables);


        if (declaredVar) // Verifica si la variable declarada es una instancia de List
        {

            element.Index.Accept(this);
            if (element.Index.Type == ExpressionType.IntegerOrBool)
            {
                ExpressionType type = GetListExpressionType(scope.GetVariableType(element.ListReference));
                element.Type = type;
            }
            else
            {
                errors.Add(SemanticException.TypeMismatch("index", ExpressionType.IntegerOrBool, element.Index.Type, element.Index.Location));
            }

        }
        else
        {
            errors.Add(SemanticException.UndeclaredVariable(element.ListReference, element.Location));
        }
    }


    /// <summary>
    /// Helper method to check arguments for functions or commands.
    /// It iterates through provided arguments, visits them, checks their types against expected types,
    /// and verifies the argument count. It also includes specific checks for string arguments that might be colors.
    /// </summary>
    /// <param name="argsType">A list of <see cref="ExpressionType"/> representing the expected types of arguments.</param>
    /// <param name="node">The <see cref="ASTNode"/> (Command or Function) whose arguments are being checked.</param>

    private void CheckArguments(List<ExpressionType> argsType, ASTNode node)
    {
        if (node is IArgument<Expression> function)
        {

            string name = (node is IName nameNode) ? nameNode.Name : node.GetType().Name;


            foreach (Expression arg in function.Args)
            {
                Godot.GD.Print(arg);
                arg.Accept(this);

                Godot.GD.Print("Checkear argumento");
                Godot.GD.Print(arg.Type);
            }

            for (int i = 0; i < Math.Min(function.Args.Count, argsType.Count); i++)
            {
                if (function.Args[i].Type != argsType[i])
                {
                    errors.Add(SemanticException.TypeMismatch(name, argsType[i], function.Args[i].Type, function.Args[i].Location));
                }

                /* if (function.Args[i].Type == ExpressionType.String)
                 {

                     if (!scope.IsDeclared(function.Args[i].Value.ToString(), scope.colors))
                     {
                         errors.Add(SemanticException.UndeclaredColor(function.Args[i].ToString(), node.Location));
                     }
                 }*/


            }

            if (function.Args.Count != argsType.Count)
            {
                errors.Add(SemanticException.IncorrectArgumentCount(name, argsType.Count, function.Args.Count, node.Location));
            }

        }

    }

    private bool CheckColor(Expression posibleColor, out PixelColor myColor)
    {
        myColor = new PixelColor(0, 0, 0);
        if (posibleColor.Value != null)
        {
            Godot.GD.Print("Hay esta cosa");
            Godot.GD.Print(posibleColor.Value);
            if (posibleColor.Type == ExpressionType.String)
            {

                if (!PixelColor.TryParse(posibleColor.Value.ToString(), out PixelColor color))
                {
                    errors.Add(SemanticException.UndeclaredColor(posibleColor.ToString(), posibleColor.Location));
                }
                myColor = color;
                return true;
            }
        }


        return false;
    }

    #region functions

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
        CheckArguments(new List<ExpressionType>() { ExpressionType.String, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, function);
        CheckColor(function.Args[0], out PixelColor color);
        function.color = color;

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsBrushColorFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsBrushColorFunction(IsBrushColorFunction function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, function);
        CheckColor(function.Args[0], out PixelColor color);
        function.color = color;
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsBrushSizeFunction"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsBrushSizeFunction(IsBrushSizeFunction function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool }, function);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="IsCanvasColor"/>.
    /// </summary>
    /// <param name="function">The function to analyze.</param>
    public void IsCanvasColor(IsCanvasColor function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, function);
        CheckColor(function.Args[0], out PixelColor color);
        function.color = color;

    }


    #endregion


    #region  Unary Expressions
    private void UnaryOperation(UnaryExpression operation, ExpressionType type)
    {
        operation.Right.Accept(this);
        if (operation.Right.Type != type)
        {
            errors.Add(SemanticException.InvalidOperation(operation.ToString(), operation.Right.Type, operation.Location));
        }
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="NotOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void NotOperation(NotOperation operation)
    {
        UnaryOperation(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="NegationOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void NegationOperation(NegationOperation operation)
    {
        UnaryOperation(operation, ExpressionType.IntegerOrBool);
    }
    #endregion

    #region Binary Expressions

    private void BinaryExpression(BinaryExpression operation, ExpressionType type)
    {
        operation.Right.Accept(this);
        operation.Left.Accept(this);
        Godot.GD.Print("Binary Expression Check");
          Godot.GD.Print(type);
        Godot.GD.Print(operation.Left.Type);
        Godot.GD.Print(operation.Right.Type);

        if (operation.Left.Type != type)
        {       Godot.GD.Print("Doy Error Aqui");
            errors.Add(SemanticException.InvalidOperation(operation.ToString(), operation.Left.Type, operation.Right.Type, operation.Location));
        }
        if (operation.Right.Type != type)
        {   Godot.GD.Print("Doy Error Aqui 2");
            errors.Add(SemanticException.InvalidOperation(operation.ToString(), operation.Right.Type, operation.Left.Type, operation.Location));
        }
    }

    #region Arithmetic Operation
    /// <summary>
    /// Performs semantic analysis on the given <see cref="AdditionOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void AdditionOperation(AdditionOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        if (operation.Left.Type == ExpressionType.String|| operation.Right.Type == ExpressionType.String)
        {
            operation.Type = ExpressionType.String;
           
        }
        else
        {
            operation.Type = ExpressionType.IntegerOrBool;

        }
      
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DivisionOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void DivisionOperation(DivisionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ExponentiationOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void ExponentiationOperation(ExponentiationOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ModuloOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void ModuloOperation(ModuloOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="MultiplicationOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void MultiplicationOperation(MultiplicationOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="SubstractionOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void SubstractionOperation(SubstractionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    #endregion

    #region Logic Operation
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ANDOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void ANDOperation(ANDOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="OrOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void OrOperation(OrOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="EqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void EqualToOperation(EqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        if (operation.Right.Type != operation.Left.Type)
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

        if (operation.Right.Type != operation.Left.Type)
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
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GreatherThanOrEqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="LessThanOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void LessThanOperation(LessThanOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="LessThanOrEqualToOperation"/>.
    /// </summary>
    /// <param name="operation">The operation to analyze.</param>
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation)
    {
        BinaryExpression(operation, ExpressionType.IntegerOrBool);
    }


    #endregion

    #endregion
    #endregion

    #region Command

    #region ListCommand
    /// <summary>
    /// Performs semantic analysis on the given <see cref="AddCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void AddCommand(AddCommand command)
    {
        if (!scope.IsDeclared(command.ListReference, scope.variables)) 
        {
            errors.Add(SemanticException.UndeclaredVariable(command.ListReference, command.Location));
            return; 
        }
        ExpressionType declaredVar = scope.GetVariableType(command.ListReference);
        ExpressionType argType = GetListExpressionType(declaredVar);

        CheckArguments(new List<ExpressionType> { argType }, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="RemoveAtCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void RemoveAtCommand(RemoveAtCommand command)
    {

        CheckArguments(new List<ExpressionType> { ExpressionType.IntegerOrBool }, command);
    }
   /// <summary>
    /// Performs semantic analysis on the given <see cref="ClearCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void ClearCommand(ClearCommand command)
    {
        CheckArguments(new List<ExpressionType>(), command);
    }
     /// <summary>
    /// Performs semantic analysis on the given <see cref="CountCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void CountCommand(CountCommand command)
    {
        CheckArguments(new List<ExpressionType>(), command);
    }
    #endregion







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



    public void AssigmentListElement(AssigmentListElement command)
    {
        command.Argument.Accept(this);
        command.Index.Accept(this);

    
        if (!scope.IsDeclared(command.Var.VariableName, scope.variables)) // Verifica si la variable de la lista está declarada
        {
            errors.Add(SemanticException.UndeclaredVariable(command.Var.VariableName, command.Location));
            return; 
        }
       
        
        ExpressionType declaredVar = scope.GetVariableType(command.Var.VariableName);
        ExpressionType argType = GetListExpressionType(declaredVar);
          if (argType != command.Argument.Type)
         {
                errors.Add(SemanticException.TypeMismatch("Assigment", argType, command.Argument.Type, command.Location));
         }
          if (command.Index.Type != ExpressionType.IntegerOrBool)
         {
                errors.Add(SemanticException.TypeMismatch("Assigment", ExpressionType.IntegerOrBool, command.Index.Type, command.Location));
         }

      

    }



    /// <summary>
    /// Performs semantic analysis on the given <see cref="ColorCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void ColorCommand(ColorCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, command);
        if (CheckColor(command.Args[0], out PixelColor color))
        {
             command.color = color;
        }
       

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DrawCircleCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void DrawCircleCommand(DrawCircleCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DrawLineCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void DrawLineCommand(DrawLineCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="DrawRectangleCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void DrawRectangleCommand(DrawRectangleCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, command);

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
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool }, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="SpawnCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void SpawnCommand(SpawnCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, command);

    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="ReSpawnCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void ReSpawnCommand(ReSpawnCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool, ExpressionType.IntegerOrBool }, command);
    }
    /// <summary>
    /// Performs semantic analysis on the given <see cref="GoToCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void GoToCommand(GoToCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.IntegerOrBool }, command);
        if (!scope.IsDeclared(command.Label, scope.labels))
        {
            errors.Add(SemanticException.LabelNotFound(command.Label, command.Location));
        }

    }


    /// <summary>
    /// Performs semantic analysis on the given <see cref="PrintCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void PrintCommand(PrintCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        if (command.Args.Count() != 1)
        {
            errors.Add(SemanticException.IncorrectArgumentCount(command.Name, 1, command.Args.Count(), command.Location));
        }


    }
   
   
    /// <summary>
    /// Performs semantic analysis on the given <see cref="RunCommand"/>.
    /// </summary>
    /// <param name="command">The command to analyze.</param>
    public void RunCommand(RunCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, command);
      

    }
    #endregion

}