using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    internal class CollisionManager
    {
        public List<Projectile> removeProjectiles = new List<Projectile>();
        public List<Alien> removeAliens = new List<Alien>();

        public void ChecarColisoes(Game jogo)
        {
            // As listas só são lidas aqui; a remoção real acontece depois no Form.
            foreach (var projectile in jogo.Projectiles)
            {
                // Colisão: Tiro do Player -> Alien
                if (projectile.IsFromPlayer)
                {
                    foreach (var alien in jogo.Aliens)
                    {
                        if (projectile.Bounds.IntersectsWith(alien.Bounds))
                        {
                            jogo.AddPoints(10);
                            removeProjectiles.Add(projectile);
                            removeAliens.Add(alien);
                            break;
                        }
                    }
                }
                // Colisão: Tiro do Alien -> Player
                else
                {
                    if (projectile.Bounds.IntersectsWith(jogo.Player.Bounds))
                    {
                        jogo.Player.LoseLife();
                        removeProjectiles.Add(projectile);
                    }
                }
            }
        }
    }
}
