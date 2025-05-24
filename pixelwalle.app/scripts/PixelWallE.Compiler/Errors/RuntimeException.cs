using System;

namespace PixelWallE.Language;
/// <summary>
/// * Thrown by the Interpreter during program execution when an operation based on the current program state.
/// </summary>

class RuntimeException : PixelWallEException
{
    /// <summary>
    /// The name of the command that was being executed when the error occurred.
    /// </summary>
    public string? CommandName { get; }

    public RuntimeException(string message, CodeLocation location, string? commandName = null) : base(message, location)
    {
        CommandName = commandName;
    }
    public RuntimeException(string message, CodeLocation location, Exception innerException, string? commandName = null) : base(message, location, innerException)
    {
        CommandName = commandName;
    }



    public static RuntimeException DivisionByZero(CodeLocation location)
    {
        string message = $"Runtime Error: Division by zero encountered during expression evaluation at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    public static RuntimeException ZeroPowerZero(CodeLocation location)
    {
        string message = $"Runtime Error: Undefined operation 0**0. Error at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    public static RuntimeException ZeroModuloZero(CodeLocation location)
    {
        string message = $"Runtime Error: Undefined operation 0%0. Error at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    public static RuntimeException ArgumentMostBePositive(string argumentName, int opffendingValue, CodeLocation location)
    {
        string message = $"Runtime Error: Argument '(argumentName)' must be positive number(grater than 0). Received {opffendingValue}. Error at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    public static RuntimeException PositionOutOfBounds(int x, int y, string commandName, CodeLocation location)
    {
        string message = $"Runtime Error in command '{commandName}': Position ({x},{y}) is outside the canvas boundaries. Error occurred at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    public static RuntimeException InvalidDirectionCoordinates(int x, int y, string commandName, CodeLocation location)
    {
        string message = $"Runtime Error in command '{commandName}': Invlid coordinates ({x},{y}). Values must be (-1,0,1). Error occurred at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

}