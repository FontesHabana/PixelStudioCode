using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using System.Drawing;
using PixelWallE.Core;

namespace PixelWallE.Language.Expressions;

/// <summary>
/// Represents a function that retrieves the count of a specific color within a rectangular region of the canvas in the PixelWallE language AST.
/// </summary>
public class GetColorCountFunction : Function, IColor
{

    public PixelColor color{ get; set; }
    /// <summary>
    /// Gets or sets the list of arguments (color, x1, y1, x2, y2).
    /// </summary>
    public override List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the expression type (always Number for color count).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the function (the color count).
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetColorCountFunction"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this function call.</param>
    /// <param name="functionName">The token type representing the function name.</param>
    /// <param name="args">The list of argument expressions (color, x1, y1, x2, y2).</param>
    public GetColorCountFunction(CodeLocation location, TokenType functionName, List<Expression>? args) : base(location, functionName, args)
    {
        Type = ExpressionType.Number;
        Name = "GetColorFunction"; // Note: Typo in original code, should be GetColorCount
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the function node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.GetColorCountFunction(this);
    }
}