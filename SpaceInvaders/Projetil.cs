using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class Projetil
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool EhDoJogador { get; private set; }
        public int Velocidade { get; private set; }
        public PictureBox Sprite { get; private set; }

        public Projetil(int x, int y, bool ehDoJogador, PictureBox sprite)
        {
            X = x;
            Y = y;
            EhDoJogador = ehDoJogador;
            Sprite = sprite;
            Velocidade = ehDoJogador ? -15 : 10; // Sobe se for do jogador, desce se for do alien
            Sprite.Location = new Point(X, Y);
        }

        // Método de comportamento
        public void Mover()
        {
            Y += Velocidade;
            Sprite.Top = Y;
        }
    }
}
