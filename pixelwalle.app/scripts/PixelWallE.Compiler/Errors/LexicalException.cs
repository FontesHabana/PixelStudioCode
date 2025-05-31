using System;

namespace PixelWallE.Language;
/// <summary>
/// Thrown by the Lexer when an invalid sequence of characters is encountered
/// that cannot be formed into a valid token.
/// </summary>
class LexicalException : PixelWallEException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LexicalException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    public LexicalException(string message, CodeLocation location) : base(message, location) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LexicalException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public LexicalException(string message, CodeLocation location, Exception innerException) : base(message, location, innerException) { }

    /// <summary>
    /// Creates a new <see cref="LexicalException"/> for an unexpected character.
    /// </summary>
    /// <param name="character">The unexpected character.</param>
    /// <param name="location">The location in the code where the character was found.</param>
    /// <returns>A new <see cref="LexicalException"/> instance.</returns>
    public static LexicalException UnexpectedCharacter(char character, CodeLocation location)
    {
        string message = $"Lexical Error: Unexpected character '{character}' found at line {location.Line}, column {location.Column}.";
        return new LexicalException(message, location); 
         
    }
}