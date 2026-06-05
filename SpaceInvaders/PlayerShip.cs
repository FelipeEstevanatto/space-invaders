using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Drawing;

namespace SpaceInvaders
{
    internal class PlayerShip
    {
        public int Lives { get; private set; } = 3;

        public int X { get; private set; }
        public int Y { get; private set; }

        public const int Width = 157;
        public const int Height = 112;

        public int Speed { get; private set; } = 10;

        public PlayerShip(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void MoveLeft()
        {
            if (X > 0)
                X -= Speed;
        }

        public void MoveRight(int limiteTela)
        {
            if (X + Width < limiteTela)
                X += Speed;
        }

        public void LoseLife()
        {
            Lives--;
        }

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);
    }
}
