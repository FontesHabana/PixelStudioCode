using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using PixelWallE.Core;
namespace PixelWallE.Language.Expressions;

/// <summary>
/// Represents a function that checks if a specific pixel on the canvas matches a given color in the PixelWallE language AST.
/// </summary>
public class IsCanvasColor : Function, IColor
{    public PixelColor color{ get; set; }
    /// <summary>
    /// Gets or sets the list of arguments (color, x, y).
    /// </summary>
    public override List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the expression type (always Bool for boolean functions).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the function (true if the pixel color matches, false otherwise).
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsCanvasColor"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this function call.</param>
    /// <param name="functionName">The token type representing the function name.</param>
    /// <param name="args">The list of argument expressions (color, x, y).</param>
    public IsCanvasColor(CodeLocation location, TokenType functionName, List<Expression?> args) : base(location, functionName, args)
    {
        Type = ExpressionType.Bool;
        Name = "IsCanvasColor";
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the function node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.IsCanvasColor(this);
    }
}