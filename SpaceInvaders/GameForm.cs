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
        private Game myGame;
        private CollisionManager gerenciadorColisoes;
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

            // Disparo do player (Garante que só atira uma vez por aperto de tecla)
            if (e.KeyCode == Keys.Space && !espacoPressionado)
            {
                espacoPressionado = true;
                // Limita a 3 tiros simultâneos na tela para evitar "metralhadora"
                int tirosAtivos = myGame.Projectiles.Count(p => p.IsFromPlayer);
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
            // Instancia a nave do player a partir da posição do PictureBox do designer
            PlayerShip player = new PlayerShip(pctShip.Location.X, pctShip.Location.Y);
            // O desenho ficará por nossa conta agora; escondemos o controle do designer
            pctShip.Visible = false;

            // Instancia o motor do jogo e o gerenciador de colisões
            myGame = new Game(player, this);
            myGame.OnAlienAtirou += CriarTiroAlien;
            gerenciadorColisoes = new CollisionManager();

            // Gera a matriz 3x5 de alienígenas obrigatória do escopo
            GenerateEnemies();

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

            // 2. Create the 3 Heart Images
            coracoesVidas = new List<PictureBox>();
            for (int i = 0; i < 3; i++)
            {
                PictureBox pctCoracao = new PictureBox();
                pctCoracao.Size = new Size(30, 30);
                pctCoracao.BackColor = Color.Transparent;
                pctCoracao.SizeMode = PictureBoxSizeMode.StretchImage;

                pctCoracao.Image = imagemCoracao;

                // Top right corner, spaced out
                int posX = this.ClientSize.Width - 40 - (i * 35);
                pctCoracao.Location = new Point(posX, 10);

                this.Controls.Add(pctCoracao);
                coracoesVidas.Add(pctCoracao);
            }
        }

        private void GenerateEnemies()
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
                    myGame.Aliens.Add(new Alien(x, y));
                }
            }
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            // 1. Movimenta o Player de forma suave
            if (indoEsquerda) myGame.Player.MoveLeft();
            if (indoDireita) myGame.Player.MoveRight(this.ClientSize.Width);

            // 2. Atualiza a posição de tiros e aliens
            myGame.Atualizar(gerenciadorColisoes);

            // 3. Checa quem bateu em quem (e passa o Form para remover imagens da tela)
            gerenciadorColisoes.ChecarColisoes(myGame);

            lblScore.Text = "Score: " + myGame.Score;

            // NEW: Update the Hearts based on the player's remaining lives
            // If they have 2 lives, only index 0 and 1 remain visible.
            for (int i = 0; i < coracoesVidas.Count; i++)
            {
                coracoesVidas[i].Visible = i < myGame.Player.Vidas;
            }

            foreach (var alienToRemove in gerenciadorColisoes.aliensRemover)
            {
               myGame.Aliens.Remove(alienToRemove);
            }
            gerenciadorColisoes.aliensRemover.Clear();

            foreach(var projToRemove in gerenciadorColisoes.projeteisRemover)
            {
               myGame.Projectiles.Remove(projToRemove);
            }
            gerenciadorColisoes.projeteisRemover.Clear();

                Invalidate();

            // 4. Valida Condições de Vitória e Derrota
            if (myGame.Player.Vidas <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show($"GAME OVER! Score final: {myGame.Score}", "Derrota");
                Application.Exit();
            }
            else if (myGame.Aliens.Count == 0)
            {
                gameTimer.Stop();
                MessageBox.Show($"VITÓRIA! Score final: {myGame.Score}", "Vitória");
                Application.Exit();
            }
        }

        // (Removido daqui e passado para baixo, para juntar se quiser)


        private void CriarTiroJogador()
        {
            var shipBounds = myGame.Player.Bounds;
            int xTiro = shipBounds.Left + (shipBounds.Width / 2) - (Projectile.Largura / 2);
            int yTiro = shipBounds.Top - Projectile.Altura;

            // Registra na lógica (true significa que é tiro do player)
            myGame.Projectiles.Add(new Projectile(xTiro, yTiro, true));
        }

        private void CriarTiroAlien(int xInicial, int yInicial)
        {
            // Limita a quantidade de tiros dos aliens na tela ao mesmo tempo também (opcional)
            int tirosAtivos = myGame.Projectiles.Count(p => !p.IsFromPlayer);
            if (tirosAtivos >= 4) return;

            // false significa que é tiro do inimigo
            myGame.Projectiles.Add(new Projectile(xInicial - (Projectile.Largura / 2), yInicial, false));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var alien in myGame?.Aliens ?? Enumerable.Empty<Alien>())
            {
                e.Graphics.DrawImage(imagemAlien, alien.Bounds);
            }

            if (myGame == null)
            {
                return;
            }

            // Desenha a nave do player
            e.Graphics.DrawImage(imagemNave, myGame.Player.Bounds);

            foreach (var projetil in myGame.Projectiles)
            {
                Brush brush = projetil.IsFromPlayer ? Brushes.Yellow : Brushes.LimeGreen;
                e.Graphics.FillRectangle(brush, projetil.Bounds);
            }
        }
    }
}
