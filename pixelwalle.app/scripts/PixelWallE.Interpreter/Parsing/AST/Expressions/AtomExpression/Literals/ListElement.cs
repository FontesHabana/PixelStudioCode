using System.Collections.Generic;

namespace PixelWallE.Language.Parsing.Expressions.Literals;

/// <summary>
/// Represents an element within a list, accessed by its reference and index.
/// </summary>
public class ListElement : AtomExpression
{
   
    /// <summary>
    /// Gets or sets the expression type of the list element.
    /// </summary>
    public override ExpressionType Type{ get; set; }

 
    /// <summary>
    /// Gets or sets the value of the list element.
    /// </summary>
    public override object? Value { get; set; }

    /// <summary>
    /// Gets or sets the reference to the list containing this element.
    /// </summary>
    public string ListReference{ get; set; }
    /// <summary>
    /// Gets or sets the index expression used to access the element.
    /// </summary>
    public Expression? Index { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListElement"/> class.
    /// </summary>
    /// <param name="location">The code location where this list element is defined.</param>
    /// <param name="listReference">The reference to the list.</param>
    /// <param name="index">The index expression.</param>
    public ListElement(CodeLocation location, string listReference, Expression index) : base(location)
    {
        ListReference = listReference;
        Index = index;
       
    }

    /// <summary>
    /// Accepts a visitor to traverse this node.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    public override void Accept(IVisitor<ASTNode> visitor)
    {
       visitor.ListElement(this);
    }
}