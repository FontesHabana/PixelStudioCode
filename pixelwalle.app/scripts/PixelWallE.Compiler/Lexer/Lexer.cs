using System.Diagnostics;
using PixelWallE.Language.Tokens;
using PixelWallE.Language;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Specialized;
namespace PixelWallE.Language.Lexer;


/// <summary>
/// The <c>Lexer</c> class is responsible for lexical analysis of source code,
/// converting a string of code into a sequence of tokens for further processing by a compiler or interpreter.
/// It supports registration and recognition of operators, keywords, and text delimiters,
/// and provides methods to tokenize input code while handling comments, whitespace, identifiers, numbers, and errors.
/// </summary>
public class Lexer
{

    Dictionary<string, TokenType> operators = new Dictionary<string, TokenType>();
    Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();
    Dictionary<string, string> texts = new Dictionary<string, string>();


    private List<string> orderedOperatorKeys;
    private List<string> orderedTextKeys;

    /// <summary>
    /// Orders the operator and text delimiter keys by descending length to ensure
    /// longest match first during tokenization.
    /// </summary>
    public void OrderedDictionary()
    {
        orderedOperatorKeys = operators.Keys.OrderByDescending(k => k.Length).ToList();
        orderedTextKeys = texts.Keys.OrderByDescending(k => k.Length).ToList();
    }


    /// <summary>
    /// Gets the collection of registered keyword strings.
    /// </summary>
    public IEnumerable<string> Keywords { get { return keywords.Keys; } }

    /// <summary>
    /// Registers an operator symbol and associates it with a token type.
    /// </summary>
    /// <param name="op">The operator symbol.</param>
    /// <param name="tokenValue">The token type associated with the operator.</param>
    public void RegisterOperator(string op, TokenType tokenValue)
    {
        this.operators[op] = tokenValue;
    }

    /// <summary>
    /// Registers a keyword and associates it with a token type.
    /// </summary>
    /// <param name="keyword">The keyword string.</param>
    /// <param name="tokenValue">The token type associated with the keyword.</param>
    public void RegisterKeyword(string keyword, TokenType tokenValue)
    {
        this.keywords[keyword] = tokenValue;
    }

    /// <summary>
    /// Registers a text literal delimiter pair.
    /// </summary>
    /// <param name="start">The starting delimiter.</param>
    /// <param name="end">The ending delimiter.</param>
    public void RegisterText(string start, string end)
    {
        this.texts[start] = end;
    }


    /// <summary>
    /// Attempts to match an operator symbol at the current position in the stream.
    /// If matched, adds the corresponding token to the token list.
    /// </summary>
    /// <param name="stream">The token reader stream.</param>
    /// <param name="tokens">The list of tokens to update.</param>
    /// <returns>True if an operator was matched; otherwise, false.</returns>
    private bool MatchSymbol(TokenReader stream, List<Token> tokens)
    {
        foreach (var op in orderedOperatorKeys)
            if (stream.Match(op))
            {
                tokens.Add(new Token(operators[op], op, null, stream.Location));
                return true;
            }
        return false;
    }

    /// <summary>
    /// Attempts to match a text literal at the current position in the stream.
    /// If matched, adds the corresponding string token to the token list and updates errors if needed.
    /// </summary>
    /// <param name="stream">The token reader stream.</param>
    /// <param name="tokens">The list of tokens to update.</param>
    /// <param name="errors">The list of lexical errors to update.</param>
    /// <returns>True if a text literal was matched; otherwise, false.</returns>
    private bool MatchText(TokenReader stream, List<Token> tokens, List<PixelWallEException> errors)
    {
        foreach (var start in orderedTextKeys)
        {
            string text;
            if (stream.Match(start))
            {
                if (!stream.ReadUntil(texts[start], out text))
                {
                    // errors.Add(new CompilingError(stream.Location, ErrorCode.Expected, texts[start])); 
                }


                //Revisar la construcción del token creo que tienes propiedades innecesarias
                tokens.Add(new Token(TokenType.STRING, text, text, stream.Location));
                return true;
            }
        }
        return false;
    }



    /// <summary>
    /// Tokenizes the input code, returning all tokens and populating the errors list with any lexical errors detected.
    /// </summary>
    /// <param name="fileName">The name of the source file.</param>
    /// <param name="code">The source code to tokenize.</param>
    /// <param name="errors">The list to populate with lexical errors.</param>
    /// <returns>An enumerable of tokens parsed from the code.</returns>
    public IEnumerable<Token> GetTokens(string fileName, string code, List<PixelWallEException> errors)
    {
        List<Token> tokens = new List<Token>();

        TokenReader stream = new TokenReader(fileName, code);

        while (!stream.EOF)
        {
            string value;
            if (stream.ReadComment())
            {
                continue;
            }
            else if (stream.ReadEOL())
            {
                tokens.Add(new Token(TokenType.EOL, "\\n", null, stream.Location));
                stream.ActionReadEOL();
                continue;
            }

            else if (stream.ReadWhiteSpace()) continue;

            else if (stream.ReadID(out value))
            {
                if (keywords.ContainsKey(value))
                    tokens.Add(new Token(keywords[value], value, null, stream.Location));
                else
                    tokens.Add(new Token(TokenType.IDENTIFIER, value, null, stream.Location));
                continue;
            }

            if (stream.ReadNumber(out value))
            {
                tokens.Add(new Token(TokenType.NUMBER, value, int.Parse(value), stream.Location));
                continue;
            }

            if (MatchText(stream, tokens, errors))
                continue;

            if (MatchSymbol(stream, tokens))
                continue;

            var unexpectedCharacter = stream.ReadAny();
            errors.Add(LexicalException.UnexpectedCharacter(unexpectedCharacter, stream.Location));
        }

        tokens.Add(new Token(TokenType.EOF, "\\0", null, stream.Location));
        return tokens;
    }






