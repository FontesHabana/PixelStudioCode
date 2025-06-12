using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

namespace PixelWallE.Language.Expressions;

/// <summary>
/// Represents a function that checks if the current brush size matches a given size in the PixelWallE language AST.
/// </summary>
public class IsBrushSizeFunction : Function
{
    /// <summary>
    /// Gets or sets the list of arguments (the size to compare with).
    /// </summary>
    public override List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the expression type (always Number for size values).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the function (true if the brush size matches, false otherwise).
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsBrushSizeFunction"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this function call.</param>
    /// <param name="functionName">The token type representing the function name.</param>
    /// <param name="args">The list of argument expressions (the size to compare with).</param>
    public IsBrushSizeFunction(CodeLocation location, TokenType functionName, List<Expression>? args) : base(location, functionName, args)
    {
        Type = ExpressionType.IntegerOrBool; // Although it returns a boolean, it's comparing against a number
        Name = "IsBrushSize";
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the function node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.IsBrushSizeFunction(this);
    }
}