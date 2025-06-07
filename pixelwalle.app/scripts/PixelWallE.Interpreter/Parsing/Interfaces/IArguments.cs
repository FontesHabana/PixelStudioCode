namespace PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using System.Collections.Generic;

/// <summary>
/// Represents an interface for AST nodes that have a list of arguments.
/// </summary>
/// <typeparam name="T">The type of the arguments.</typeparam>
public interface IArgument<T>
{
    /// <summary>
    /// Gets or sets the list of arguments.
    /// </summary>
    List<T> Args { get; set; }
}