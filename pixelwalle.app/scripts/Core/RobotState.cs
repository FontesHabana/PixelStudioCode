namespace PixelWallE.Core;

/// <summary>
/// Represents the state of the robot, including its position, brush size, and brush color.
/// </summary>
public class RobotState
{
    /// <summary>
    /// Gets or sets the X coordinate of the robot.
    /// </summary>
    public int X{get; set;}
    /// <summary>
    /// Gets or sets the Y coordinate of the robot.
    /// </summary>
    public int Y{get; set;}
    /// <summary>
    /// Gets or sets the size of the robot's brush.
    /// </summary>
    public int BrushSize{get;set;}
    /// <summary>
    /// Gets or sets the color of the robot's brush.
    /// </summary>
    public PixelColor BrushColor{get; set;}

    /// <summary>
    /// Initializes a new instance of the <see cref="RobotState"/> class with default values.
    /// </summary>
    public RobotState(){
        X=0;
        Y=0;
        BrushSize=1;
        BrushColor=new PixelColor(0,0,0,0);
    }

    
}