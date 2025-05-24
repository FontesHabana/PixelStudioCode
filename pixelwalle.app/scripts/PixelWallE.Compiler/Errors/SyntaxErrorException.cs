using System;
using PixelWallE.Language.Tokens;
namespace PixelWallE.Language;

/// <summary>
/// * Thrown by the Parser when the sequence of tokens violates the grammatical rules of the language
/// </summary>
public class SyntaxException : PixelWallEException
{
    /// <summary>
    /// The token that wasd  found where the error occurred.
    /// </summary>
    public string? FoundToken { get; }


    /// <summary>
    /// A description of what token(s) were expected.
    /// </summary>
    public string? ExpectedToken { get; }


    public SyntaxException(string message, CodeLocation location, string? foundToken = null, string? expectedToken = null) : base(message, location)
    {
        FoundToken = foundToken;
        ExpectedToken = expectedToken;
    }

    public static SyntaxException UnexpectedToken(string found, string expected, CodeLocation location)
    {
        string message = $"Syntax Error: Unexpected token '{found}'. Expected {expected}  at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location, found, expected);

    }

    public static SyntaxException SpawnMisplaced(CodeLocation location)
    {
        string message = $"Syntax Error: The 'Spawn' command must be the first command int the program. Error at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }

    public static SyntaxException DuplicateSpawn(CodeLocation location)
    {
        string message = $"Syntax Error: The 'Spawn' command can only appear once in the program. Duplicate 'Spawn' at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }
    
    public static SyntaxException ExpectedNewLineAfterCommand(string offendingTokenValue, CodeLocation location)
    {
        string message = $"Syntax Error: Expected a new line or end of file after command or a label, but found '{offendingTokenValue}'. Error at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location);
    }
}