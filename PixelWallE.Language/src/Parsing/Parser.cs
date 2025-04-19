using System.Linq.Expressions;
using PixelWallE.Language.Tokens;

namespace PixelWallE.Language.Parsing;

public class Parser{

    private List<Token> tokens;
    private int currentTokenIndex = 0;

    Parser(List<Token> tokens){
        this.tokens = tokens;
    }


    private Expression Expression(){
        return equality();
    }

    
}
