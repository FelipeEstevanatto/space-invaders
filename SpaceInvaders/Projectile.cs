using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class Projectile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsFromPlayer { get; private set; }
        public int Speed { get; private set; }
        public const int Width = 5;
        public const int Height = 15;
        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public Projectile(int x, int y, bool isFromPlayer)
        {
            X = x;
            Y = y;
            IsFromPlayer = isFromPlayer;
            Speed = isFromPlayer ? -15 : 10; // Sobe se for do player, desce se for do alien
        }

        // Método de comportamento
        public void Move()
        {
            Y += Speed;
        }
    }
}
