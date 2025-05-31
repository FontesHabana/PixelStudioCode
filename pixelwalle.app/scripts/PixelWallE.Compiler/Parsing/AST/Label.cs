namespace PixelWallE.Language.Parsing;

/// <summary>
/// Represents a label in the PixelWallE program, used for referencing positions in the command list (e.g., for GoTo).
/// </summary>
public class Label
{
    /// <summary>
    /// Gets the label's reference name.
    /// </summary>
    public string LabelReference { get; private set; }

    /// <summary>
    /// Gets the index of the command associated with this label.
    /// </summary>
    public int CommandIndicator { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    /// <param name="labelRefence">The name of the label.</param>
    /// <param name="commnadIndicator">The index of the command this label points to.</param>
    public Label(string labelRefence, int commnadIndicator)
    {
        LabelReference = labelRefence;
        CommandIndicator = commnadIndicator;
    }
}