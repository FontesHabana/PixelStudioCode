namespace PixelWallE.Language.Parsing;

using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;
using PixelWallE.Core;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.Arm;
using PixelWallE.Language.Parsing.Expressions.Literals;
using System.IO;
using System.Threading;


/// <summary>
/// The <c>Executer</c> class is responsible for interpreting and executing the parsed Abstract Syntax Tree (AST) of the PixelWallE language.
/// It traverses the AST, evaluates expressions, and performs commands to manipulate the canvas and robot state.
/// </summary>
public class Executer : IVisitor<ASTNode>
{
    private Scope scope;
    /// <summary>
    /// A list to store any errors encountered during the execution of the program.
    /// </summary>
    public List<PixelWallEException> errors;
    /// <summary>
    /// A list to store messages to be displayed on the console.
    /// </summary>
    public List<string> ConsoleMessages;
    /// <summary>
    /// The canvas on which the robot will draw.
    /// </summary>
    private Canvas canvas;
    /// <summary>
    /// The state of the robot.
    /// </summary>
    private RobotState robot;
    /// <summary>
    /// Gets or sets the current index of the statement being executed in the program.
    /// </summary>
    /// <value>
    /// The index of the current statement.
    /// </value>
    public int Index { get; set; }


    CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <c>Executer</c> class.
    /// </summary>
    /// <param name="scope">The scope containing variables and labels.</param>
    /// <param name="canvas">The canvas to be manipulated.</param>
    /// <param name="robot">The robot state to be updated.</param>
    /// <param name="error">The list to store any errors encountered during execution.</param>
    /// /// <param name="consoleMessages">The list to store messages for the console.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param> 
    public Executer(Scope scope, Canvas canvas, RobotState robot, List<PixelWallEException> error, List<string> consoleMessages, CancellationToken cancellationToken)
    {
        this.scope = scope;
        this.canvas = canvas;
        this.robot = robot;
        ConsoleMessages = consoleMessages;

        errors = error;
        Index = 0;

        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Executes the given elemental program by traversing its statements and handling any exceptions.
    /// </summary>
    /// <param name="program">The elemental program to execute.</param>
    public void ElementalProgram(ElementalProgram program)
    {
        while (program.Statements.Count > Index)
        {
            try
            {   cancellationToken.ThrowIfCancellationRequested();
                program.Statements[Index].Accept(this);
                Index++;
            }
            catch (PixelWallEException error)
            {
                errors.Add(error);
                Godot.GD.Print(error);
                return;
            }
            catch (System.Exception error)
            {
                errors.Add(new RuntimeException(error.ToString(), program.Statements[Index].Location));
                Godot.GD.Print(error);
                return;
            }
        }
    }

    #region Expressions

    /// <summary>
    /// Evaluates a parenthesized expression.
    /// </summary>
    /// <param name="expression">The parenthesized expression to evaluate.</param>
    public void ParenthesizedExpression(ParenthesizedExpression expression)
    {
        expression.InnerExpression.Accept(this);
        expression.Value = expression.InnerExpression.Value;
    }

    /// <summary>
    /// Retrieves the value of a variable from the current scope.
    /// </summary>
    /// <param name="var">The variable to retrieve the value from.</param>
    public void Variable(Variable var)
    {
        var.Value = scope.GetVariable(var.VariableName);
    }



    private ExpressionType GetExpressionType(ExpressionType type)
    {
        ExpressionType argType = ExpressionType.Anytype;
        if (type == ExpressionType.ListIntegerOrBool)
            argType = ExpressionType.IntegerOrBool;
        if (type == ExpressionType.ListString)
            argType = ExpressionType.String;
        return argType;
    }
    public void List(List list)
    {
        ExpressionType argType = GetExpressionType(list.Type);
        foreach (Expression item in list.Args)
        {
            item.Accept(this);
        }
        list.Value = list.Args;

    }
    public void ListElement(ListElement element)
    {
        List<Expression> list = (List<Expression>)scope.GetVariable(element.ListReference);
        element.Index.Accept(this);
        if ((IntegerOrBool)element.Index.Value < list.Count && (IntegerOrBool)element.Index.Value >= 0)
        {   
            element.Value = list[(IntegerOrBool)element.Index.Value].Value;
            return;
        }
        throw RuntimeException.IndexOutOfRange((IntegerOrBool)element.Index.Value,list.Count, "Get element", element.Index.Location);
    }

    #region functions

    /// <summary>
    /// Gets the actual X coordinate of the robot.
    /// </summary>
    /// <param name="function">The function to get the X coordinate.</param>
    public void GetActualXFunction(GetActualXFunction function)
    {
        function.Value = new IntegerOrBool(robot.X);
    }

    /// <summary>
    /// Gets the actual Y coordinate of the robot.
    /// </summary>
    /// <param name="function">The function to get the Y coordinate.</param>
    public void GetActualYFunction(GetActualYFunction function)
    {
        function.Value = new IntegerOrBool(robot.Y);
    }

    /// <summary>
    /// Gets the size of the canvas.
    /// </summary>
    /// <param name="function">The function to get the canvas size.</param>
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function)
    {
        function.Value = new IntegerOrBool(canvas.Size);
    }

