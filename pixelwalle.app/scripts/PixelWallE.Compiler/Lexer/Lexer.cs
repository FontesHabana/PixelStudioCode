using System.Diagnostics;
using PixelWallE.Language.Tokens;

using System.Collections.Generic;
using System.Linq;
using System;
namespace PixelWallE.Language.Lexer;


 public class Lexer{
    
    Dictionary<string, TokenType> operators = new Dictionary<string, TokenType>();
    Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();
    Dictionary<string, string> texts = new Dictionary<string, string>();

    public IEnumerable<string> Keywords { get { return keywords.Keys; } }

    //public List<Token> tokens = new List<Token>();
     /* Associates an operator symbol with the correspondent token value */
        public void RegisterOperator(string op, TokenType tokenValue)
        {
            this.operators[op] = tokenValue;
        }

        /* Associates a keyword with the correspondent token value */
        public void RegisterKeyword(string keyword, TokenType tokenValue)
        {
            this.keywords[keyword] = tokenValue;
        }

        /* Associates a Text literal starting delimiter with their correspondent ending delimiter */
        public void RegisterText(string start, string end)
        {
            this.texts[start] = end;
        }


         /* Matches a new symbol in the code and read it from the string. The new symbol is added to the token list as an operator. */
        private bool MatchSymbol(TokenReader stream, List<Token> tokens)
        {
            foreach (var op in operators.Keys.OrderByDescending(k => k.Length))
                if (stream.Match(op))
                {
                    tokens.Add(new Token(operators[op], op ,null, stream.Location));
                    return true;
                }
            return false;
        }

        /* Matches a Text part in the code and read the literal from the stream.
        The tokens list is updated with the new string token and errors is updated with new errors if detected. */
        private bool MatchText (TokenReader stream, List<Token> tokens, List<CompilingError> errors)
        {
            foreach (var start in texts.Keys.OrderByDescending(k=>k.Length))
            {
                string text;
                if (stream.Match(start))
                {
                    if (!stream.ReadUntil(texts[start], out text)){
                        errors.Add(new CompilingError(stream.Location, ErrorCode.Expected, texts[start])); 
                    }
                       

                        //Revisar la construcción del token creo que tienes propiedades innecesarias
                    tokens.Add(new Token(TokenType.STRING, text, text, stream.Location));
                    return true;
                }
            }
            return false;
        }
        


  /* Returns all tokens read from the code and populate the errors list with all lexical errors detected. */
        public IEnumerable<Token> GetTokens(string fileName, string code, List<CompilingError> errors)
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
                    if (stream.ReadEOL())
                      {  
                        tokens.Add(new Token(TokenType.EOL, "\\n",null, stream.Location));
                        stream.ActionReadEOL();
                        continue;
                       }

                if (stream.ReadWhiteSpace())
                    continue;

                

                if (stream.ReadID(out value))
                {
                    if (keywords.ContainsKey(value))
                        tokens.Add(new Token(keywords[value], value ,null, stream.Location));
                    else
                        tokens.Add(new Token(TokenType.IDENTIFIER, value, null,stream.Location));
                    continue;
                }

                if(stream.ReadNumber(out value))
                {
                    int d;
                    if (!int.TryParse(value, out d))
                        errors.Add(new CompilingError(stream.Location, ErrorCode.Invalid, "Number format"));
                    tokens.Add(new Token(TokenType.NUMBER, value, d, stream.Location));
                    continue;
                }

                if (MatchText(stream, tokens, errors))
                    continue;
                
                //System.Console.WriteLine("Trate de matchear simbolo");
                if (MatchSymbol(stream, tokens))
                    continue;
                
              //  System.Console.WriteLine("pasé por aqui");
                var unkOp = stream.ReadAny();
                errors.Add(new CompilingError(stream.Location, ErrorCode.Unknown, unkOp.ToString()));
            }
            tokens.Add(new Token(TokenType.EOF,"\\0", null, stream.Location));
            return tokens;
        }

        /* Allows to read from a string numbers, identifiers and matching some prefix. 
        It has some useful methods to do that */
     
   /* //Posibilidad de convertir estas fjunciones en una clase especifica leer token y el scaner llame estos metodos
    private void scanToken(){
        char c=advance();
        switch (c)
        {   case '(':addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case '[': addToken(TokenType.LEFT_BRACKET); break;
            case ']': addToken(TokenType.RIGHT_BRACKET); break;
            case ',': addToken(TokenType.COMMA); break;
            case '-': addToken(TokenType.MINUS); break;
            case '+': addToken(TokenType.PLUS); break;
            case '/': addToken(TokenType.DIVIDE); break;
            case '*':
            addToken(match('*') ? TokenType.EXPONENTIAL : TokenType.MULTIPLY);
            break;

            case '!':
            addToken(match('=') ? TokenType.NOT_EQUAL : TokenType.NOT);
            break;
            case '=':
            if (match('='))
            {
                addToken(TokenType.EQUAL);
                
            }
            break;
            case '<':
            addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
            break;
            case '>':
            addToken(match('=') ? TokenType.GREATER_EQUAL : match('-')? TokenType.ASSIGNMENT: TokenType.GREATER);
            break;
            case '&':
            if (match('&'))
            {
                addToken(TokenType.EQUAL);
                
            }
            break;
            case '|':
            if (match('|'))
            {
                addToken(TokenType.EQUAL);
               
            }
             break;
           


             
            case '"': stringtype(); break;
            
            //Comentarios
            case '#': while(peek()!='\n' &&!isAtEnd()) advance(); break;
            case ' ':
            case'\r':
            case '\t':
                break;
            case '\n': line++; break;
            default:
            if (Char.IsDigit(c))
            {
                number();
            }
            else if(isAlpha(c)){
                identifier();
            }
            else{
             PWELanguage.error(new CodeLocation(Source, line, current-1),"Unexpected character");
            }
            
            break;
             
        }
    }

*/
/*
    private char advance(){
        current++;
        return Source[current-1];

    }
    private void addToken(TokenType type){
        addToken(type, null);
    }
    private void addToken(TokenType type, Object? literal){
        string text=Source.Substring(start, current);
        tokens.Add(new Token(type,text, literal , new CodeLocation(Source, line, current-1)));
    }

    private bool match(char expected) {
    if (isAtEnd()) return false;
    if (Source[current] != expected) return false;
    current++;
    return true;
    }

   
    
    private void stringtype(){
        while( peek() != '"' && !isAtEnd()){
            if (peek()=='\n' ) line++;
            advance();
        }
        if (isAtEnd())
        {
            PWELanguage.error(new CodeLocation(Source, line, current-1), "Unterminated string.");
            return;
        }
        advance();

        string value=Source.Substring(start+1, current-1);
        addToken(TokenType.STRING, value);
    }
    private void number(){
        while (Char.IsDigit(peek())) advance();
        string value=Source.Substring(start, current);
        addToken(TokenType.NUMBER, int.Parse(value));
    }
    private void identifier(){
        while (isAlphaNumeric(peek())) advance();
        addToken(TokenType.IDENTIFIER);
    }

    private bool isAlpha(char c){
        return( c>='a'&& c<='z')||( c>='A'&& c<='Z')||c=='-';
    }
    private bool isAlphaNumeric(char c){
        return isAlpha(c)||Char.IsDigit(c);
    }

///Falta por añadir caracter de salto de linea y keyword, estudiar el codigo de rodrigo para meter esto mas organizado

*/


 class TokenReader{
    string FileName;
    string code;
    private int start=0;
    private int current=0;
    private int line=1;

     public TokenReader(string fileName, string code)
            {
                this.FileName = fileName;
                this.code = code;
                this.current = 0;
                this.line = 1;
                this.start = -1;
            }

     public CodeLocation Location
       {
        get
        {
           return new CodeLocation
           {
                File = FileName,
                Line = line,
                Column = current-start
             };
        }
       }
        public bool EOF
            {
                get { return current >= code.Length; }
            }

        public bool EOL
            {
                get { return EOF || code[current] == '\n'; }
            }

        /* Peek the next character */
        private char Peek(){
            if (current < 0 || current >= code.Length)
            //Revisar este error
                    throw new InvalidOperationException();

                return code[current];
            }
        
        /*Retorna si a un caracter le sigue una palabra deseada*/
        public bool ContinuesWith(string prefix)
            {
                if (current + prefix.Length > code.Length)
                    return false;
                for (int i = 0; i < prefix.Length; i++)
                    if (code[current + i] != prefix[i])
                        return false;
                return true;
            }
        /*Devuelve y avanza en el codigo si es positiva contiueswith*/
        public bool Match(string prefix)
            {
                if (ContinuesWith(prefix))
                {
                    current += prefix.Length;
                    return true;
                }

                return false;
            }
        /*Devuelve si un caracter es valido en una situación. Utilizado para etiquetas y variables*/
        public bool ValidIdCharacter(char c, bool begining)
            {
                return c == '-' || (begining ? char.IsLetter(c) : char.IsLetterOrDigit(c));
            }
        //Determina si una palabra es un identifier
        public bool ReadID(out string id)
            {
                id = "";
                if (Peek()=='-')
                {
                   return id.Length > 0;
                }
                while (!EOL && ValidIdCharacter(Peek(), id.Length == 0))
                    id += ReadAny();
                return id.Length > 0;
            }
        //Lee número incluyendo decimales
        public bool ReadNumber(out string number)
            {
                number = "";
                while (!EOL && char.IsDigit(Peek()))
                    number += ReadAny();
               
                if (number.Length == 0)
                    return false;


                return number.Length > 0;
            }
        //Revisar uso de esto
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

         public bool ReadWhiteSpace()
            {
                if (char.IsWhiteSpace(Peek()))
                {
                    ReadAny();
                    return true;
                }
                return false;
            }

        public bool ReadEOL()
            { 
                if(Peek()=='\n'){
                    
                    return true;
                }
                return false;
            }
        public void ActionReadEOL(){
                line++;
                start = current++;
        }
       
        public bool ReadComment(){
            if (Peek()=='#')
            {
                string text="";
                while (!EOL|| !EOF)
                {
                    text += ReadAny();
                    if (EOL|| EOF)
                    {
                       break;
                    }
                }
                return true;
            }
            return false;
        }
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

//Comentarios que tengo que revisar al final de esto
//Cómo se cuentan las filas????