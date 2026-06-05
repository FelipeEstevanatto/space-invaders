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
        public int Vidas { get; private set; } = 3;

        public int X { get; private set; }
        public int Y { get; private set; }

        public const int Largura = 157;
        public const int Altura = 112;

        public int Velocidade { get; private set; } = 10;

        public PlayerShip(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void MoveLeft()
        {
            if (X > 0)
                X -= Velocidade;
        }

        public void MoveRight(int limiteTela)
        {
            if (X + Largura < limiteTela)
                X += Velocidade;
        }

        public void PerderVida()
        {
            Vidas--;
        }

        public Rectangle Bounds => new Rectangle(X, Y, Largura, Altura);
    }
}
