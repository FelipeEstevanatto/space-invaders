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
        private CollisionManager collisionManager;
        private Timer gameTimer;

        // Controles de movimentação contínua
        private bool goingLeft;
        private bool goingRight;

        private Label lblScore;
        private List<PictureBox> heartLives;
        private readonly Image imagemAlien = Properties.Resources.alien_png;
        private readonly Image imagemCoracao = Properties.Resources.red_heart;
        private readonly Image shipImage = Properties.Resources.nave_png;

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
                goingLeft = true;

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                goingRight = true;

            // Disparo do player (Garante que só atira uma vez por aperto de tecla)
            if (e.KeyCode == Keys.Space && !espacoPressionado)
            {
                espacoPressionado = true;
                // Limita a 3 tiros simultâneos na screen para evitar "metralhadora"
                int tirosAtivos = myGame.Projectiles.Count(p => p.IsFromPlayer);
                if (tirosAtivos < 3)
                {
                    CreatePlayerProjectile();
                }
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                goingLeft = false;

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                goingRight = false;

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
            myGame.OnAlienShot += CreateAlienProjectile;
            collisionManager = new CollisionManager();

            // Gera a matriz 3x5 de alienígenas obrigatória do escopo
            GenerateEnemies();

            // NEW: Call the method to draw the UI
            CreateUserInterface();

            // Configura o Game Loop (O coração do jogo)
            gameTimer = new Timer();
            gameTimer.Interval = 16; // Roda a ~60 FPS (Quadros por segundo)
            gameTimer.Tick += timerClock_Tick;
            gameTimer.Start();
        }

        private void CreateUserInterface()
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
            heartLives = new List<PictureBox>();
            for (int i = 0; i < 3; i++)
            {
                PictureBox pctHeart = new PictureBox();
                pctHeart.Size = new Size(30, 30);
                pctHeart.BackColor = Color.Transparent;
                pctHeart.SizeMode = PictureBoxSizeMode.StretchImage;

                pctHeart.Image = imagemCoracao;

                // Top right corner, spaced out
                int posX = this.ClientSize.Width - 40 - (i * 35);
                pctHeart.Location = new Point(posX, 10);

                this.Controls.Add(pctHeart);
                heartLives.Add(pctHeart);
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
            if (goingLeft) myGame.Player.MoveLeft();
            if (goingRight) myGame.Player.MoveRight(this.ClientSize.Width);

            // 2. Atualiza a posição de tiros e aliens
            myGame.Update(collisionManager);

            // 3. Checa quem bateu em quem (e passa o Form para remover imagens da screen)
            collisionManager.ChecarColisoes(myGame);

            lblScore.Text = "Score: " + myGame.Score;

            // NEW: Update the Hearts based on the player's remaining lives
            // If they have 2 lives, only index 0 and 1 remain visible.
            for (int i = 0; i < heartLives.Count; i++)
            {
                heartLives[i].Visible = i < myGame.Player.Lives;
            }

            foreach (var alienToRemove in collisionManager.removeAliens)
            {
               myGame.Aliens.Remove(alienToRemove);
            }
            collisionManager.removeAliens.Clear();

            foreach(var projToRemove in collisionManager.removeProjectiles)
            {
               myGame.Projectiles.Remove(projToRemove);
            }
            collisionManager.removeProjectiles.Clear();

            Invalidate();

            // 4. Valida Condições de Vitória e Derrota
            if (myGame.Player.Lives <= 0)
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

        private void CreatePlayerProjectile()
        {
            var shipBounds = myGame.Player.Bounds;
            int xTiro = shipBounds.Left + (shipBounds.Width / 2) - (Projectile.Width / 2);
            int yTiro = shipBounds.Top - Projectile.Height;

            // Registra na lógica (true significa que é tiro do player)
            myGame.Projectiles.Add(new Projectile(xTiro, yTiro, true));
        }

        private void CreateAlienProjectile(int xInicial, int yInicial)
        {
            // Limita a quantidade de tiros dos aliens na screen ao mesmo tempo também (opcional)
            int tirosAtivos = myGame.Projectiles.Count(p => !p.IsFromPlayer);
            if (tirosAtivos >= 4) return;

            // false significa que é tiro do inimigo
            myGame.Projectiles.Add(new Projectile(xInicial - (Projectile.Width / 2), yInicial, false));
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
            e.Graphics.DrawImage(shipImage, myGame.Player.Bounds);

            foreach (var projectile in myGame.Projectiles)
            {
                Brush brush = projectile.IsFromPlayer ? Brushes.Yellow : Brushes.LimeGreen;
                e.Graphics.FillRectangle(brush, projectile.Bounds);
            }
        }
    }
}
