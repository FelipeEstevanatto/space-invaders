using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    internal class GerenciadorDeColisoes
    {
        public List<Projetil> projeteisRemover = new List<Projetil>();
        public List<Alien> aliensRemover = new List<Alien>();

        public void ChecarColisoes(Jogo jogo)
        {
            // As listas só são lidas aqui; a remoção real acontece depois no Form.
            foreach (var projetil in jogo.Projeteis)
            {
                // Colisão: Tiro do Jogador -> Alien
                if (projetil.EhDoJogador)
                {
                    foreach (var alien in jogo.Aliens)
                    {
                        if (projetil.Sprite.Bounds.IntersectsWith(alien.Sprite.Bounds))
                        {
                            jogo.AdicionarPontos(10);
                            projeteisRemover.Add(projetil);
                            aliensRemover.Add(alien);
                            break;
                        }
                    }
                }
                // Colisão: Tiro do Alien -> Jogador
                else
                {
                    if (projetil.Sprite.Bounds.IntersectsWith(jogo.Jogador.Sprite.Bounds))
                    {
                        jogo.Jogador.PerderVida();
                        projeteisRemover.Add(projetil);
                    }
                }
            }
        }
    }
}
