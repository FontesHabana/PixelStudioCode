using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;

namespace PixelWallE.Language.Commands;

/// <summary>
/// Represents the 'GoTo' command in the PixelWallE language, which performs a conditional jump to a label.
/// </summary>
public class GoToCommand : Command
{
    /// <summary>
    /// Gets or sets the label to jump to.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the counter to prevent infinite cycles.
    /// </summary>
    public int InfinteCycle { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoToCommand"/> class.
    /// </summary>
    /// <param name="location">The code location where the command appears.</param>
    /// <param name="commandName">The token type representing the command name.</param>
    /// <param name="args">The list of argument expressions for the command.</param>
    /// <param name="label">The label to jump to.</param>
    public GoToCommand(CodeLocation location, TokenType commandName, List<Expression> args, string label)
        : base(location, commandName, args)
    {
        Label = label;
        Name = "GoTo";
        InfinteCycle = 0;
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the command node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.GoToCommand(this);
    }
}