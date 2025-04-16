using System.Runtime.InteropServices;
using PixellWallE.Language.Lexer;
using PixelWallE.Language.Tokens;


public class PWELanguage{
    static bool hadError=false;
    public static void Main(){
        string text = File.ReadAllText("./test.pw"); 
        
        run(text);
        





    }


   private static void run(string source){
    Lexer lex = LexerProvider.Lexical;

    IEnumerable<Token> tokens= lex.GetTokens("test", source, new List<CompilingError>());
foreach (Token token in tokens)
 {
    Console.WriteLine(token.toString());
 }
   }


    public static void error(CodeLocation location, String message) {
    report(location, "", message);
    }
    private static void report(CodeLocation location, String where,
    String message) {
    System.Console.WriteLine(
    "[line " + location.Line + "][column" +location.Column+ "] Error" + where + ": " + message);
    hadError = true;
    }
}