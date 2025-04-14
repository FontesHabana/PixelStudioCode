using System.Dynamic;

namespace PixelWallE.Language.Tokens;

public class Token
{
     public TokenType Type { get; private set;}
     public string Value {get; private set;}
     public CodeLocation Location {get; private set;}

     public Token(TokenType type, string value, CodeLocation location){
        Type=type;
        Value=value;
        Location=location;
     }

     //View Token
     public String toString(){
        return Type+ " "+ Value;
     }

}

