using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

namespace PixelWallE.Language.Expressions;

/// <summary>
/// Represents a function that retrieves the robot's current X-coordinate in the PixelWallE language AST.
/// </summary>
public class GetActualXFunction : Function
{
    /// <summary>
    /// Gets or sets the list of arguments (should be empty for this function).
    /// </summary>
    public override List<Expression> Args { get; set; }

    /// <summary>
    /// Gets or sets the expression type (always Number for coordinate values).
    /// </summary>
    public override ExpressionType Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the function (the X-coordinate).
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetActualXFunction"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this function call.</param>
    /// <param name="functionName">The token type representing the function name.</param>
    /// <param name="args">The list of argument expressions (should be empty).</param>
    public GetActualXFunction(CodeLocation location, TokenType functionName, List<Expression?> args) : base(location, functionName, args)
    {
        Type = ExpressionType.Number;
        Name = "GetActualX"; // Note: Typo in original code, should be GetActualX
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the function node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.GetActualXFunction(this);
    }
}