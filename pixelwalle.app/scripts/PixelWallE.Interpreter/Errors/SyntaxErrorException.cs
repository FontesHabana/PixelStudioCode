using System;
using PixelWallE.Language.Tokens;
namespace PixelWallE.Language;

/// <summary>
/// Thrown by the Parser when the sequence of tokens violates the grammatical rules of the language.
/// </summary>
public class SyntaxException : PixelWallEException
{
    /// <summary>
    /// Gets the token that was found where the error occurred.
    /// </summary>
    public string? FoundToken { get; }

    /// <summary>
    /// Gets a description of what token(s) were expected.
    /// </summary>
    public string? ExpectedToken { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="foundToken">The token that was found where the error occurred.</param>
    /// <param name="expectedToken">A description of what token(s) were expected.</param>
    public SyntaxException(string message, CodeLocation location, string? foundToken = null, string? expectedToken = null) : base(message, location)
    {
        FoundToken = foundToken;
        ExpectedToken = expectedToken;
    }

    /// <summary>
    /// Creates a new <see cref="SyntaxException"/> for an unexpected token.
    /// </summary>
    /// <param name="found">The token that was found.</param>
    /// <param name="expected">A description of what token(s) were expected.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SyntaxException"/> instance.</returns>
    public static SyntaxException UnexpectedToken(string found, string expected, CodeLocation location)
    {
        string message = $"Syntax Error: Unexpected token '{found}'. Expected {expected}  at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location, found, expected);

    }

    /// <summary>
    /// Creates a new <see cref="SyntaxException"/> for a misplaced 'Spawn' command.
    /// </summary>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SyntaxException"/> instance.</returns>
    public static SyntaxException SpawnMisplaced(CodeLocation location)
    {
        string message = $"Syntax Error: The 'Spawn' command must be the first command int the program. Error at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="SyntaxException"/> for a duplicate 'Spawn' command.
    /// </summary>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SyntaxException"/> instance.</returns>
    public static SyntaxException DuplicateSpawn(CodeLocation location)
    {
        string message = $"Syntax Error: The 'Spawn' command can only appear once in the program and  the 'Spawn' command must be the first command int the program (including labels). Error in 'Spawn' at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="SyntaxException"/> for a duplicate label.
    /// </summary>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="label">The name of the duplicate label.</param>
    /// <returns>A new <see cref="SyntaxException"/> instance.</returns>
    public static SyntaxException DuplicateLabel(CodeLocation location, string label)
    {
        string message = $"Syntax Error: The label {label} has already exist. Error at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }

    /// <summary>
    /// Creates a new <see cref="SyntaxException"/> for a missing new line after a command.
    /// </summary>
    /// <param name="offendingTokenValue">The value of the offending token.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <returns>A new <see cref="SyntaxException"/> instance.</returns>
    public static SyntaxException ExpectedNewLineAfterCommand(string offendingTokenValue, CodeLocation location)
    {
        string message = $"Syntax Error: Expected a new line or end of file after command or label, but found '{offendingTokenValue}'. Error at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }
    

}