
namespace PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;
using System.Net;
using System.Collections.Generic;
using System.Linq;

public class SemanticChecker : IVisitor<ASTNode>
{
    private Scope scope;
    public List<CompilingError> errors;

    public SemanticChecker(Scope scope, List<CompilingError> error)
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
        if (!scope.IsDeclaredVariable(var.VariableName))
        {
            errors.Add(new CompilingError(var.Location, ErrorCode.None, $"La variable {var.VariableName} no está declarada"));
            return;
        }
        var.Type=scope.GetVariableType(var.VariableName);
    }

    private void CheckArguments(List<Expression> args)
    {
        foreach (var arg in args)
        {
            arg.Accept(this);
        }
    }
    #region functions
    //Functions
    public void GetActualXFunction(GetActualXFunction function)
    {
        if (function.Args.Count != 0)
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "La función No debe recibir ningún argumento"));
    }

    public void GetActualYFunction(GetActualYFunction function)
    {

        if (function.Args.Count != 0)
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "La función No debe recibir ningún argumento"));
    }
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function)
    {

        if (function.Args.Count != 0)
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "La función No debe recibir ningún argumento"));
    }
    public void GetColorCountFunction(GetColorCountFunction function)
    {
        CheckArguments(function.Args);
        if (function.Args.Count!= 5)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, $"No recibe {function.Args.Count} argumento"));
            return;
        }

        if (function.Args[0].Type != ExpressionType.String)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser string"));

        }
        if (function.Args[1].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));

        }
        if (function.Args[2].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));

        }
        if (function.Args[3].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));

        }
        if (function.Args[4].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));

        }
    }
    public void IsBrushColorFunction(IsBrushColorFunction function)
    {
        //Revisar si el checkeo semantico aqui incluye o no parentesis
        CheckArguments(function.Args);

        if (function.Args.Count != 1)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, $"No recibe {function.Args.Count} argumento Deberia recibir 1"));
        return;
        }
        if (function.Args[0].Type != ExpressionType.String)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser string"));
            
        }
    }
    public void IsBrushSizeFunction(IsBrushSizeFunction function)
    {
        CheckArguments(function.Args);
        if (function.Args.Count !=1)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, $"LA funcion no recibe {function.Args.Count()} argumentos"));
