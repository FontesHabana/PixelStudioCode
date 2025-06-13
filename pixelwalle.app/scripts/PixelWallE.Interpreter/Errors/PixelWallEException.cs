//Sacar de aqui
using System.Runtime.ConstrainedExecution;
using PixelWallE.Language.Tokens;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace PixelWallE.Language;
/// <summary>
/// Base class for all custom exceptions in the Pixel Wall-E application.
/// </summary>
public abstract class PixelWallEException : SystemException
{
    /// <summary>
    /// Gets the code location in the source code where the error occurred.
    /// </summary>
    public CodeLocation Location { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PixelWallEException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    protected PixelWallEException(string message, CodeLocation location) : base(message)
    {
        Location = location;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PixelWallEException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="location">The location in the code where the error occurred.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected PixelWallEException(string message, CodeLocation location,Exception innerException) : base(message, innerException)
    {
        Location = location;
    }
}

