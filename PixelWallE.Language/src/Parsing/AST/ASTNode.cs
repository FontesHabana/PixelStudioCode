namespace PixelWallE.Language.Parsing;
using PixelWallE.Language.Tokens;
using PixellWallE.Language.Lexer;
public abstract class ASTNode
    {
        public CodeLocation Location {get; set;}
        public abstract bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors);
        public ASTNode(CodeLocation location)
        {
            Location = location;
        }
    }  

//Solo para ver funcionamiento
public class printAst{
    public static void printAstNode(ASTNode node, int depth)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}{node.GetType().Name} at {node.Location}");
        foreach (var property in node.GetType().GetProperties())
        {
            if (property.PropertyType == typeof(ASTNode))
            {
                var childNode = (ASTNode)property.GetValue(node);
                if (childNode != null)
                {
                    printAstNode(childNode, depth + 1);
                }
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (IEnumerable<ASTNode>)property.GetValue(node);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        printAstNode(item, depth + 1);
                    }
                }
            }
        }
    }
} 