return;
        }
        if (function.Args[0].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));

        }
    }
    public void IsCanvasColor(IsCanvasColor function)
    {
        CheckArguments(function.Args);
        if (function.Args.Count != 3)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, $"No recibe {function.Args.Count()} argumento"));
        return;
        }
        if (function.Args[0].Type != ExpressionType.String)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser string"));
        }
        if (function.Args[1].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));
        }
        if (function.Args[2].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));
        }
    }
    public void IsColorFunction(IsColorFunction function)
    {
        CheckArguments(function.Args);
        if (function.Args.Count !=3)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, $"No recibe {function.Args.Count} argumento"));
        return;
        }
        if (function.Args[0].Type != ExpressionType.String)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser string"));
        }
        if (function.Args[1].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));
        }
        if (function.Args[2].Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(function.Location, ErrorCode.Invalid, "Debe ser número"));
        }
    }
    #endregion


    #region  Unary Expressions
    public void NotOperation(NotOperation operation)
    {
        operation.Right.Accept(this);

        if (operation.Right.Type != ExpressionType.Bool)
        {
            errors.Add(new CompilingError(operation.Location, ErrorCode.Invalid, "The operand of the not operation must be a boolean"));
        }
    }
    public void NegationOperation(NegationOperation operation)
    {
        operation.Right.Accept(this);

        if (operation.Right.Type != ExpressionType.Number && operation.Right.Type != ExpressionType.Bool)
        {   
            errors.Add(new CompilingError(operation.Location, ErrorCode.Invalid, "The operand of the negation operation must be a number"));
        }
    }
    #endregion

    #region Binary Expressions
    //Esto es una gran mierda por favor revisa como vas a comprobar las variables y el tipo de estas Quizas un método que compruebe si es una variable y devuelva el tipo de la variable
    //Analizar como no añadir error si la variable no está declarada porque es del tipo AnyType
    private void BinaryExpression(BinaryExpression operation, ExpressionType type)
    {
        operation.Right.Accept(this);
        operation.Left.Accept(this);

       if (operation.Right.Type == ExpressionType.String ||operation.Left.Type==ExpressionType.String)
        {
            errors.Add(new CompilingError(operation.Location, ErrorCode.Invalid, $"Los lados de la expresion deben ser {type} "));
        }
    }
    private void BoolExpression(BinaryExpression operation){
        operation.Right.Accept(this);
        operation.Left.Accept(this);

        if (operation.Left.Type!=ExpressionType.Bool||operation.Right.Type!=ExpressionType.Bool)
        {
             errors.Add(new CompilingError(operation.Location, ErrorCode.Invalid, $"Los lados de la expresion deben ser {ExpressionType.Bool} "));
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
        BoolExpression(operation);
    }
    public void OrOperation(OrOperation operation)
    {
        BoolExpression(operation);
    }
    public void EqualToOperation(EqualToOperation operation)
    {   operation.Left.Accept(this);
        operation.Right.Accept(this);
        
        if ((operation.Right.Type==ExpressionType.String&&operation.Left.Type!=ExpressionType.String)||(operation.Left.Type==ExpressionType.String&&operation.Right.Type!=ExpressionType.String))
        {
            errors.Add(new CompilingError(operation.Location, ErrorCode.Invalid, "Los lados de la expresión deben ser del mismo tipo"));
        }
       
    }
    public void NotEqualToOperation(NotEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        
        if ((operation.Right.Type==ExpressionType.String&&operation.Left.Type!=ExpressionType.String)||(operation.Left.Type==ExpressionType.String&&operation.Right.Type!=ExpressionType.String))
        {
            errors.Add(new CompilingError(operation.Location, ErrorCode.Invalid, "Los lados de la expresión deben ser del mismo tipo"));
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

        if (scope.IsDeclaredVariable(command.Var.VariableName))
        {
            if (scope.GetVariableType(command.Var.VariableName) != command.Argument.Type)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Expected, $"La variable es del tipo {scope.GetVariableType(command.Var.VariableName)} y el argumento es del tipo {command.Argument.Type}"));
            }
        }

        scope.DeclareVariable(command.Var.VariableName, null, command.Argument.Type);
        
        

    }
    public void ColorCommand(ColorCommand command)
    {
        CheckArguments(command.Args);

        if (command.Args.Count != 1)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
        return;
        }
        if (command.Args[0].Type != ExpressionType.String)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un color"));
        }
    }
    public void DrawCircleCommand(DrawCircleCommand command)
    {
        CheckArguments(command.Args);
        if (command.Args.Count != 3)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (command.Args[i].Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un número"));
            }
        }
    }
    public void DrawLineCommand(DrawLineCommand command)
    {
        CheckArguments(command.Args);
        if (command.Args.Count !=3)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (command.Args[i].Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un numero"));
            }
        }
    }
    public void DrawRectangleCommand(DrawRectangleCommand command)
    {
        CheckArguments(command.Args);
        if (command.Args.Count != 5)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
       return;
        }

        for (int i = 0; i < 5; i++)
        {
            if (command.Args[i].Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un número"));

            }
        }

    }
    public void FillCommand(FillCommand command)
    {
        if (command.Args.Count > 0)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
return;
        }

    }
    public void SizeCommand(SizeCommand command)
    {
        CheckArguments(command.Args);
        if (command.Args.Count != 1)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
        return;
        }
        for (int i = 0; i < 1; i++)
        {
            if (command.Args[i].Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un numero"));

            }
        }
    }
    public void SpawnCommand(SpawnCommand command)
    {
        CheckArguments(command.Args);
        if (command.Args.Count != 2)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
        return;
        }

        for (int i = 0; i < 2; i++)
        {
            if (command.Args[i].Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un numero"));
            }
        }

    }
    public void GoToCommand(GoToCommand command)
    {
        CheckArguments(command.Args);

        if (command.Args.Count != 1)
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "Cantidad de argumentos inválida"));
       return;
        }
        for (int i = 0; i < 1; i++)
        {
            if (command.Args[i].Type != ExpressionType.Bool)
            {
                errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "El argumento debe ser un bool"));
            }
        }
        if (!scope.IsDeclaredLabel(command.Label))
        {
            errors.Add(new CompilingError(command.Location, ErrorCode.Invalid, "La etiqueta no se encuentra declarada"));
        }

    }
    #endregion

}