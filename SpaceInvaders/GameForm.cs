using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        // Declaração dos nossos objetos (Associação)
        private Jogo meuJogo;
        private GerenciadorDeColisoes gerenciadorColisoes;
        private Timer gameTimer;

        // Controles de movimentação contínua
        private bool indoEsquerda;
        private bool indoDireita;

        private Label lblScore;
        private List<PictureBox> coracoesVidas;
        private readonly Image imagemAlien = Properties.Resources.alien_png;
        private readonly Image imagemCoracao = Properties.Resources.red_heart;
        private readonly Image imagemNave = Properties.Resources.nave_png;

        // Controle para evitar que o segurar espaço crie muitos tiros ou fique engasgando
        private bool espacoPressionado;

        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Aceita setas ou A/D conforme requisito
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                indoEsquerda = true;

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                indoDireita = true;

            // Disparo do jogador (Garante que só atira uma vez por aperto de tecla)
            if (e.KeyCode == Keys.Space && !espacoPressionado)
            {
                espacoPressionado = true;
                // Limita a 3 tiros simultâneos na tela para evitar "metralhadora"
                int tirosAtivos = meuJogo.Projeteis.Count(p => p.EhDoJogador);
                if (tirosAtivos < 3)
                {
                    CriarTiroJogador();
                }
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                indoEsquerda = false;

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                indoDireita = false;

            if (e.KeyCode == Keys.Space)
                espacoPressionado = false;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            // Instancia a nave do jogador a partir da posição do PictureBox do designer
            NaveJogador jogador = new NaveJogador(pctShip.Location.X, pctShip.Location.Y);
            // O desenho ficará por nossa conta agora; escondemos o controle do designer
            pctShip.Visible = false;

            // Instancia o motor do jogo e o gerenciador de colisões
            meuJogo = new Jogo(jogador, this);
            meuJogo.OnAlienAtirou += CriarTiroAlien;
            gerenciadorColisoes = new GerenciadorDeColisoes();

            // Gera a matriz 3x5 de alienígenas obrigatória do escopo
            GerarInimigos();

            // NEW: Call the method to draw the UI
            CriarInterfaceDeUsuario();

            // Configura o Game Loop (O coração do jogo)
            gameTimer = new Timer();
            gameTimer.Interval = 16; // Roda a ~60 FPS (Quadros por segundo)
            gameTimer.Tick += timerClock_Tick;
            gameTimer.Start();
        }

        private void CriarInterfaceDeUsuario()
        {
            // 1. Create the Score Label dynamically
            lblScore = new Label();
            lblScore.Text = "Score: 0";
            lblScore.ForeColor = Color.White; // Make sure your Form BackColor is Black!
            lblScore.Font = new Font("Consolas", 14, FontStyle.Bold);
            lblScore.AutoSize = true;
            lblScore.Location = new Point(10, 10); // Top left corner
            this.Controls.Add(lblScore);

            // 2. Create the 3 Heart Images dynamically
            coracoesVidas = new List<PictureBox>();
            for (int i = 0; i < 3; i++)
            {
                PictureBox pctCoracao = new PictureBox();
                pctCoracao.Size = new Size(30, 30);
                pctCoracao.BackColor = Color.Transparent;
                pctCoracao.SizeMode = PictureBoxSizeMode.StretchImage;

                // CHANGE "nome_da_imagem_do_coracao" to your actual resource name!
                pctCoracao.Image = imagemCoracao;

                // Position them in the top right corner, spaced out
                int posX = this.ClientSize.Width - 40 - (i * 35);
                pctCoracao.Location = new Point(posX, 10);

                this.Controls.Add(pctCoracao);
                coracoesVidas.Add(pctCoracao);
            }
        }

        private void GerarInimigos()
        {
            int colunas = 5;
            int linhas = 3;
            int espacamentoX = 60;
            int espacamentoY = 50;
            int margemEsquerda = 50;
            int margemTopo = 30;

            for (int linha = 0; linha < linhas; linha++)
            {
                for (int coluna = 0; coluna < colunas; coluna++)
                {
                    int x = margemEsquerda + (coluna * espacamentoX);
                    int y = margemTopo + (linha * espacamentoY);
                    meuJogo.Aliens.Add(new Alien(x, y));
                }
            }
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            // 1. Movimenta o Jogador de forma suave
            if (indoEsquerda) meuJogo.Jogador.MoverEsquerda();
            if (indoDireita) meuJogo.Jogador.MoverDireita(this.ClientSize.Width);

            // 2. Atualiza a posição de tiros e aliens
            meuJogo.Atualizar(gerenciadorColisoes);

            // 3. Checa quem bateu em quem (e passa o Form para remover imagens da tela)
            gerenciadorColisoes.ChecarColisoes(meuJogo);

            lblScore.Text = "Score: " + meuJogo.Pontuacao;

            // NEW: Update the Hearts based on the player's remaining lives
            // If they have 2 lives, only index 0 and 1 remain visible.
            for (int i = 0; i < coracoesVidas.Count; i++)
            {
                coracoesVidas[i].Visible = i < meuJogo.Jogador.Vidas;
            }

            foreach (var alienToRemove in gerenciadorColisoes.aliensRemover)
            {
               meuJogo.Aliens.Remove(alienToRemove);
            }
            gerenciadorColisoes.aliensRemover.Clear();

            foreach(var projToRemove in gerenciadorColisoes.projeteisRemover)
            {
               meuJogo.Projeteis.Remove(projToRemove);
            }
            gerenciadorColisoes.projeteisRemover.Clear();

                Invalidate();

            // 4. Valida Condições de Vitória e Derrota
            if (meuJogo.Jogador.Vidas <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show($"GAME OVER! Score final: {meuJogo.Pontuacao}", "Derrota");
                Application.Exit();
            }
            else if (meuJogo.Aliens.Count == 0)
            {
                gameTimer.Stop();
                MessageBox.Show($"VITÓRIA! Score final: {meuJogo.Pontuacao}", "Vitória");
                Application.Exit();
            }
        }

        // (Removido daqui e passado para baixo, para juntar se quiser)


        private void CriarTiroJogador()
        {
            var shipBounds = meuJogo.Jogador.Bounds;
            int xTiro = shipBounds.Left + (shipBounds.Width / 2) - (Projetil.Largura / 2);
            int yTiro = shipBounds.Top - Projetil.Altura;

            // Registra na lógica (true significa que é tiro do jogador)
            meuJogo.Projeteis.Add(new Projetil(xTiro, yTiro, true));
        }

        private void CriarTiroAlien(int xInicial, int yInicial)
        {
            // Limita a quantidade de tiros dos aliens na tela ao mesmo tempo também (opcional)
            int tirosAtivos = meuJogo.Projeteis.Count(p => !p.EhDoJogador);
            if (tirosAtivos >= 4) return;

            // false significa que é tiro do inimigo
            meuJogo.Projeteis.Add(new Projetil(xInicial - (Projetil.Largura / 2), yInicial, false));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var alien in meuJogo?.Aliens ?? Enumerable.Empty<Alien>())
            {
                e.Graphics.DrawImage(imagemAlien, alien.Bounds);
            }

            if (meuJogo == null)
            {
                return;
            }

            // Desenha a nave do jogador
            e.Graphics.DrawImage(imagemNave, meuJogo.Jogador.Bounds);

            foreach (var projetil in meuJogo.Projeteis)
            {
                Brush brush = projetil.EhDoJogador ? Brushes.Yellow : Brushes.LimeGreen;
                e.Graphics.FillRectangle(brush, projetil.Bounds);
            }
        }
    }
}
