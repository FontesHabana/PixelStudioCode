namespace PixelWallE.Language.Expressions;

public abstract class Expression
{
    

    protected abstract void interpret();
    protected abstract void resolve();
    protected abstract void analyze();
}