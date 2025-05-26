namespace PixelWallE.Language.Tokens;

using System;
using System.Collections;
using System.Collections.Generic;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

/* This stream has functions to operate over a list of tokens.
The methods are simple, you can understand them easily */
public class TokenStream : IEnumerable<Token>
{
    public List<Token> tokens;

    public int position;
    public int current;
    public int Position { get { return position; } }

    public TokenStream(IEnumerable<Token> tokens)
    {
        this.tokens = new List<Token>(tokens);
        position = 0;
        current=0;
    }

    public bool End => position == tokens.Count-1;

public bool IsAtEnd() {
return Peek().Type == TokenType.EOF;
}
    public Token Peek(){
         return tokens[current];
    }

    public bool Match(List<TokenType> types){
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    public bool Check(TokenType type){
        if (End) return false;
        return Peek().Type==type;
    }

    public Token? Consume(TokenType type,string expected)
    {
         if (Check(type)) return Advance();
        throw SyntaxException.UnexpectedToken(type.ToString(), expected, Peek().Location);
    }

    public void Synchronize(){
        //Advance();
        while (!IsAtEnd())
        {    
            if (Peek().Type == TokenType.EOL)
            {
                Advance();
                return;
            }
            Advance();
        }
    }
    public Token Advance(){
        if (!End) current++;
        
        return Previous();
    }

    public Token Previous(){
        return tokens[current-1];
    }


     
    public void MoveNext(int k)
    {
        current += k;
    }

    public void MoveBack(int k)
    {
        current -= k;
    }

     /* The next methods are used to scroll through the token list 
     if a condition is satisfied */

     /* In this case, the condition is to have a next position */
    public bool Next()
    {
        if (position < tokens.Count - 1)
        {
            position++;
        }

        return position < tokens.Count;
    }

    /* In this case, the next position must match the given type */
    public bool Next( TokenType type )
    {
        if (position < tokens.Count-1 && LookAhead(1).Type == type)
        {
            position++;
            return true;
        }

        return false;
    }

    /* In this cas, the next position must match the given value */
    public bool Next(string value)
    {            
        if (position < tokens.Count-1 && LookAhead(1).Value == value)
        {
            position++;
            return true;
        }

        return false;
    }

    public bool CanLookAhead(int k = 0)
    {
        return tokens.Count - position > k;
    }

    public Token LookAhead(int k = 0)
    {
        return tokens[position + k];
    }

    public IEnumerator<Token> GetEnumerator()
    {
        for (int i = position; i < tokens.Count; i++)
            yield return tokens[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
/*
    internal bool Match(TokenType TRUE)
    {
        throw new NotImplementedException();
    }*/
}

