using System.Runtime.InteropServices;
using System.Collections.Generic;
namespace PixelWallE.Language.Parsing;

/// <summary>
/// Represents the root node of a PixelWallE program's Abstract Syntax Tree (AST).
/// Contains the list of statements, labels, and any errors found during parsing.
/// </summary>
public class ElementalProgram : ASTNode
{
    /// <summary>
    /// Gets or sets the list of errors found during parsing or semantic analysis.
    /// </summary>
    public List<PixelWallEException> Errors { get; set; }

    /// <summary>
    /// Gets or sets the list of statements (AST nodes) that make up the program.
    /// </summary>
    public List<ASTNode> Statements { get; set; }

    /// <summary>
    /// Gets the list of labels defined in the program, if any.
    /// </summary>
    public List<Label>? Labels { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementalProgram"/> class.
    /// </summary>
    /// <param name="location">The code location associated with this program node.</param>
    /// <param name="errors">The list of errors found during parsing or semantic analysis.</param>
    public ElementalProgram(CodeLocation location, List<PixelWallEException> errors) : base(location)
    {
        Errors = errors;
        Statements = new List<ASTNode>();
        Labels = new List<Label>();
    }

    /// <summary>
    /// Accepts a visitor for traversing or processing the program node.
    /// </summary>
    /// <param name="visitor">The visitor instance.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
        visitor.ElementalProgram(this);
    }
}