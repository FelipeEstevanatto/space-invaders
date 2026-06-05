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
        private readonly Form _screen;
        private Random random = new Random();

        public Game(PlayerShip player, Form screen)
        {
            Player = player;
            Aliens = new List<Alien>();
            Projectiles = new List<Projectile>();
            _screen = screen;
        }

        // Evento para avisar o form que um alien quer atirar
        public event Action<int, int> OnAlienShot;

        // Método chamado pelo Timer do Form1
        public void Update(CollisionManager collisionManager)
        {
            // 1. Move todos os projéteis
            foreach (var p in Projectiles)
            {
                p.Move();

                // Destrói o projétil se sair da screen, permitindo que o player atire novamente
                if (p.Y + Projectile.Height < 0 || p.Y > _screen.ClientSize.Height)
                {
                    collisionManager.removeProjectiles.Add(p);
                }
            }

            // 2. Move Aliens
            bool hasHitEdge = false;
            foreach (var alien in Aliens)
            {
                alien.Move();
                if (alien.Bounds.Right >= _screen.ClientSize.Width || alien.Bounds.Left <= 0)
                {
                    hasHitEdge = true;
                }
            }

            if (hasHitEdge)
            {
                foreach (var alien in Aliens)
                    alien.ReverseDirectionAndDescend();
            }

            // Lógica de tiro aleatório dos aliens
            if (Aliens.Count > 0)
            {
                // Verifica tempo (por ex. a cada frame tem 2% de chance de um alien atirar)
                if (random.Next(0, 100) < 2)
                {
                    // Sorteia um alien para atirar
                    int randomIndex = random.Next(0, Aliens.Count);
                    Alien alienShooter = Aliens[randomIndex];

                    // Calcula de onde sai o tiro
                    int xTiro = alienShooter.Bounds.Left + (Alien.AlienWidth / 2);
                    int yTiro = alienShooter.Bounds.Bottom;

                    // Avisa o Form para desenhar o tiro na screen
                    OnAlienShot?.Invoke(xTiro, yTiro);
                }
            }
        }
        public void AddPoints(int points)
        {
            Score += points;
        }
    }
}
