using System;

namespace PixelWallE.Core;

    /// <summary>
    /// Represents a canvas for drawing pixels.
    /// </summary>
    public class Canvas  
    {
        /// <summary>
        /// Gets the size of the canvas.
        /// </summary>
        public int Size { get; private set; }
        /// <summary>
        /// Gets or sets the matrix representing the canvas.
        /// </summary>
        public PixelColor[,] Matrix { get;  set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class.
        /// </summary>
        /// <param name="size">The size of the canvas.</param>
        public Canvas(int size)
        {
            Size = size;
            Matrix = new PixelColor[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Matrix[x, y] = new PixelColor(255,255,255);
                }

            }
        }

        
    }

