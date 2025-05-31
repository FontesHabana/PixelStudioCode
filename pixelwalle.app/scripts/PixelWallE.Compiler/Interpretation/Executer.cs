
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

public class Executer : IVisitor<ASTNode>
{
    private Scope scope;
    public List<PixelWallEException> errors;

    private Canvas canvas;
    private RobotState robot;
    private int Index { get; set; }

    public Executer(Scope scope, Canvas canvas, RobotState robot, List<PixelWallEException> error)
    {
        this.scope = scope;
        this.canvas = canvas;
        this.robot = robot;

        errors = error;
        Index = 0;
    }





 


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
    public void ParenthesizedExpression(ParenthesizedExpression expression)
    {
        expression.InnerExpression.Accept(this);

        expression.Value = expression.InnerExpression.Value;

    }
    public void Variable(Variable var)
    {
       var.Value=scope.GetVariable(var.VariableName);
    }
    #region functions
    //Functions
    public void GetActualXFunction(GetActualXFunction function)
    {
        function.Value = robot.X;
    }
    public void GetActualYFunction(GetActualYFunction function)
    {
        function.Value = robot.Y;
    }
    public void GetCanvasSizeFunction(GetCanvasSizeFunction function)
    {   
        function.Value = canvas.Size;
    }
    public void GetColorCountFunction(GetColorCountFunction function)
    {    foreach (Expression item in function.Args)
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
    public void IsBrushColorFunction(IsBrushColorFunction function){
         foreach (Expression item in function.Args)
        {
            item.Accept(this);
          
        }
        object color = function.Args[0].Value;
        if (robot.BrushColor == (string)color) 
            function.Value= true;
        else
            function.Value = false;
        
    }
    public void IsBrushSizeFunction(IsBrushSizeFunction function){
         foreach (Expression item in function.Args)
        {
            item.Accept(this);
          
        }
           object size = function.Args[0];
        if (robot.BrushSize == (int)size) 
            function.Value= true;
        else
            function.Value = false;
        
    }
    public void IsCanvasColor(IsCanvasColor function){
        foreach (Expression item in function.Args)
        {
            item.Accept(this);
           
        }
           object color = function.Args[0];
           object x = function.Args[1];
           object y = function.Args[2];
        if ((int)x < 0 || (int)x >= canvas.Size || (int)y < 0 || (int)y >= canvas.Size)
            function.Value= false;
        else
            if (canvas.Matrix[robot.Y+(int)y,robot.X+(int)x] == (string)color) 
                function.Value= true;
            else
                function.Value = false;
            
    }
    public void IsColorFunction(IsColorFunction function){}
    #endregion

    
    #region  Unary Expressions
    public void NotOperation(NotOperation operation){
        operation.Right.Accept(this);
        operation.Value = !(bool)operation.Right.Value;
    }
    public void NegationOperation(NegationOperation operation){
        operation.Right.Accept(this);

        int right=(int)operation.Right.Value;
        operation.Value=-right;
    }
    #endregion

    #region Binary Expressions

    #region Arithmetic Operation
    public void AdditionOperation(AdditionOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left=(int)operation.Left.Value;
        int  right=(int)operation.Right.Value;
        operation.Value=left+right;
    }
    public void DivisionOperation(DivisionOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
       
       int left=(int)operation.Left.Value;
       int  right=(int)operation.Right.Value;
        
            if (right == 0)
            {
                throw RuntimeException.DivisionByZero(operation.Location);
            }
        operation.Value=left/right;
    }
    public void ExponentiationOperation(ExponentiationOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
         int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        
        if ((int)left==0&&(int)right==0)
        {
            throw RuntimeException.ZeroPowerZero(operation.Location);
        }
        operation.Value=(int)Math.Pow(left,right);
    }
       public void ModuloOperation(ModuloOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
       
         int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        operation.Value=left%right;
       }
    public void MultiplicationOperation(MultiplicationOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
       
        int left=(int)operation.Left.Value;
       int  right=(int)operation.Right.Value;
        
        operation.Value=left*right;
    }
    public void SubstractionOperation(SubstractionOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
       
         int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        
        operation.Value=left-right;
    }
    #endregion

    #region Logic Operation
    public void ANDOperation(ANDOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);

       
        operation.Value=(bool)operation.Left.Value&&(bool)operation.Right.Value;
    }
    public void OrOperation(OrOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
     
        operation.Value=(bool)operation.Left.Value||(bool)operation.Right.Value;
    }
    public void EqualToOperation(EqualToOperation operation){
         operation.Left.Accept(this);
         operation.Right.Accept(this);
        
        if (operation.Left.Type==ExpressionType.String)
        {
            operation.Value=operation.Left.Equals(operation.Right.Value);
            return;
        }
        int left=(int)operation.Left.Value;
        int  right=(int)operation.Right.Value;
        
        operation.Value=left.Equals(right);
    }
    public void GreatherThanOperation(GreatherThanOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
        int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        
        
        operation.Value=(int)left>(int)right;
    }
    public void GreatherThanOrEqualToOperation(GreatherThanOrEqualToOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
          int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        
        
        operation.Value=(int)left>=(int)right;
    }
    public void LessThanOperation(LessThanOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
         int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        operation.Value=left<right;
       
    }
    public void LessThanOrEqualToOperation(LessThanOrEqualToOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
      
       int left=(int)operation.Left.Value;
         int  right=(int)operation.Right.Value;
        
        operation.Value=left<=right;
    }
    public void NotEqualToOperation(NotEqualToOperation operation){
        operation.Left.Accept(this);
        operation.Right.Accept(this);
      
           operation.Left.Accept(this);
         operation.Right.Accept(this);
         
        
        if (operation.Left.Type==ExpressionType.String)
        {
            operation.Value=!operation.Left.Equals(operation.Right);
            return;
        }
        int left=(int)operation.Left.Value;
        int  right=(int)operation.Right.Value;
        
        operation.Value=!left.Equals(right);
    }