    /// <summary>
    /// Gets the count of a specific color within a specified rectangular region on the canvas.
    /// </summary>
    /// <param name="function">The function to get the color count.</param>
    /// <exception cref="RuntimeException">Thrown when the provided coordinates are out of bounds.</exception>
    public void GetColorCountFunction(GetColorCountFunction function)
    {
        foreach (Expression item in function.Args)
        {
            item.Accept(this);
        }

        IntegerOrBool color = (IntegerOrBool)function.Args[0].Value;
        IntegerOrBool x1 = (IntegerOrBool)function.Args[1].Value;
        IntegerOrBool y1 = (IntegerOrBool)function.Args[2].Value;
        IntegerOrBool x2 = (IntegerOrBool)function.Args[3].Value;
        IntegerOrBool y2 = (IntegerOrBool)function.Args[4].Value;


        if (x1 < 0 || x1 >= canvas.Size || x2 < 0 || x2 >= canvas.Size || y1 < 0 || y1 >= canvas.Size || y2 < 0 || y2 >= canvas.Size)
        {
            function.Value = new IntegerOrBool(0);
            return;
        }

        if (x1 > x2)
        {
            int temp = x1;
            x1 = x2;
            x2 = temp;
        }

        if (y1 > y2)
        {
            int temp = y1;
            y1 = y2;
            y2 = temp;
        }

        int colorCount = 0;
        for (int y = y1; y <= y2; y++)
        {
            for (int x = x1; x < x2; x++)
            {
                if (canvas.Matrix[y, x] == function.color)
                {
                    colorCount++;
                }
            }
        }
        function.Value = new IntegerOrBool(colorCount);
    }

    /// <summary>
    /// Checks if the current brush color of the robot matches the specified color.
    /// </summary>
    /// <param name="function">The function to check the brush color.</param>
    public void IsBrushColorFunction(IsBrushColorFunction function)
    {
        foreach (Expression item in function.Args)
        {
            item.Accept(this);
        }

        object color = function.Args[0].Value;
        if (robot.BrushColor == function.color)
            function.Value = new IntegerOrBool(true);
        else
            function.Value = new IntegerOrBool(false);
    }

    /// <summary>
    /// Checks if the current brush size of the robot matches the specified size.
    /// </summary>
    /// <param name="function">The function to check the brush size.</param>
    public void IsBrushSizeFunction(IsBrushSizeFunction function)
    {
        foreach (Expression item in function.Args)
        {
            item.Accept(this);
        }

        object size = function.Args[0].Value;
        if (robot.BrushSize == (IntegerOrBool)size)
            function.Value = new IntegerOrBool(true);
        else
            function.Value = new IntegerOrBool(false);
    }

    /// <summary>
    /// Checks if the color of a specific pixel on the canvas matches the specified color.
    /// </summary>
    /// <param name="function">The function to check the canvas color.</param>
    public void IsCanvasColor(IsCanvasColor function)
    {
        foreach (Expression item in function.Args)
        {
            item.Accept(this);
        }

        int x = (IntegerOrBool)function.Args[1].Value;
        int y = (IntegerOrBool)function.Args[2].Value;

        if (robot.X + x < 0 || robot.X + x >= canvas.Size || robot.Y + y < 0 || robot.Y + y >= canvas.Size)
            function.Value = new IntegerOrBool(false);
        else if (canvas.Matrix[robot.Y + y, robot.X + x] == function.color)
            function.Value = new IntegerOrBool(true);
        else
            function.Value = new IntegerOrBool(false);
    }

