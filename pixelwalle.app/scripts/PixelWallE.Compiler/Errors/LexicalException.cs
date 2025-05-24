using System;

namespace PixelWallE.Language;
/// <summary>
/// * Thrown by the Lexer when an invalidsequence of characters is encountred
/// * that cannt be formed into a valid token.
/// </summary>

class LexicalException : PixelWallEException
{
    public LexicalException(string message, CodeLocation location) : base(message, location) { }
    public LexicalException(string message, CodeLocation location, Exception innerException) : base(message, location, innerException) { }

    public static LexicalException UnexpectedCharacter(char character, CodeLocation location)
    {
        string message = $"Lexical Error: Unexpected character '{character}' found at line {location.Line}, column {location.Column}.";
        return new LexicalException(message, location); 
         
    }
}