    /// <summary>
    /// The <c>TokenReader</c> class provides utilities for reading and parsing characters from the source code,
    /// supporting operations such as matching, peeking, reading identifiers, numbers, comments, and handling code locations.
    /// </summary>
    class TokenReader
    {
        string FileName;
        string code;
        private int start;
        private int current;
        private int line;


        /// <summary>
        /// Initializes a new instance of the <see cref="TokenReader"/> class.
        /// </summary>
        /// <param name="fileName">The name of the source file.</param>
        /// <param name="code">The source code to read.</param>
        public TokenReader(string fileName, string code)
        {
            this.FileName = fileName;
            this.code = code;
            this.current = 0;
            this.line = 1;
            this.start = -1;
        }

        /// <summary>
        /// Gets the current code location (file, line, column).
        /// </summary>
        public CodeLocation Location
        {
            get
            {
                return new CodeLocation
                {
                    File = FileName,
                    Line = line,
                    Column = current - start - 1
                };
            }
        }

        /// <summary>
        /// Gets a value indicating whether the end of the file has been reached.
        /// </summary>
        public bool EOF
        {
            get { return current >= code.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether the current character is end-of-line or end-of-file.
        /// </summary>
        public bool EOL
        {
            get { return EOF || code[current] == '\n'; }
        }

        /// <summary>
        /// Peeks at the next character in the code without advancing the position.
        /// </summary>
        /// <returns>The next character.</returns>
        private char Peek()
        {
            if (current < 0 || current >= code.Length)

                throw new InvalidOperationException();

            return code[current];
        }

        /// <summary>
        /// Determines whether the code at the current position starts with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to check for.</param>
        /// <returns>True if the code continues with the prefix; otherwise, false.</returns>
        public bool ContinuesWith(string prefix)
        {
            if (current + prefix.Length > code.Length)
                return false;
            for (int i = 0; i < prefix.Length; i++)
                if (code[current + i] != prefix[i])
                    return false;
            return true;
        }

        /// <summary>
        /// If the code at the current position starts with the specified prefix, advances the position.
        /// </summary>
        /// <param name="prefix">The prefix to match.</param>
        /// <returns>True if the prefix was matched and position advanced; otherwise, false.</returns>
        public bool Match(string prefix)
        {
            if (ContinuesWith(prefix))
            {
                current += prefix.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a character is valid for an identifier, depending on its position.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <param name="begining">True if checking the first character of the identifier.</param>
        /// <returns>True if the character is valid; otherwise, false.</returns>
        public bool ValidIdCharacter(char c, bool begining)
        {
            return c == '_' || (begining ? char.IsLetter(c) : char.IsLetterOrDigit(c));
        }

        /// <summary>
        /// Reads an identifier from the current position.
        /// </summary>
        /// <param name="id">The identifier read, if any.</param>
        /// <returns>True if an identifier was read; otherwise, false.</returns>
        public bool ReadID(out string id)
        {
            id = "";
            if (Peek() == '_')
                return id.Length > 0;

            while (!EOL && ValidIdCharacter(Peek(), id.Length == 0))
                id += ReadAny();
            return id.Length > 0;
        }

        /// <summary>
        /// Reads a int number from the current position.
        /// </summary>
        /// <param name="number">The number read, if any.</param>
        /// <returns>True if a number was read; otherwise, false.</returns>
        public bool ReadNumber(out string number)
        {
            number = "";
            while (!EOL && char.IsDigit(Peek()))
                number += ReadAny();

            if (number.Length == 0)
                return false;


            return number.Length > 0;
        }

        /// <summary>
        /// Reads characters until the specified end delimiter is found.
        /// </summary>
        /// <param name="end">The ending delimiter.</param>
        /// <param name="text">The text read, if any.</param>
        /// <returns>True if the end delimiter was found; otherwise, false.</returns>
        public bool ReadUntil(string end, out string text)
        {
            text = "";
            while (!Match(end))
            {
                if (EOL || EOF)
                    return false;
                text += ReadAny();
            }
            return true;
        }

        /// <summary>
        /// Reads and skips whitespace characters.
        /// </summary>
        /// <returns>True if whitespace was read; otherwise, false.</returns>
        public bool ReadWhiteSpace()
        {
            if (char.IsWhiteSpace(Peek()))
            {
                ReadAny();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the current character is an end-of-line character.
        /// </summary>
        /// <returns>True if end-of-line was read; otherwise, false.</returns>
        public bool ReadEOL()
        {
            if (Peek() == '\n')
            {

                return true;
            }
            return false;
        }

        /// <summary>
        /// Advances the position after reading an end-of-line character, updating line and start position.
        /// </summary>
        public void ActionReadEOL()
        {
            line++;
            start = current++;
        }

        /// <summary>
        /// Reads and skips a comment starting with '#'.
        /// </summary>
        /// <returns>True if a comment was read; otherwise, false.</returns>
        public bool ReadComment()
        {
            if (Peek() == '#')
            {
                string text = "";
                while (!EOL || !EOF)
                {
                    text += ReadAny();
                    if (EOL || EOF)
                    {
                        break;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the next character from the code, advancing the position.
        /// Handles line and start position updates for end-of-line.
        /// </summary>
        /// <returns>The character read, or '\0' if end-of-file.</returns>
        public char ReadAny()
        {
            if (EOF)
                return '\0';

            if (EOL)
            {
                line++;
                start = current;
            }
            return code[current++];
        }
    }

}

