
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

public class SemanticChecker : IVisitor<ASTNode>
{
    //Checkear color declarado aislarlo en una sola funcion
    private Scope scope;
    public List<PixelWallEException> errors;

    public SemanticChecker(Scope scope, List<PixelWallEException> error)
    {
        this.scope = scope;
        errors = error;
    }

    public void ElementalProgram(ElementalProgram program)
    {   
        
        foreach (var command in program.Statements)
        {
           command.Accept(this);
        }
    }

    #region Expressions
    public void ParenthesizedExpression(ParenthesizedExpression exp)
    {
        //Revisar si el checkeo semantico aqui incluye o no parentesis

        exp.InnerExpression.Accept(this);
        //revisar si esto se puede llenar en el parser
        exp.Type = exp.InnerExpression.Type;

    }
    public void Variable(Variable var)
    {
        if (!scope.IsDeclared(var.VariableName,scope.variables))
        {
           
            errors.Add(SemanticException.UndeclaredVariable(var.VariableName, var.Location));
            return;
        }
        var.Type=scope.GetVariableType(var.VariableName);
    }

//Quitar la lista de argumentos viene implicita por el function
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
    public void GetActualXFunction(GetActualXFunction function)
    {
        CheckArguments(new List<ExpressionType>(), function);
    }

    public void GetActualYFunction(GetActualYFunction function)
    {

       CheckArguments(new List<ExpressionType>(), function);
    }
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function)
    {
         CheckArguments(new List<ExpressionType>(), function);
    }
    public void GetColorCountFunction(GetColorCountFunction function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number }, function);
         
    }
    public void IsBrushColorFunction(IsBrushColorFunction function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, function);
         
    }
    public void IsBrushSizeFunction(IsBrushSizeFunction function)
    {
        CheckArguments(new List<ExpressionType>(){ExpressionType.Number}, function);
    }
    public void IsCanvasColor(IsCanvasColor function)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String, ExpressionType.Number, ExpressionType.Number }, function);
        
    }
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
    public void NotOperation(NotOperation operation)
    {
        UnaryOperation(operation, ExpressionType.Bool);
    }
    public void NegationOperation(NegationOperation operation)
    {
        UnaryOperation(operation, ExpressionType.Number);
    }
    #endregion

    #region Binary Expressions
    //Esto es una gran mierda por favor revisa como vas a comprobar las variables y el tipo de estas Quizas un método que compruebe si es una variable y devuelva el tipo de la variable
    //Analizar como no añadir error si la variable no está declarada porque es del tipo AnyType
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
    public void AdditionOperation(AdditionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void DivisionOperation(DivisionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void ExponentiationOperation(ExponentiationOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void ModuloOperation(ModuloOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void MultiplicationOperation(MultiplicationOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void SubstractionOperation(SubstractionOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    #endregion

    #region Logic Operation
    public void ANDOperation(ANDOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Bool);
    }
    public void OrOperation(OrOperation operation)
    {
         BinaryExpression(operation, ExpressionType.Bool);
    }
    public void EqualToOperation(EqualToOperation operation)
    {   operation.Left.Accept(this);
        operation.Right.Accept(this);
        
        if (operation.Right.Type !=operation.Left.Type)
        {
            errors.Add(SemanticException.TypeMismatch("Equal", operation.Left.Type, operation.Right.Type, operation.Location));
        }
       
    }
    public void NotEqualToOperation(NotEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        if (operation.Right.Type !=operation.Left.Type)
        {
            errors.Add(SemanticException.TypeMismatch("NotEqual", operation.Left.Type, operation.Right.Type, operation.Location));
        }
    }

    public void GreatherThanOperation(GreatherThanOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void LessThanOperation(LessThanOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation)
    {
        BinaryExpression(operation, ExpressionType.Number);
    }
    

    #endregion

    #endregion
    #endregion

    #region Command
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
    public void ColorCommand(ColorCommand command)
    {
        CheckArguments(new List<ExpressionType>() { ExpressionType.String }, command);
        
    }
    public void DrawCircleCommand(DrawCircleCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number, ExpressionType.Number}, command);
    }
    public void DrawLineCommand(DrawLineCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number, ExpressionType.Number}, command);
    }
    public void DrawRectangleCommand(DrawRectangleCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number}, command);

    }
    public void FillCommand(FillCommand command)
    {
        CheckArguments(new List<ExpressionType>(), command);
    }
    public void SizeCommand(SizeCommand command)
    {
         CheckArguments(new List<ExpressionType>(){ExpressionType.Number}, command);
    }
    public void SpawnCommand(SpawnCommand command)
    {
        CheckArguments(new List<ExpressionType>(){ExpressionType.Number, ExpressionType.Number}, command);

    }
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