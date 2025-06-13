namespace PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

/// <summary>
/// Represents an interface for AST nodes that have a name property.
/// </summary>
public interface IName
{
    /// <summary>
    /// Gets or sets the name of the node.
    /// </summary>
    string Name { get; set; }
}