//Sacar de aqui
using System.Runtime.ConstrainedExecution;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace PixelWallE.Language;


//Modificar esto
public class CompilingError:SystemException
    {   //private Interpreter interpreter=new Interpreter(0, "");
        public ErrorCode Code { get; private set; }

        public string Argument { get; private set; }

        public CodeLocation Location {get; private set;}
         public Token? Token{get;}

    public CompilingError(CodeLocation location, ErrorCode code, string argument)
    {
        this.Code = code;
        this.Argument = argument;
        Location = location;
    }
    public CompilingError(Token token, string message) : base($"[Line {token.Location.Line}:{token.Location.Column}] Parse Error: {message} (Found token: {token.Type}'{token.Value}')")
    {
        Token = token;
        Argument = message;
        Location = token.Location;
        }
    public CompilingError(CodeLocation location, string message) : base($"[Line {location.Line}:{location.Column}] Parse Error: {message} )")
    {
        Token = default;
        Location = location;
        Argument = message;
        }
    public CompilingError(string message) : base($"Parse error: {message}")
    {
        Token = default;
        Argument = message;
        }
          public override string ToString()
            {   
        return "Error: "+ Code+ " " + Argument + " Line:" + Location.Line + " Column:" + Location.Column;
            }

    }


    public enum ErrorCode
    {
        None,
        Expected,
        Invalid,
        Unknown,
    }