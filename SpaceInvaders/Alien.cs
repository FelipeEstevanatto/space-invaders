using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class Alien
    {
        public const int Largura = 40;
        public const int Altura = 40;

        public int X { get; private set; }
        public int Y { get; private set; }
        public Rectangle Bounds => new Rectangle(X, Y, Largura, Altura);
        public int VelocidadeX { get; private set; } = 5;

        public Alien(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Mover()
        {
            X += VelocidadeX;
        }

        public void InverterDirecaoEDescer()
        {
            VelocidadeX = -VelocidadeX;
            Y += 30;
        }
    }
}
