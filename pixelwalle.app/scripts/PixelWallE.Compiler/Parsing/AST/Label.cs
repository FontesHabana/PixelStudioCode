namespace PixelWallE.Language.Parsing;
public class Label
{
    public string LabelReference{get; private set;}
    public int CommandIndicator{get; private set;}

   public Label(string labelRefence, int commnadIndicator){
        LabelReference=labelRefence;
        CommandIndicator=commnadIndicator;
    }
}