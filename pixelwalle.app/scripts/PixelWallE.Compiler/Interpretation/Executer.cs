namespace PixelWallE.Language.Parsing;

using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;
using PixelWallE.Core;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.Arm;


//using Godot;
//using Godot;

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
    public List<string> ConsoleMessages;
    private Canvas canvas;
    private RobotState robot;
    /// <summary>
    /// Gets or sets the current index of the statement being executed in the program.
    /// </summary>
    /// <value>
    /// The index of the current statement.
    /// </value>
    public int Index { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>Executer</c> class.
    /// </summary>
    /// <param name="scope">The scope containing variables and labels.</param>
    /// <param name="canvas">The canvas to be manipulated.</param>
    /// <param name="robot">The robot state to be updated.</param>
    /// <param name="error">The list to store any errors encountered during execution.</param>
    public Executer(Scope scope, Canvas canvas, RobotState robot, List<PixelWallEException> error,List<string> consoleMessages)
    {
        this.scope = scope;
        this.canvas = canvas;
        this.robot = robot;
        ConsoleMessages = consoleMessages;

        errors = error;
        Index = 0;
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
            {
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
                errors.Add(new RuntimeException(error.Message, program.Statements[Index].Location));
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

    #region functions

    /// <summary>
    /// Gets the actual X coordinate of the robot.
    /// </summary>
    /// <param name="function">The function to get the X coordinate.</param>
    public void GetActualXFunction(GetActualXFunction function)
    {
        function.Value = robot.X;
    }

    /// <summary>
    /// Gets the actual Y coordinate of the robot.
    /// </summary>
    /// <param name="function">The function to get the Y coordinate.</param>
    public void GetActualYFunction(GetActualYFunction function)
    {
        function.Value = robot.Y;
    }

    /// <summary>
    /// Gets the size of the canvas.
    /// </summary>
    /// <param name="function">The function to get the canvas size.</param>
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function)
    {
        function.Value = canvas.Size;
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

        object color = function.Args[0];
        object x1 = function.Args[1];
        object y1 = function.Args[2];
        object x2 = function.Args[3];
        object y2 = function.Args[4];

        if ((int)x1 < 0 || (int)x1 >= canvas.Size || (int)x2 < 0 || (int)x2 >= canvas.Size || (int)y1 < 0 || (int)y1 >= canvas.Size || (int)y2 < 0 || (int)y2 >= canvas.Size)
        {
            function.Value = 0;
            return;
        }

        if ((int)x1 > (int)x2)
        {
            int temp = (int)x1;
            x1 = x2;
            x2 = temp;
        }

        if ((int)y1 > (int)y2)
        {
            int temp = (int)y1;
            y1 = y2;
            y2 = temp;
        }

        int colorCount = 0;
        for (int y = (int)y1; y <= (int)y2; y++)
        {
            for (int x = (int)x1; x < (int)x2; x++)
            {
                if (canvas.Matrix[y, x] == (string)color)
                {
                    colorCount++;
                }
            }
        }
        function.Value = colorCount;
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
        if (robot.BrushColor == (string)color)
            function.Value = true;
        else
            function.Value = false;
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

        object size = function.Args[0];
        if (robot.BrushSize == (int)size)
            function.Value = true;
        else
            function.Value = false;
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

        object color = function.Args[0];
        object x = function.Args[1];
        object y = function.Args[2];

        if ((int)x < 0 || (int)x >= canvas.Size || (int)y < 0 || (int)y >= canvas.Size)
            function.Value = false;
        else
            if (canvas.Matrix[robot.Y + (int)y, robot.X + (int)x] == (string)color)
            function.Value = true;
        else
            function.Value = false;
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
        operation.Value = !(bool)operation.Right.Value;
    }

    /// <summary>
    /// Performs a negation operation on the given expression.
    /// </summary>
    /// <param name="operation">The negation operation to perform.</param>
    public void NegationOperation(NegationOperation operation)
    {
        operation.Right.Accept(this);

        int right = (int)operation.Right.Value;
        operation.Value = -right;
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
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;
        operation.Value = left + right;
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

        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        if (right == 0)
        {
            throw RuntimeException.DivisionByZero(operation.Location);
        }
        operation.Value = left / right;
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
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        if ((int)left == 0 && (int)right == 0)
        {
            throw RuntimeException.ZeroPowerZero(operation.Location);
        }
        operation.Value = (int)Math.Pow(left, right);
    }

    /// <summary>
    /// Performs a modulo operation on the given expression.
    /// </summary>
    /// <param name="operation">The modulo operation to perform.</param>
    public void ModuloOperation(ModuloOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;
        operation.Value = left % right;
    }

    /// <summary>
    /// Performs a multiplication operation on the given expression.
    /// </summary>
    /// <param name="operation">The multiplication operation to perform.</param>
    public void MultiplicationOperation(MultiplicationOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = left * right;
    }

    /// <summary>
    /// Performs a subtraction operation on the given expression.
    /// </summary>
    /// <param name="operation">The subtraction operation to perform.</param>
    public void SubstractionOperation(SubstractionOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = left - right;
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

        operation.Value = (bool)operation.Left.Value && (bool)operation.Right.Value;
    }

    /// <summary>
    /// Performs a logical OR operation on the given expression.
    /// </summary>
    /// <param name="operation">The OR operation to perform.</param>
    public void OrOperation(OrOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        operation.Value = (bool)operation.Left.Value || (bool)operation.Right.Value;
    }

    /// <summary>
    /// Performs an equality comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The equality comparison operation to perform.</param>
    public void EqualToOperation(EqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        if (operation.Left.Type == ExpressionType.String)
        {
            operation.Value = operation.Left.Equals(operation.Right.Value);
            return;
        }
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = left.Equals(right);
    }

    /// <summary>
    /// Performs a greater-than comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The greater-than comparison operation to perform.</param>
    public void GreatherThanOperation(GreatherThanOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = (int)left > (int)right;
    }

    /// <summary>
    /// Performs a greater-than-or-equal-to comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The greater-than-or-equal-to comparison operation to perform.</param>
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = (int)left >= (int)right;
    }

    /// <summary>
    /// Performs a less-than comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The less-than comparison operation to perform.</param>
    public void LessThanOperation(LessThanOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;
        operation.Value = left < right;
    }

    /// <summary>
    /// Performs a less-than-or-equal-to comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The less-than-or-equal-to comparison operation to perform.</param>
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = left <= right;
    }

    /// <summary>
    /// Performs a not-equal-to comparison operation on the given expression.
    /// </summary>
    /// <param name="operation">The not-equal-to comparison operation to perform.</param>
    public void NotEqualToOperation(NotEqualToOperation operation)
    {
        operation.Left.Accept(this);
        operation.Right.Accept(this);

        operation.Left.Accept(this);
        operation.Right.Accept(this);

        if (operation.Left.Type == ExpressionType.String)
        {
            operation.Value = !operation.Left.Equals(operation.Right);
            return;
        }
        int left = (int)operation.Left.Value;
        int right = (int)operation.Right.Value;

        operation.Value = !left.Equals(right);
    }

    #endregion;

    #endregion

    #endregion

    #region Command

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
        int x = (int)command.Args[0].Value;
        int y = (int)command.Args[1].Value;
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
                if (CheckPosition(X + x, Y + y, canvas.Size) && robot.BrushColor != "Transparent")
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
        robot.BrushColor = (string)color;
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

        if ((int)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Radio", (int)command.Args[2].Value, command.Location);
        }

        CheckDirection(command);
        object cx = command.Args[0].Value;
        object cy = command.Args[1].Value;
        object r = command.Args[2].Value;

        for (int i = 0; i < (int)r; i++)
        {
            MoveRobot((int)cx, (int)cy, command);
        }

        //Implementar algoritmo de bresenham
        int x = 0;
        int y = (int)r;
        int d = 3 - 2 * (int)r;

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

        if ((int)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Distance", (int)command.Args[2].Value, command.Location);
        }

        CheckDirection(command);
        object x = command.Args[0].Value;
        object y = command.Args[1].Value;
        object distance = command.Args[2].Value;

        for (int i = 0; i < (int)distance; i++)
        {
            DrawPixel(robot.X, robot.Y);
            MoveRobot((int)x, (int)y, command);
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

        if ((int)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Distance", (int)command.Args[2].Value, command.Location);
        }
        if ((int)command.Args[3].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Width", (int)command.Args[2].Value, command.Location);
        }
        if ((int)command.Args[4].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Height", (int)command.Args[2].Value, command.Location);
        }

        CheckDirection(command);
        object x = command.Args[0].Value;
        object y = command.Args[1].Value;
        object distance = command.Args[2].Value;

        object width = command.Args[3].Value;
        object height = command.Args[4].Value;

        Godot.GD.Print(x);
        Godot.GD.Print(y);
        Godot.GD.Print(distance);
        Godot.GD.Print(width);
        Godot.GD.Print(height);
        for (int i = 0; i < (int)distance; i++)
        {
            MoveRobot((int)x, (int)y, command);
        }

        for (int i = robot.Y - (int)width + 1; i < robot.Y + (int)width; i++)
        {
            DrawPixel(robot.Y + (int)height - 1, i);
            DrawPixel(robot.Y - (int)height + 1, i);
        }
        for (int i = robot.X - (int)height + 1; i < robot.X + (int)height; i++)
        {
            DrawPixel(i, robot.X + (int)width - 1);
            DrawPixel(i, robot.X - (int)width + 1);
        }
    }

    /// <summary>
    /// Fills a region on the canvas with the robot's current brush color, starting from the robot's current position.
    /// </summary>
    /// <param name="command">The fill command to execute.</param>
    public void FillCommand(FillCommand command)
    {
        string currentColor = canvas.Matrix[robot.Y, robot.X];
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
        object size = command.Args[0].Value;
        robot.BrushSize = (int)size;
    }

    /// <summary>
    /// Sets the initial position of the robot on the canvas.
    /// </summary>
    /// <param name="command">The spawn command to execute.</param>
    public void SpawnCommand(SpawnCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        object x = command.Args[0].Value;
        object y = command.Args[1].Value;
        robot.X = (int)x;
        robot.Y = (int)y;
        RobotOutException(robot.X, robot.Y, canvas.Size, command);
    }

    /// <summary>
    /// Jumps to a specific label in the code if the given condition is true.
    /// </summary>
    /// <param name="command">The go-to command to execute.</param>
    /// <exception cref="RuntimeException">Thrown when a possibly infinite loop is detected.</exception>
    public void GoToCommand(GoToCommand command)
    {
        if (command.InfinteCycle < 1000000) //10**6
        {
            foreach (Expression arg in command.Args)
            {
                arg.Accept(this);
            }
            if ((bool)command.Args[0].Value)
            {
                Label? label = scope.GetLabel(command.Label);
                Index = label.CommandIndicator - 1;
            }
            command.InfinteCycle++;
        }
        else
        {
            throw new RuntimeException("Possibly infinite Loop at ", command.Location, "GoTo");
        }
    }



    public void PrintCommand(PrintCommand command)
    {
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        ConsoleMessages.Add(command.Args[0].Value.ToString());
    }
    #endregion
}