    #endregion

    #region  Unary Expressions

    /// <summary>
    /// Performs a logical NOT operation on the given expression.
    /// </summary>
    /// <param name="operation">The NOT operation to perform.</param>
    public void NotOperation(NotOperation operation)
    {
        operation.Right.Accept(this);
        operation.Value = new IntegerOrBool(!(IntegerOrBool)operation.Right.Value);
    }
    
    /// <summary>
    /// Performs a negation operation on the given expression.
    /// </summary>
    /// <param name="operation">The negation operation to perform.</param>
    public void NegationOperation(NegationOperation operation)
    {
        operation.Right.Accept(this);

        IntegerOrBool right = (IntegerOrBool)operation.Right.Value;
        operation.Value = new IntegerOrBool(-right);
    }

    #endregion

    #region Binary Expressions

    #region Arithmetic Operation

    /// <summary>
    /// Performs an addition operation on the given expression.
    /// </summary>
    /// <param name="operation">The addition operation to perform.</param>
    public void AdditionOperation(AdditionOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        if (operation.Type == ExpressionType.IntegerOrBool)
        {
            IntegerOrBool left = (IntegerOrBool)operation.Left.Value;
            IntegerOrBool right = (IntegerOrBool)operation.Right.Value;
            operation.Value = new IntegerOrBool(left + right);
        }
        else
        {
             string left = operation.Left.Value.ToString();
             string right = operation.Right.Value.ToString ();
             operation.Value = left + right;
        }
        
    }

    /// <summary>
    /// Performs a division operation on the given expression.
    /// </summary>
    /// <param name="operation">The division operation to perform.</param>
    /// <exception cref="RuntimeException">Thrown when dividing by zero.</exception>
    public void DivisionOperation(DivisionOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        if (right == 0)
        {
            throw RuntimeException.DivisionByZero(operation.Location);
        }
        operation.Value = new IntegerOrBool(left / right);
    }

    /// <summary>
    /// Performs an exponentiation operation on the given expression.
    /// </summary>
    /// <param name="operation">The exponentiation operation to perform.</param>
    /// <exception cref="RuntimeException">Thrown when raising zero to the power of zero.</exception>
    public void ExponentiationOperation(ExponentiationOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        if ((IntegerOrBool)left == 0 && (IntegerOrBool)right == 0)
        {
            throw RuntimeException.ZeroPowerZero(operation.Location);
        }
        operation.Value = new IntegerOrBool((int)Math.Pow(left, right));
    }

    /// <summary>
    /// Performs a modulo operation on the given expression.
    /// </summary>
    /// <param name="operation">The modulo operation to perform.</param>
    public void ModuloOperation(ModuloOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;
        operation.Value = new IntegerOrBool(left % right);
    }

    /// <summary>
    /// Performs a multiplication operation on the given expression.
    /// </summary>
    /// <param name="operation">The multiplication operation to perform.</param>
    public void MultiplicationOperation(MultiplicationOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        operation.Value = new IntegerOrBool(left * right);
    }

    /// <summary>
    /// Performs a subtraction operation on the given expression.
    /// </summary>
    /// <param name="operation">The subtraction operation to perform.</param>
    public void SubstractionOperation(SubstractionOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        operation.Value = new IntegerOrBool(left - right);
    }

    #endregion

    #region Logic Operation

    /// <summary>
    /// Performs a logical AND operation on the given expression.
    /// </summary>
    /// <param name="operation">The AND operation to perform.</param>
    public void ANDOperation(ANDOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        operation.Value = new IntegerOrBool((IntegerOrBool)operation.Left.Value && (IntegerOrBool)operation.Right.Value);
    }

    /// <summary>
    /// Performs a logical OR operation on the given expression.
    /// </summary>
    /// <param name="operation">The OR operation to perform.</param>
    public void OrOperation(OrOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        operation.Value = new IntegerOrBool((IntegerOrBool)operation.Left.Value || (IntegerOrBool)operation.Right.Value);
    }

    /// <summary>
    /// Performs an equality comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The equality comparison operation to perform.</param>
    public void EqualToOperation(EqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        object left = operation.Left.Value;
        object right = operation.Right.Value;

        operation.Value = new IntegerOrBool(left.Equals(right));
    }

