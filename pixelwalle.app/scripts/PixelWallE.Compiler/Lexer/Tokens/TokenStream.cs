namespace PixelWallE.Language.Tokens;

using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Provides functionality to operate over a list of tokens, allowing navigation,
/// validation, and consumption of tokens during lexical or syntactic analysis.
/// </summary>
public class TokenStream
{
    /// <summary>
    /// The list of tokens managed by this stream.
    /// </summary>
    public List<Token> tokens;

    /// <summary>
    /// Index representing the current token for parsing operations.
    /// </summary>
    public int index;

    /// <summary>
    /// Gets the current iteration index.
    /// </summary>
    public int Position { get { return index; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenStream"/> class with the specified tokens.
    /// </summary>
    /// <param name="tokens">The collection of tokens to operate on.</param>
    public TokenStream(IEnumerable<Token> tokens)
    {
        this.tokens = new List<Token>(tokens);
        index  = 0;
        index=0;
    }

    /// <summary>
    /// Gets a value indicating whether the stream is at the last token.
    /// </summary>
    public bool IsAtStreamEnd => index == tokens.Count - 1;

    /// <summary>
    /// Determines whether the current token is the end-of-file (EOF) token.
    /// </summary>
    /// <returns>True if the current token is EOF; otherwise, false.</returns>
    public bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    /// <summary>
    /// Returns the current token in the stream.
    /// </summary>
    /// <returns>The current <see cref="Token"/>.</returns>
    public Token Peek()
    {
        Godot.GD.Print(index);
         return tokens[index];
    }

    /// <summary>
    /// Attempts to match the current token against a list of token types.
    /// Advances the stream if a match is found.
    /// </summary>
    /// <param name="types">A list of token types to match against.</param>
    /// <returns>True if a match is found and the stream is advanced; otherwise, false.</returns>
    public bool Match(List<TokenType> types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the current token matches the specified token type.
    /// </summary>
    /// <param name="type">The token type to check.</param>
    /// <returns>True if the current token matches the type; otherwise, false.</returns>
    public bool Check(TokenType type)
    {
       // if (IsAtStreamEnd) return false;
        return Peek().Type==type;
    }

    /// <summary>
    /// Consumes the current token if it matches the specified type; otherwise, throws a syntax exception.
    /// </summary>
    /// <param name="type">The expected token type.</param>
    /// <param name="expected">A description of the expected token for error reporting.</param>
    /// <returns>The consumed <see cref="Token"/> if successful.</returns>
    /// <exception cref="SyntaxException">Thrown if the current token does not match the expected type.</exception>
    public Token? Consume(TokenType type, string expected)
    {
         if (Check(type)) return Advance();
        throw SyntaxException.UnexpectedToken(type.ToString(), expected, Peek().Location);
    }

    /// <summary>
    /// Advances the stream until an end-of-line (EOL) token is found or the end of the stream is reached.
    /// Used for error recovery and synchronization.
    /// </summary>
    public void Synchronize(TokenType type=TokenType.EOL)
    {
        //Advance();
        while (!IsAtEnd())
        {    
            if (Peek().Type == type)
            {
                Advance();
                return;
            }
            Advance();
        }
    }

    /// <summary>
    /// Advances to the next token in the stream if not at the end.
    /// </summary>
    /// <returns>The previous <see cref="Token"/> before advancing.</returns>
    public Token Advance()
    {
        if (!IsAtStreamEnd) index++;
        
        return Previous();
    }

    /// <summary>
    /// Returns the token immediately before the current token.
    /// </summary>
    /// <returns>The previous <see cref="Token"/>.</returns>
    public Token Previous()
    {
        return tokens[index-1];
    }

    /// <summary>
    /// Moves the current index backward by the specified number of tokens.
    /// </summary>
    /// <param name="k">The number of tokens to move back.</param>
    public void MoveBack(int k)
    {
        index -= k;
    }

   


}

