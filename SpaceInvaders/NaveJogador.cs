using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class NaveJogador
    {
        public int Vidas { get; private set; } = 3;
        public PictureBox Sprite { get; private set; }
        public int Velocidade { get; private set; } = 10;

        public NaveJogador(PictureBox sprite)
        {
            Sprite = sprite;
        }

        public void MoverEsquerda()
        {
            if (Sprite.Left > 0)
                Sprite.Left -= Velocidade;
        }

        public void MoverDireita(int limiteTela)
        {
            if (Sprite.Right < limiteTela)
                Sprite.Left += Velocidade;
        }

        public void PerderVida()
        {
            Vidas--;
        }

        public Rectangle Bounds => Sprite.Bounds;
    }
}