    /// <summary>
    /// Performs a greater-than comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The greater-than comparison operation to perform.</param>
    public void GreatherThanOperation(GreatherThanOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        operation.Value = new IntegerOrBool((IntegerOrBool)left > (IntegerOrBool)right);
    }

    /// <summary>
    /// Performs a greater-than-or-equal-to comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The greater-than-or-equal-to comparison operation to perform.</param>
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        operation.Value = new IntegerOrBool(left >= right);
    }

    /// <summary>
    /// Performs a less-than comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The less-than comparison operation to perform.</param>
    public void LessThanOperation(LessThanOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;
        operation.Value = new IntegerOrBool(left < right);
    }

    /// <summary>
    /// Performs a less-than-or-equal-to comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The less-than-or-equal-to comparison operation to perform.</param>
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        operation.Value = new IntegerOrBool(left <= right);
    }

    /// <summary>
    /// Performs a not-equal-to comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The not-equal-to comparison operation to perform.</param>
    public void NotEqualToOperation(NotEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);


        if (operation.Left.Type == ExpressionType.String)
        {
            operation.Value = new IntegerOrBool(!operation.Left.Equals(operation.Right));
            return;
        }
        int left = (IntegerOrBool)operation.Left.Value;
        int right = (IntegerOrBool)operation.Right.Value;

        operation.Value = new IntegerOrBool(!left.Equals(right));
    }

    #endregion;

    #endregion

    #endregion

    #region Command

    #region ListCommand
    /// <summary>
    /// Adds an element to the end of the list.
    /// </summary>
    /// <param name="command">The add command to execute.The Args[0] is the element to add to the list.</param>
    public void AddCommand(AddCommand command)
    {
        command.Args[0].Accept(this);

        List<Expression> list = (List<Expression>)scope.GetVariable(command.ListReference);
        list.Add(command.Args[0]);
    }
    /// <summary>
    /// Removes the element at the specified index of the list.
    /// </summary>
    /// <param name="command">The remove at command to execute.The Args[0] is the index to remove from the list.</param>
    /// <exception cref="RuntimeException">Thrown when the index is out of range.</exception>

    public void RemoveAtCommand(RemoveAtCommand command)
    {
        command.Args[0].Accept(this);
        int index = (IntegerOrBool)command.Args[0].Value;
        List<Expression> list = (List<Expression>)scope.GetVariable(command.ListReference);
        if (index < list.Count && index >= 0)
        {
            list.RemoveAt(index);
            return;
        }
        throw RuntimeException.IndexOutOfRange(index,list.Count,command.Name, command.Args[0].Location);

    }
    /// <summary>
    /// Removes all elements from the list.
    /// </summary>
    /// <param name="command">The clear command to execute.</param>

    public void ClearCommand(ClearCommand command)
    {


        List<Expression> list = (List<Expression>)scope.GetVariable(command.ListReference);

        list.Clear();

    }
    /// <summary>
    /// Gets the number of elements contained in the list.
    /// </summary>
    /// <param name="command">The count command to execute.</param>

    public void CountCommand(CountCommand command)
    {
        List<Expression> list = (List<Expression>)scope.GetVariable(command.ListReference);

        command.Value = list.Count;
    }
    #endregion

    /// <summary>
    /// Checks if the given coordinates are within the bounds of the canvas.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="size">The size of the canvas.</param>
    /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
    private bool CheckPosition(int x, int y, int size)
    {
        if (x < 0 || x >= size || y < 0 || y >= size)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the given direction coordinates are valid.
    /// </summary>
    /// <param name="command">The command containing the direction coordinates.</param>
    /// <exception cref="RuntimeException">Thrown when the direction coordinates are invalid.</exception>
    private void CheckDirection(Command command)
    {
        int x = (IntegerOrBool)command.Args[0].Value;
        int y = (IntegerOrBool)command.Args[1].Value;
        if (x != 1 && x != -1 && x != 0)
        {
            throw RuntimeException.InvalidDirectionCoordinates(x, y, command.Name, command.Location);
        }
        if (y != 1 && y != -1 && y != 0)
        {
            throw RuntimeException.InvalidDirectionCoordinates(x, y, command.Name, command.Location);
        }
    }

    /// <summary>
    /// Throws an exception if the robot's position is out of bounds.
    /// </summary>
    /// <param name="x">The x-coordinate of the robot.</param>
    /// <param name="y">The y-coordinate of the robot.</param>
    /// <param name="size">The size of the canvas.</param>
    /// <param name="node">The AST node causing the exception.</param>
    /// <exception cref="RuntimeException">Thrown when the robot's position is out of bounds.</exception>
    private void RobotOutException(int x, int y, int size, ASTNode node)
    {
        if (!CheckPosition(x, y, size))
        {
            throw RuntimeException.PositionOutOfBounds(x, y, node.ToString(), node.Location);
        }
    }

    /// <summary>
    /// Draws a pixel on the canvas with the robot's current brush color and size.
    /// </summary>
    /// <param name="X">The x-coordinate of the pixel.</param>
    /// <param name="Y">The y-coordinate of the pixel.</param>
    private void DrawPixel(int X, int Y)
    {
        for (int y = 0; y < robot.BrushSize; y++)
        {
            for (int x = 0; x < robot.BrushSize; x++)
            {
                if (CheckPosition(X + x, Y + y, canvas.Size) && robot.BrushColor.Alpha != 0)
                {
                    canvas.Matrix[Y + y, X + x] = robot.BrushColor;
                }
            }
        }
    }

    /// <summary>
    /// Moves the robot to a new position on the canvas.
    /// </summary>
    /// <param name="x">The amount to move in the x-direction.</param>
    /// <param name="y">The amount to move in the y-direction.</param>
    /// <param name="node">The AST node causing the movement.</param>
    private void MoveRobot(int x, int y, ASTNode node)
    {
        robot.X += x;
        robot.Y += y;
        RobotOutException(robot.X, robot.Y, canvas.Size, node);
    }

    /// <summary>
    /// Assigns a value to a variable in the current scope.
    /// </summary>
    /// <param name="command">The assignment expression to execute.</param>
    public void AssigmentExpression(AssigmentExpression command)
    {
        command.Argument.Accept(this);
        scope.AssignVariable(command.Var.VariableName, command.Argument.Value, command.Argument.Type);
    }





    public void AssigmentListElement(AssigmentListElement command)
    {
        command.Argument.Accept(this);
        command.Index.Accept(this);
        int index = (IntegerOrBool)command.Index.Value;
        List<Expression> list = (List<Expression>)scope.GetVariable(command.Var.VariableName);
        if (index < list.Count && index >= 0)
        {
            list[index]=command.Argument;
            return;
        }
        throw RuntimeException.IndexOutOfRange(index,list.Count,command.Name, command.Index.Location);
    }
    /// <summary>
    /// Sets the brush color of the robot.
    /// </summary>
    /// <param name="command">The color command to execute.</param>
    public void ColorCommand(ColorCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        object color = command.Args[0].Value;
        robot.BrushColor = command.color;
    }

    /// <summary>
    /// Draws a circle on the canvas using the robot's current position and brush.
    /// </summary>
    /// <param name="command">The draw circle command to execute.</param>
    /// <exception cref="RuntimeException">
    /// Thrown when the radius is not a positive value or when the direction coordinates are invalid.
    /// </exception>
    public void DrawCircleCommand(DrawCircleCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }

        if ((IntegerOrBool)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Radio", (IntegerOrBool)command.Args[2].Value, command.Location);
        }

        CheckDirection(command);
        object cx = command.Args[0].Value;
        object cy = command.Args[1].Value;
        object r = command.Args[2].Value;

        for (int i = 0; i < (IntegerOrBool)r; i++)
        {
            MoveRobot((IntegerOrBool)cx, (IntegerOrBool)cy, command);
        }

        //Implementar algoritmo de bresenham
        int x = 0;
        int y = (IntegerOrBool)r;
        int d = 3 - 2 * (IntegerOrBool)r;

        while (x <= y)
        {
            // Dibujar los ocho octantes
            DrawPixel(robot.X + x, robot.Y + y);
            DrawPixel(robot.X - x, robot.Y + y);
            DrawPixel(robot.X + x, robot.Y - y);
            DrawPixel(robot.X - x, robot.Y - y);
            DrawPixel(robot.X + y, robot.Y + x);
            DrawPixel(robot.X - y, robot.Y + x);
            DrawPixel(robot.X + y, robot.Y - x);
            DrawPixel(robot.X - y, robot.Y - x);

            if (d < 0)
            {
                d += 4 * x + 6;
            }
            else
            {
                d += 4 * (x - y) + 10;
                y--;
            }
            x++;
        }
    }

    /// <summary>
    /// Draws a line on the canvas using the robot's current position and brush.
    /// </summary>
    /// <param name="command">The draw line command to execute.</param>
    /// <exception cref="RuntimeException">
    /// Thrown when the distance is not a positive value or when the direction coordinates are invalid.
    /// </exception>
    public void DrawLineCommand(DrawLineCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }

        if ((IntegerOrBool)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Distance", (IntegerOrBool)command.Args[2].Value, command.Location);
        }

        CheckDirection(command);
        object x = command.Args[0].Value;
        object y = command.Args[1].Value;
        object distance = command.Args[2].Value;

        for (int i = 0; i < (IntegerOrBool)distance; i++)
        {
            DrawPixel(robot.X, robot.Y);
            MoveRobot((IntegerOrBool)x, (IntegerOrBool)y, command);
        }
    }

    /// <summary>
    /// Draws a rectangle on the canvas using the robot's current position and brush.
    /// </summary>
    /// <param name="command">The draw rectangle command to execute.</param>
    /// <exception cref="RuntimeException">
    /// Thrown when the distance, width, or height are not positive values, or when the direction coordinates are invalid.
    /// </exception>
    public void DrawRectangleCommand(DrawRectangleCommand command)
    {
        Godot.GD.Print("empiezzo a dibujar");
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }

        if ((IntegerOrBool)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Distance", (IntegerOrBool)command.Args[2].Value, command.Location);
        }
        if ((IntegerOrBool)command.Args[3].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Width", (IntegerOrBool)command.Args[2].Value, command.Location);
        }
        if ((IntegerOrBool)command.Args[4].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Height", (IntegerOrBool)command.Args[2].Value, command.Location);
        }

        CheckDirection(command);
        int x = (IntegerOrBool)command.Args[0].Value;
        int y = (IntegerOrBool)command.Args[1].Value;
        int distance = (IntegerOrBool)command.Args[2].Value;

        int width = (IntegerOrBool)command.Args[3].Value;
        int height = (IntegerOrBool)command.Args[4].Value;

        Godot.GD.Print(x);
        Godot.GD.Print(y);
        Godot.GD.Print(distance);
        Godot.GD.Print(width);
        Godot.GD.Print(height);
        for (int i = 0; i < (IntegerOrBool)distance; i++)
        {
            MoveRobot(x, y, command);
        }



        for (int i = robot.Y - height + 1; i < robot.Y + height; i++)
        {
            DrawPixel(robot.X + width - 1, i);
            DrawPixel(robot.X - width + 1, i);
        }
        for (int i = robot.X - width + 1; i < robot.X + width; i++)
        {
            DrawPixel(i, robot.Y + height - 1);
            DrawPixel(i, robot.Y - height + 1);
        }
    }

    /// <summary>
    /// Fills a region on the canvas with the robot's current brush color, starting from the robot's current position.
    /// </summary>
    /// <param name="command">The fill command to execute.</param>
    public void FillCommand(FillCommand command)
    {
        PixelColor currentColor = canvas.Matrix[robot.Y, robot.X];
        Stack<(int, int)> cells = new Stack<(int, int)>();
        List<(int, int)> visited = new List<(int, int)>();
        cells.Push((robot.Y, robot.X));
        visited.Add((robot.Y, robot.X));

        while (cells.Count > 0)
        {
            (int y, int x) currentCell = cells.Pop();
            if (canvas.Matrix[currentCell.y, currentCell.x] == currentColor)
            {
                canvas.Matrix[currentCell.y, currentCell.x] = robot.BrushColor;
                if (currentCell.x + 1 < canvas.Size && canvas.Matrix[currentCell.y, currentCell.x + 1] == currentColor)
                {
                    if (!visited.Contains((currentCell.y, currentCell.x + 1)))
                    {
                        cells.Push((currentCell.y, currentCell.x + 1));
                        visited.Add((currentCell.y, currentCell.x + 1));
                    }
                }
                if (currentCell.x - 1 >= 0 && canvas.Matrix[currentCell.y, currentCell.x - 1] == currentColor)
                {
                    if (!visited.Contains((currentCell.y, currentCell.x - 1)))
                    {
                        cells.Push((currentCell.y, currentCell.x - 1));
                        visited.Add((currentCell.y, currentCell.x - 1));
                    }
                }
                if (currentCell.y + 1 < canvas.Size && canvas.Matrix[currentCell.y + 1, currentCell.x] == currentColor)
                {
                    if (!visited.Contains((currentCell.y + 1, currentCell.x)))
                    {
                        cells.Push((currentCell.y + 1, currentCell.x));
                        visited.Add((currentCell.y + 1, currentCell.x));
                    }
                }
                if (currentCell.y - 1 >= 0 && canvas.Matrix[currentCell.y - 1, currentCell.x] == currentColor)
                {
                    if (!visited.Contains((currentCell.y - 1, currentCell.x)))
                    {
                        cells.Push((currentCell.y - 1, currentCell.x));
                        visited.Add((currentCell.y - 1, currentCell.x));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets the brush size of the robot.
    /// </summary>
    /// <param name="command">The size command to execute.</param>
    public void SizeCommand(SizeCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        int size = (IntegerOrBool)command.Args[0].Value;
        if (size >= 0)
        {
            robot.BrushSize = size;
        }
        else
        {
            throw RuntimeException.ArgumentMostBePositive("Size", size, command.Args[0].Location);
        }
      
    }

    /// <summary>
    /// Sets the initial position of the robot on the canvas.
    /// </summary>
    /// <param name="command">The spawn command to execute.</param>
    public void SpawnCommand(SpawnCommand command)
    {
        SetRobotPosition(command);
    }

    /// <summary>
    /// Resets the position of the robot on the canvas.
    /// </summary>
    /// <param name="command">The respawn command to execute.</param>
    public void ReSpawnCommand(ReSpawnCommand command)
    {
        SetRobotPosition(command);
    }

    /// <summary>
    /// Sets the robot's position based on the provided command arguments.
    /// </summary>
    /// <param name="command">The command containing the x and y coordinates.</param>
    private void SetRobotPosition(IArgument<Expression> command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        IntegerOrBool x =(IntegerOrBool) command.Args[0].Value;
        IntegerOrBool y =(IntegerOrBool) command.Args[1].Value;
        robot.X = x;
        robot.Y = y;
        RobotOutException(robot.X, robot.Y, canvas.Size, (ASTNode)command);
    }

    /// <summary>
    /// Jumps to a specific label in the code if the given condition is true.
    /// </summary>
    /// <param name="command">The go-to command to execute.</param>
    /// <exception cref="RuntimeException">Thrown when a possibly infinite loop is detected.</exception>
    public void GoToCommand(GoToCommand command)
    {
        
            foreach (Expression arg in command.Args)
            {
                arg.Accept(this);
            }
            if ((IntegerOrBool)command.Args[0].Value)
            {
                Label? label = scope.GetLabel(command.Label);
                Index = label.CommandIndicator - 1;
            }
            
        
    }


    /// <summary>
    /// Prints the value of the argument to the console.
    /// </summary>
    /// <param name="command">The print command to execute.</param>
    public void PrintCommand(PrintCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        ConsoleMessages.Add(command.Args[0].Value.ToString());
    }


    /// <summary>
    /// Executes a script from a specified file path.
    /// </summary>
    /// <param name="command">The run command to execute.</param>
    /// <exception cref="RuntimeException">Thrown when the file does not exist or when errors are found in the executed script.</exception>

    public void RunCommand(RunCommand command)
    {
        foreach (var item in command.Args)
        {
            item.Accept(this);
        }
        string path = (string)command.Args[0].Value;
        if (!File.Exists(path))
        {
            throw RuntimeException.FindExceptionInPath(path);
        }
        string source = File.ReadAllText(path);
        Godot.GD.Print(source);
        Interpreter localInterpreter = new Interpreter(canvas, source);
        try
        {
            localInterpreter.Run(cancellationToken);
        }
        catch (SystemException)
        {

        }

        foreach (var item in localInterpreter.ConsoleMessage)
        {
            ConsoleMessages.Add(item);
        }
        if (localInterpreter.Errors.Count > 0)
        {
            errors.Add(RuntimeException.ErrorsInPath(path));
            foreach (var item in localInterpreter.Errors)
            {
                errors.Add(item);
            }
            throw RuntimeException.ErrorsInPath(path);
        }


    }
    #endregion
}