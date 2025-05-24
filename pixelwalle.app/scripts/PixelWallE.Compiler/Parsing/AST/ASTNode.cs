namespace PixelWallE.Language.Parsing;
using PixelWallE.Language;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Parsing.Expressions.Literals;
using PixelWallE.Language.Commands;
using Godot;

using System.Collections.Generic;
public abstract class ASTNode
    {
        public CodeLocation Location {get; set;}
      
        public ASTNode(CodeLocation location)
        {
            Location = location;
        }
   
        public abstract void Accept(IVisitor<ASTNode> visitor);
    }  

//Solo para ver funcionamiento
//Solo para ver funcionamiento
//Solo para ver funcionamiento
public class printAst{
    public static void printAstNode(ASTNode node, int depth, HashSet<ASTNode> visitedNodes)
    {
        const int MAX_DEPTH = 20; // Limit recursion depth to prevent stack overflow

        string indent = new string(' ', depth * 4);
        
        if (node is null)
        {
            GD.Print($"{indent}Null Node");
            return;
        }

        if (depth > MAX_DEPTH)
        {
            GD.Print($"{indent}Max recursion depth reached.");
            return;
        }

        if (node is ElementalProgram)
        {   ElementalProgram enode=(ElementalProgram)node;
            foreach (var item in enode.Statements)
            {   
                node=(ASTNode)item;
                printAstNode(node,2);
            }
        }
        if (visitedNodes.Contains(node))
        {
            GD.Print($"{indent}Cycle detected: Node already visited.");
            return;
        }

        visitedNodes.Add(node);

        GD.Print($"{indent}{node.GetType().Name}  {node.Location.Line}");

        if (node is PixelWallE.Language.Parsing.Expressions.Expression expression)
        {
           // GD.Print($"{indent}  Type: {expression.Type}, Value: {expression.Value}");
        }

        if (node is ParenthesizedExpression parenthesizedExpression)
        {
            GD.Print($"{indent}  Inner Expression:");
            printAstNode(parenthesizedExpression.InnerExpression, depth + 1, visitedNodes);
        }
        else if (node is BinaryExpression binaryExpression)
        {
            GD.Print($"{indent}  Left:");
            printAstNode(binaryExpression.Left, depth + 1, visitedNodes);
            GD.Print($"{indent}  Right:");
            printAstNode(binaryExpression.Right, depth + 1, visitedNodes);
        }
        else if (node is UnaryExpression unaryExpression)
        {
            GD.Print($"{indent}  Operand:");
            printAstNode(unaryExpression.Right, depth + 1, visitedNodes);
        }
        else if (node is Bool boolLiteral)
        {
            GD.Print($"{indent}  Value: {boolLiteral.Value}");
        }
        else if (node is Number numberLiteral)
        {
            GD.Print($"{indent}  Value: {numberLiteral.Value}");
        }
        else if (node is StringLiteral stringLiteral)
        {
            GD.Print($"{indent}  Value: {stringLiteral.Value}");
        }
        else if (node is Variable variable)
        {
            GD.Print($"{indent}  Name: {variable.VariableName}");
        }
        else if (node is Function function)
        {
            GD.Print($"{indent}  Name: {function.FunctionType}");
            GD.Print($"{indent}  Arguments:");
            foreach (var arg in function.Args)
            {
                printAstNode(arg, depth + 1, visitedNodes);
            }
        }
         else if (node is Command command)
        {
            GD.Print($"{indent}  Name: {command.CommandName}");
            GD.Print($"{indent}  Arguments:");
            foreach (var arg in command.Args)
            {
                printAstNode(arg, depth + 1, visitedNodes);
            }
        }
    }

    public static void printAstNode(ASTNode node, int depth)
    {
        printAstNode(node, depth, new HashSet<ASTNode>());
    }
}