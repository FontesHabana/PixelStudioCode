using System;

namespace PixelWallE.Language;
/// <summary>
/// Thrown by the Interpreter during program execution when an operation is invalid based on the current program state.
/// </summary>
class RuntimeException : PixelWallEException
{
    /// <summary>
    /// Gets the name of the command that was being executed when the error occurred.
    /// </summary>
    public string? CommandName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="commandName">The name of the command that was being executed when the error occurred.</param>
    public RuntimeException(string message, CodeLocation location, string? commandName = null) : base(message, location)
    {
        CommandName = commandName;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="commandName">The name of the command that was being executed when the error occurred.</param>
    public RuntimeException(string message, CodeLocation location, Exception innerException, string? commandName = null) : base(message, location, innerException)
    {
        CommandName = commandName;
    }


    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for a division by zero error.
    /// </summary>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException DivisionByZero(CodeLocation location)
    {
        string message = $"Runtime Error: Division by zero encountered during expression evaluation at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for a zero power zero error.
    /// </summary>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException ZeroPowerZero(CodeLocation location)
    {
        string message = $"Runtime Error: Undefined operation 0**0. Error at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for a zero modulo zero error.
    /// </summary>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException ZeroModuloZero(CodeLocation location)
    {
        string message = $"Runtime Error: Undefined operation 0%0. Error at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for an argument that must be positive.
    /// </summary>
    /// <param name="argumentName">The name of the argument.</param>
    /// <param name="opffendingValue">The offending value.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException ArgumentMostBePositive(string argumentName, int opffendingValue, CodeLocation location)
    {
        string message = $"Runtime Error: Argument '{argumentName}' must be positive number(grater than 0). Received {opffendingValue}. Error at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for a position that is out of bounds.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="commandName">The name of the command that was being executed when the error occurred.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException PositionOutOfBounds(int x, int y, string commandName, CodeLocation location)
    {
        string message = $"Runtime Error in command '{commandName}': Position ({x},{y}) is outside the canvas boundaries. Error occurred at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for invalid direction coordinates.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="commandName">The name of the command that was being executed when the error occurred.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException InvalidDirectionCoordinates(int x, int y, string commandName, CodeLocation location)
    {
        string message = $"Runtime Error in command '{commandName}': Invalid coordinates ({x},{y}). Values must be (-1,0,1). Error occurred at line {location.Line}, column {location.Column}.";
        return new RuntimeException(message, location);
    }



    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for an index that is out of range.
    /// </summary>
    /// <param name="index">The index that is out of range.</param>
    /// <param name="collectionSize">The size of the collection.</param>
    /// <param name="commandName">The name of the command that was being executed when the error occurred.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>

    public static RuntimeException IndexOutOfRange(int index, int collectionSize, string commandName, CodeLocation location)
    {
        string message = $"Runtime Error in command '{commandName}': Index '{index}'  was out of range. It must be non-negative and less than the size of the collection ({collectionSize}).";
        return new RuntimeException(message, location);
    }



    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for general errors found within a specified path.
    /// </summary>
    /// <param name="path">The path where the errors were found (e.g., file path).</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException ErrorsInPath(string path)
    {
        string message = $"Runtime Error: Multiple errors detected in '{path}'. Please review the file for issues.";
        return new RuntimeException(message, new CodeLocation());
    }

    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for an exception encountered during a 'find' operation within a specified path.
    /// </summary>
    /// <param name="path">The path where the find operation failed.</param>
    /// <param name="innerException">The actual exception that caused the find operation to fail (optional).</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException FindExceptionInPath(string path, Exception? innerException = null)
    {
        string message = $"Runtime Error: An exception occurred while trying to find or access resources at '{path}'.";
        // Using a default CodeLocation as the error pertains to the path itself rather than a specific code line.
        return innerException != null ? new RuntimeException(message, new CodeLocation(), innerException) : new RuntimeException(message, new CodeLocation());
    }


    /// <summary>
    /// Creates a new <see cref="RuntimeException"/> for an undeclared color.
    /// </summary>
    /// <param name="colorName">The name of the undeclared color.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="RuntimeException"/> instance.</returns>
    public static RuntimeException UndeclaredColor(string colorName, CodeLocation location)
    {
        string message = $"Runtime Error: Use of undeclared color '{colorName}'. Error at line{location.Line}, column {location.Column}";
        return new RuntimeException(message, location, colorName);
    }
}