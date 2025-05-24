using System;
using System.Reflection.Emit;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
namespace PixelWallE.Language;

/// <summary>
/// * Thrown by the Semantic Analyzer when the code is syntactically correct but logically invalid.
/// </summary>
public class SemanticException : PixelWallEException
{
    /// <summary>
    /// The name of the identifier (variable,label, etc.) that caused the error.
    /// </summary>
    public string? OffendingIdentifier { get; }

    public SemanticException(string message, CodeLocation location, string? offendingIdentifier) : base(message, location)
    {
        OffendingIdentifier = offendingIdentifier;
    }

   

    public static SemanticException UndeclaredVariable(string variableName, CodeLocation location)
    {
        string message = $"Semantic Error: Use of undeclared variable '{variableName}'. Error at line{location.Line}, column {location.Column}";
        return new SemanticException(message, location, variableName);
    }

    public static SemanticException UndeclaredColor(string colorName, CodeLocation location)
    {
        string message = $"Semantic Error: Use of undeclared color '{colorName}'. Error at line{location.Line}, column {location.Column}";
        return new SemanticException(message, location, colorName);
    }
    public static SemanticException LabelNotFound(string labelName, CodeLocation location)
    {
        string message = $"Semantic Error: Label '{labelName}' not found in the current scope. GoTo declared at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, labelName);
    }

    public static SemanticException TypeMismatch(string operationContext, ExpressionType expectedType, ExpressionType actualType, CodeLocation location)
    {
        string message = $"Semantic Error: Type mismatch in '{operationContext}'.  Expected '{expectedType}', but found '{actualType}'. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, $"{expectedType}/{actualType}");
    }

    public static SemanticException InvalidOperation(string operation, ExpressionType operandType, CodeLocation location)
    {
        string message = $"Semantic Error: Operation '{operation}' is not defined for type '{operandType}'. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, operandType.ToString());
    }
    
    public static SemanticException IncorrectArgumentCount(string functionName,int expectedArgs, int actualArgs, CodeLocation location)
    {
        string message = $"Semantic Error: Function '{functionName}' expects '{expectedArgs}' argument(s) but was called with {actualArgs}. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, functionName);
    }

}