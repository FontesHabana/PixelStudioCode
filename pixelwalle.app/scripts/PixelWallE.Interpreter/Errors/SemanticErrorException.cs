using System;
using System.Reflection.Emit;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
namespace PixelWallE.Language;

/// <summary>
/// Thrown by the Semantic Analyzer when the code is syntactically correct but logically invalid.
/// </summary>
public class SemanticException : PixelWallEException
{
    /// <summary>
    /// Gets the name of the identifier (variable, label, etc.) that caused the error.
    /// </summary>
    public string? OffendingIdentifier { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="offendingIdentifier">The name of the identifier that caused the error.</param>
    public SemanticException(string message, CodeLocation location, string? offendingIdentifier) : base(message, location)
    {
        OffendingIdentifier = offendingIdentifier;
    }


    /// <summary>
    /// Creates a new <see cref="SemanticException"/> for an undeclared variable.
    /// </summary>
    /// <param name="variableName">The name of the undeclared variable.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SemanticException"/> instance.</returns>
    public static SemanticException UndeclaredVariable(string variableName, CodeLocation location)
    {
        string message = $"Semantic Error: Use of undeclared variable '{variableName}'. Error at line{location.Line}, column {location.Column}";
        return new SemanticException(message, location, variableName);
    }

    /// <summary>
    /// Creates a new <see cref="SemanticException"/> for an undeclared color.
    /// </summary>
    /// <param name="colorName">The name of the undeclared color.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SemanticException"/> instance.</returns>
    public static SemanticException UndeclaredColor(string colorName, CodeLocation location)
    {
        string message = $"Semantic Error: Use of undeclared color '{colorName}'. Error at line{location.Line}, column {location.Column}";
        return new SemanticException(message, location, colorName);
    }
    /// <summary>
    /// Creates a new <see cref="SemanticException"/> for a label that was not found.
    /// </summary>
    /// <param name="labelName">The name of the label that was not found.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SemanticException"/> instance.</returns>
    public static SemanticException LabelNotFound(string labelName, CodeLocation location)
    {
        string message = $"Semantic Error: Label '{labelName}' not found in the current scope. GoTo declared at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, labelName);
    }

    /// <summary>
    /// Creates a new <see cref="SemanticException"/> for a type mismatch.
    /// </summary>
    /// <param name="operationContext">The context of the operation where the type mismatch occurred.</param>
    /// <param name="expectedType">The expected type.</param>
    /// <param name="actualType">The actual type.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SemanticException"/> instance.</returns>
    public static SemanticException TypeMismatch(string operationContext, ExpressionType expectedType, ExpressionType actualType, CodeLocation location)
    {
        string message = $"Semantic Error: Type mismatch in '{operationContext}'.  Expected '{expectedType}', but found '{actualType}'. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, $"{expectedType}/{actualType}");
    }

    /// <summary>
    /// Creates a new <see cref="SemanticException"/> for an invalid operation.
    /// </summary>
    /// <param name="operation">The invalid operation.</param>
    /// <param name="operandType">The type of the operand.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SemanticException"/> instance.</returns>
    public static SemanticException InvalidOperation(string operation, ExpressionType operandType,ExpressionType operandType2, CodeLocation location)
    {
        string message = $"Semantic Error: Operation '{operation}' is not defined for type '{operandType}' with type '{operandType2}'. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, operandType.ToString());
    }

    public static SemanticException InvalidOperation(string operation, ExpressionType operandType, CodeLocation location)
    {
        string message = $"Semantic Error: Operation '{operation}' is not defined for type '{operandType}'. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, operandType.ToString());
    }

    /// <summary>
    /// Creates a new <see cref="SemanticException"/> for an incorrect number of arguments.
    /// </summary>
    /// <param name="functionName">The name of the function.</param>
    /// <param name="expectedArgs">The expected number of arguments.</param>
    /// <param name="actualArgs">The actual number of arguments.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SemanticException"/> instance.</returns>
    public static SemanticException IncorrectArgumentCount(string functionName, int expectedArgs, int actualArgs, CodeLocation location)
    {
        string message = $"Semantic Error: Function '{functionName}' expects '{expectedArgs}' argument(s) but was called with {actualArgs}. Error at line {location.Line}, column {location.Column}";
        return new SemanticException(message, location, functionName);
    }
    

   
}