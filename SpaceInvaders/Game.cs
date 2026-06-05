using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class Game
    {
        public PlayerShip Player { get; private set; }
        public List<Alien> Aliens { get; private set; }
        public List<Projectile> Projectiles { get; private set; }
        public int Score { get; private set; } = 0;
        private readonly Form _tela;
        private Random random = new Random();

        public Game(PlayerShip player, Form tela)
        {
            Player = player;
            Aliens = new List<Alien>();
            Projectiles = new List<Projectile>();
            _tela = tela;
        }

        // Evento para avisar o form que um alien quer atirar
        public event Action<int, int> OnAlienAtirou;

        // Método chamado pelo Timer do Form1
        public void Atualizar(CollisionManager gerenciadorColisoes)
        {
            // 1. Move todos os projéteis
            foreach (var p in Projectiles)
            {
                p.Move();

                // Destrói o projétil se sair da tela, permitindo que o player atire novamente
                if (p.Y + Projectile.Altura < 0 || p.Y > _tela.ClientSize.Height)
                {
                    gerenciadorColisoes.projeteisRemover.Add(p);
                }
            }

            // 2. Move Aliens
            bool bateuNaBorda = false;
            foreach (var alien in Aliens)
            {
                alien.Move();
                if (alien.Bounds.Right >= _tela.ClientSize.Width || alien.Bounds.Left <= 0)
                {
                    bateuNaBorda = true;
                }
            }

            if (bateuNaBorda)
            {
                foreach (var alien in Aliens)
                    alien.InverterDirecaoEDescer();
            }

            // Lógica de tiro aleatório dos aliens
            if (Aliens.Count > 0)
            {
                // Verifica tempo (por ex. a cada frame tem 2% de chance de um alien atirar)
                if (random.Next(0, 100) < 2)
                {
                    // Sorteia um alien para atirar
                    int indexSorteado = random.Next(0, Aliens.Count);
                    Alien alienAtirador = Aliens[indexSorteado];

                    // Calcula de onde sai o tiro
                    int xTiro = alienAtirador.Bounds.Left + (Alien.AlienWidth / 2);
                    int yTiro = alienAtirador.Bounds.Bottom;

                    // Avisa o Form para desenhar o tiro na tela
                    OnAlienAtirou?.Invoke(xTiro, yTiro);
                }
            }
        }
        public void AdicionarPontos(int pontos)
        {
            Score += pontos;
        }
    }
}
