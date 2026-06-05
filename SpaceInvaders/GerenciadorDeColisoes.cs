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
            // Em vez de limpar e repopular, vamos usar a lista que já pode conter coisas
            // vindas do método Jogo.Atualizar()

            foreach (var projetil in jogo.Projeteis.ToList())
            {
                // Colisão: Tiro do Jogador -> Alien
                if (projetil.EhDoJogador)
                {
                    foreach (var alien in jogo.Aliens.ToList())
                    {
                        if (projetil.Sprite.Bounds.IntersectsWith(alien.Sprite.Bounds))
                        {
                            jogo.AdicionarPontos(10);
                            projeteisRemover.Add(projetil);
                            aliensRemover.Add(alien);
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

            // Fazer a limpeza visual e lógica
            // (Você precisará remover os PictureBoxes do Form e os objetos das Listas)
        }
    }
}
