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
        public int Velocidade { get; private set; }
        public const int Largura = 5;
        public const int Altura = 15;
        public Rectangle Bounds => new Rectangle(X, Y, Largura, Altura);

        public Projectile(int x, int y, bool isFromPlayer)
        {
            X = x;
            Y = y;
            IsFromPlayer = isFromPlayer;
            Velocidade = isFromPlayer ? -15 : 10; // Sobe se for do player, desce se for do alien
        }

        // Método de comportamento
        public void Move()
        {
            Y += Velocidade;
        }
    }
}
