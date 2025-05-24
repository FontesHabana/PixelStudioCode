namespace PixelWallE.Core;

public class RobotState
{
    public int X{get; set;}
    public int Y{get; set;}
    public int BrushSize{get;set;}
    public string BrushColor{get; set;}

    public RobotState(){
        X=0;
        Y=0;
        BrushSize=1;
        BrushColor="transparent";
    }

    
}