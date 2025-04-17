
namespace PixellWallE.Language.Lexer;
using PixelWallE.Language.Tokens;

//Modificar esto
public class CompilingError
    {
        public ErrorCode Code { get; private set; }

        public string Argument { get; private set; }

        public CodeLocation Location {get; private set;}

        public CompilingError(CodeLocation location, ErrorCode code, string argument)
        {
            this.Code = code;
            this.Argument = argument;
            Location = location;
        }

    public override string ToString()
    {
        return Code+ " " + Argument + " " + Location.Line + " " + Location.Column;
    }

    }

    public enum ErrorCode
    {
        None,
        Expected,
        Invalid,
        Unknown,
    }