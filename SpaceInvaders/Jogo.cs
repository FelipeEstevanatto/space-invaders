using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    internal class Jogo
    {
        public NaveJogador Jogador { get; private set; }
        public List<Alien> Aliens { get; private set; }
        public List<Projetil> Projeteis { get; private set; }
        public int Pontuacao { get; private set; } = 0;
        private Form _tela;
        private Random random = new Random();

        public Jogo(NaveJogador jogador, Form tela)
        {
            Jogador = jogador;
            Aliens = new List<Alien>();
            Projeteis = new List<Projetil>();
            _tela = tela;
        }

        // Evento para avisar o form que um alien quer atirar
        public event Action<int, int> OnAlienAtirou;

        // Método chamado pelo Timer do Form1
        public void Atualizar(GerenciadorDeColisoes gerenciadorColisoes)
        {
            // 1. Mover todos os projéteis
            foreach (var p in Projeteis)
            {
                p.Mover();

                // Destrói o projétil se sair da tela, permitindo que o jogador atire novamente
                if (p.Sprite.Bottom < 0 || p.Sprite.Top > _tela.ClientSize.Height)
                {
                    gerenciadorColisoes.projeteisRemover.Add(p);
                }
            }

            // 2. Mover Aliens
            bool bateuNaBorda = false;
            foreach (var alien in Aliens)
            {
                alien.Mover();
                if (alien.Sprite.Right >= _tela.ClientSize.Width || alien.Sprite.Left <= 0)
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
                    int xTiro = alienAtirador.Sprite.Location.X + (alienAtirador.Sprite.Width / 2);
                    int yTiro = alienAtirador.Sprite.Bottom;

                    // Avisa o Form para desenhar o tiro na tela
                    OnAlienAtirou?.Invoke(xTiro, yTiro);
                }
            }
        }
        public void AdicionarPontos(int pontos)
        {
            Pontuacao += pontos;
        }
    }
}
