using System.Dynamic;

namespace PixelWallE.Language.Tokens;

/// <summary>
/// Represents a lexical token produced by the lexer.
/// A token contains information about its type, textual value,
/// optional literal value, and its location in the source code.
/// </summary>
public class Token
{
    /// <summary>
    /// Gets the type of the token.
    /// </summary>
    public TokenType Type { get; private set; }

    /// <summary>
    /// Gets the textual value of the token as it appears in the source code.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets the literal value associated with the token, if applicable.
    /// For example, for a numeric token, this could be its numeric value.
    /// </summary>
    public object? Literal { get; private set; }

    /// <summary>
    /// Gets the location of the token in the source code, including line and column.
    /// </summary>
    public CodeLocation Location { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="type">The type of the token.</param>
    /// <param name="value">The textual value of the token.</param>
    /// <param name="literal">The literal value associated with the token, or null if not applicable.</param>
    /// <param name="location">The location of the token in the source code.</param>
    public Token(TokenType type, string value, object? literal, CodeLocation location)
    {
        Type = type;
        Value = value;
        Literal = literal;
        Location = location;
    }

    /// <summary>
    /// Returns a string representation of the token, including its type, value, and location.
    /// </summary>
    /// <returns>A string describing the token.</returns>
    public override string ToString()
    {
        return $" {Type} {Value}  fila  {Location.Line} columna {Location.Column}";
    }
}