     #endregion;
    #endregion

    #endregion

     #region Command
    private bool CheckPosition(int x, int y, int size){
        if (x<0||x>=size||y<0||y>=size)
        {
            return false;
        }
        return true;
    }
    private void CheckDirection(Command command)
    {
        int x = (int)command.Args[0].Value;
        int y = (int)command.Args[1].Value;
        if (x != 1 && x != -1 && x != 0 )
        {
            throw RuntimeException.InvalidDirectionCoordinates(x, y, command.Name, command.Location);
        }
        if ( y != 1 && y != -1 && y != 0)
        {
             throw RuntimeException.InvalidDirectionCoordinates(x, y, command.Name, command.Location);
        }
    }
    private void RobotOutException(int x, int y, int size, ASTNode node){
        if (!CheckPosition(x,y,size))
        {
            throw RuntimeException.PositionOutOfBounds(x,y,node.ToString(), node.Location);
        }
    }
    private void DrawPixel(int X, int Y){
        for (int y = 0; y< robot.BrushSize; y++)
        {
            for (int x = 0; x < robot.BrushSize; x++)
            {
                if (CheckPosition(X+x ,Y+ y, canvas.Size)&& robot.BrushColor!="Transparent")
                {
                      canvas.Matrix[Y+y,X+x]=robot.BrushColor;
                }
              
            }
        }
    }
    private void MoveRobot(int x, int y, ASTNode node){
        robot.X+=x;
        robot.Y+=y;
        RobotOutException(robot.X, robot.Y, canvas.Size, node);
    }
    public void AssigmentExpression(AssigmentExpression command){
        command.Argument.Accept(this);
        scope.AssignVariable(command.Var.VariableName,command.Argument.Value,command.Argument.Type);
    }
    public void ColorCommand(ColorCommand command){
         foreach (Expression item in command.Args)
        {
            item.Accept(this);
        }
        object color=command.Args[0].Value;
        robot.BrushColor=(string)color;
    }
    public void DrawCircleCommand(DrawCircleCommand command){
        foreach (Expression item in command.Args)
        {
            item.Accept(this);
           
        }
        if ((int)command.Args[2].Value <= 0)
        {
            throw RuntimeException.ArgumentMostBePositive("Radio", (int)command.Args[2].Value, command.Location);
        
        }
        CheckDirection(command);
        object cx=command.Args[0].Value;
        object cy=command.Args[1].Value;
        object r=command.Args[2].Value;

        for (int i = 0; i < (int)r; i++)
        {
            MoveRobot((int)cx,(int)cy, command);
        }
        //Implementar algoritmo de bresenham
        int x = 0;
        int y = (int)r;
        int d = 3 - 2 * (int)r;

        while (x <= y)
        {
            // Dibujar los ocho octantes
            DrawPixel( robot.X + x, robot.Y + y);
            DrawPixel( robot.X - x, robot.Y + y);
            DrawPixel( robot.X + x, robot.Y - y);
            DrawPixel( robot.X - x, robot.Y - y);
            DrawPixel( robot.X + y, robot.Y + x);
            DrawPixel( robot.X - y, robot.Y + x);
            DrawPixel( robot.X + y, robot.Y - x);
            DrawPixel( robot.X - y, robot.Y - x);

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
    public void DrawLineCommand(DrawLineCommand command){
         foreach (Expression item in command.Args)
        {
            item.Accept(this);
        
        }
        if ((int)command.Args[2].Value <= 0)
        {
           throw RuntimeException.ArgumentMostBePositive("Distance", (int)command.Args[2].Value, command.Location);
           
        }
        CheckDirection(command);
        object x=command.Args[0].Value;
        object y=command.Args[1].Value;
        object distance=command.Args[2].Value;
        for (int i = 0; i < (int)distance; i++)
        {
            DrawPixel(robot.X,robot.Y);
            MoveRobot((int)x,(int)y, command);
        }


    }
    public void DrawRectangleCommand(DrawRectangleCommand command)
    {   Godot.GD.Print("empiezzo a dibujar");
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
       
        for (int i = robot.Y-(int)width+1; i < robot.Y+(int)width; i++)
        {
            DrawPixel(robot.Y+(int)height-1,i);
            DrawPixel(robot.Y-(int)height+1,i);  
        }
        for (int i = robot.X-(int)height+1; i < robot.X+(int)height; i++)
        {
            DrawPixel(i,robot.X+(int)width-1);
            DrawPixel(i,robot.X-(int)width+1);  
        }
    }
    public void FillCommand(FillCommand command){
        string currentColor=canvas.Matrix[robot.Y,robot.X];
        Stack<(int,int)> cells=new Stack<(int,int)>();
        List<(int,int)> visited= new List<(int, int)>();
        cells.Push((robot.Y,robot.X));
        visited.Add((robot.Y,robot.X));
        
        
        while (cells.Count>0)
        {
            (int y, int x) currentCell=cells.Pop();
            if (canvas.Matrix[currentCell.y,currentCell.x]==currentColor)
            {
                canvas.Matrix[currentCell.y,currentCell.x]=robot.BrushColor;
                if (currentCell.x+1<canvas.Size && canvas.Matrix[currentCell.y,currentCell.x+1]==currentColor)
                {
                    if (!visited.Contains((currentCell.y,currentCell.x+1)))
                    {
                        cells.Push((currentCell.y,currentCell.x+1));
                        visited.Add((currentCell.y,currentCell.x+1));
                    }
                }
                if (currentCell.x-1>=0 && canvas.Matrix[currentCell.y,currentCell.x-1]==currentColor)
                {
                    if (!visited.Contains((currentCell.y,currentCell.x-1)))
                    {
                        cells.Push((currentCell.y,currentCell.x-1));
                        visited.Add((currentCell.y,currentCell.x-1));
                    }
                }
                if (currentCell.y+1<canvas.Size && canvas.Matrix[currentCell.y+1,currentCell.x]==currentColor)
                {
                    if (!visited.Contains((currentCell.y+1,currentCell.x)))
                    {
                        cells.Push((currentCell.y+1,currentCell.x));
                        visited.Add((currentCell.y+1,currentCell.x));
                    }
                }
                if (currentCell.y-1>=0 && canvas.Matrix[currentCell.y-1,currentCell.x]==currentColor)
                {
                    if (!visited.Contains((currentCell.y-1,currentCell.x)))
                    {
                        cells.Push((currentCell.y-1,currentCell.x));
                        visited.Add((currentCell.y-1,currentCell.x));
                    }
                }
            }
        }


    }
    public void SizeCommand(SizeCommand command){
         foreach (Expression item in command.Args)
        {
            item.Accept(this);
           
        }
        object size=command.Args[0].Value;
        robot.BrushSize=(int)size;
    }
    public void SpawnCommand(SpawnCommand command){
         foreach (Expression item in command.Args)
        {
            item.Accept(this);
           
        }
        object x=command.Args[0].Value;
        object y=command.Args[1].Value;
        robot.X=(int)x;
        robot.Y=(int)y;
        RobotOutException(robot.X,robot.Y,canvas.Size,command);
    }
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
    #endregion


}