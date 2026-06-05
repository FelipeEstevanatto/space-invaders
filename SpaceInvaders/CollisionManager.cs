using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    internal class CollisionManager
    {
        public List<Projectile> projeteisRemover = new List<Projectile>();
        public List<Alien> aliensRemover = new List<Alien>();

        public void ChecarColisoes(Game jogo)
        {
            // As listas só são lidas aqui; a remoção real acontece depois no Form.
            foreach (var projetil in jogo.Projectiles)
            {
                // Colisão: Tiro do Player -> Alien
                if (projetil.IsFromPlayer)
                {
                    foreach (var alien in jogo.Aliens)
                    {
                        if (projetil.Bounds.IntersectsWith(alien.Bounds))
                        {
                            jogo.AdicionarPontos(10);
                            projeteisRemover.Add(projetil);
                            aliensRemover.Add(alien);
                            break;
                        }
                    }
                }
                // Colisão: Tiro do Alien -> Player
                else
                {
                    if (projetil.Bounds.IntersectsWith(jogo.Player.Bounds))
                    {
                        jogo.Player.PerderVida();
                        projeteisRemover.Add(projetil);
                    }
                }
            }
        }
    }
}
