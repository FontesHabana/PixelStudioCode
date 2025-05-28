using System;

namespace PixelWallE.Core
{
    public class Canvas  
    {
        public int Size { get; private set; }
        public string[,] Matrix { get;  set; }


        public Canvas(int size)
        {
            Size = size;
            Matrix = new string[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Matrix[x, y] = "White";
                }

            }
        }

        
    }
}
