using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

namespace PixelWallE.Language.Expressions;

/// <summary>
/// Represents the abstract base class for function expressions in the PixelWallE language AST.
/// Functions have a type, a name, and a list of argument expressions.
/// </summary>
public abstract class Function : AtomExpression, IArgument<Expression>, IName
{
    /// <summary>
    /// Gets or sets the list of argument expressions for the function.
    /// </summary>
    public virtual List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the token type representing the function type.
    /// </summary>
    public TokenType FunctionType { get; set; }

    /// <summary>
    /// Gets or sets the name of the function.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Function"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this function.</param>
    /// <param name="functionType">The token type representing the function type.</param>
    /// <param name="args">The list of argument expressions for the function.</param>
    public Function(CodeLocation location, TokenType functionType, List<Expression>? args) : base(location)
    {
        FunctionType = functionType;
        if (args == null)
        {
            Args = new List<Expression>();
        }
        else
        {
            Args = args;
        }
    }
}