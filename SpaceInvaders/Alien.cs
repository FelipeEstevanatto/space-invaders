using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class Alien
    {
        public PictureBox Sprite { get; private set; }
        public int VelocidadeX { get; private set; } = 5;

        public Alien(PictureBox sprite)
        {
            Sprite = sprite;
        }

        public void Mover()
        {
            // Lógica de zigue-zague ou descida constante
            Sprite.Left += VelocidadeX;
        }

        public void InverterDirecaoEDescer()
        {
            VelocidadeX = -VelocidadeX;
            Sprite.Top += 30; // Desce em direção ao jogador
        }
    }
}
