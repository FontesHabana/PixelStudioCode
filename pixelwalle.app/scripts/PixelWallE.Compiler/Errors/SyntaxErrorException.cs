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
    public Token? FoundToken{ get; }

    /// <summary>
    /// A description of what token(s) were expected.
    /// </summary>
    public Token? ExpectedToken{ get; }


    public SyntaxException(string message, CodeLocation location, Token? foundToken = null, Token? expectedToken = null) : base(message, location)
    {
        FoundToken = foundToken;
        ExpectedToken = expectedToken;    
    }
   
    public static SyntaxException UnexpectedToken(Token found,Token expected, CodeLocation location)
    {
        string message = $"Syntax Error: Unexpected token '{found}'. Expected {expected}  at line {location.Line}, column {location.Column}.";
        return new SyntaxException(message, location, found, expected);

    }
}