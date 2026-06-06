using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using SpaceInvaders;
using Color = System.Drawing.Color;

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        // Declaração dos nossos objetos (Associação)
        private Game game;
        private AudioManager audioManager;
        private Timer gameTimer;

        private Panel menuPanel;
        private Button startButton;
        private Button exitButton;
        private Label titleLabel;

        private bool gameStarted = false;


        public GameForm()
        {
            InitializeComponent();

            // Make sure the form is double buffered to reduce flickering
            DoubleBuffered = true;
            KeyPreview = true;

            Resize += GameForm_Resize;

            // create new timer
            gameTimer = new Timer();
            gameTimer.Interval = 16; // Aproximadamente 60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            CreateMainMenu();

            soundIcon.Visible = false;
        }
        private void CreateMainMenu()
        {
            menuPanel = new Panel();
            menuPanel.Dock = DockStyle.Fill;
            menuPanel.BackColor = Color.Black;

            titleLabel = new Label();
            titleLabel.Text = "SPACE INVADERS";
            titleLabel.ForeColor = Color.LimeGreen;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Consolas", 28, FontStyle.Bold);
            titleLabel.AutoSize = true;

            startButton = new Button();
            startButton.Text = "START";
            startButton.Font = new Font("Consolas", 14, FontStyle.Bold);
            startButton.Width = 180;
            startButton.Height = 45;
            startButton.Click += StartButton_Click;

            exitButton = new Button();
            exitButton.Text = "EXIT";
            exitButton.Font = new Font("Consolas", 14, FontStyle.Bold);
            exitButton.Width = 180;
            exitButton.Height = 45;
            exitButton.Click += ExitButton_Click;

            menuPanel.Controls.Add(titleLabel);
            menuPanel.Controls.Add(startButton);
            menuPanel.Controls.Add(exitButton);

            Controls.Add(menuPanel);
            menuPanel.BringToFront();

            PositionMenuControls();
        }
        private void PositionMenuControls()
        {
            if (menuPanel == null)
            {
                return;
            }

            titleLabel.Left = ClientSize.Width / 2 - titleLabel.Width / 2;
            titleLabel.Top = ClientSize.Height / 2 - 130;

            startButton.Left = ClientSize.Width / 2 - startButton.Width / 2;
            startButton.Top = ClientSize.Height / 2 - 30;

            exitButton.Left = ClientSize.Width / 2 - exitButton.Width / 2;
            exitButton.Top = startButton.Bottom + 15;
        }
        private void AudioManager_PlayEffect(SoundEffectType effectType)
        {
            if (!gameStarted)
            {
                return;
            }
            audioManager.PlayEffect(effectType);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            if (game != null)
            {
                game.SetViewPort(ClientSize);
            }
            PositionMenuControls();
            Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted)
            {
                return;
            }
            game.Update(ClientSize);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!gameStarted)
            {
                return;
            }
            game.Draw(e.Graphics, ClientSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!gameStarted)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    StartButton_Click(this, EventArgs.Empty);
                }

                if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }

                return;
            }

            game.KeyDown(e.KeyCode);
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!gameStarted)
            {
                return;
            }

            game.KeyUp(e.KeyCode);
            e.Handled = true;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;

            return key == Keys.Left ||
                key == Keys.Right ||
                key == Keys.Space ||
                key == Keys.A ||
                key == Keys.D ||
                base.IsInputKey(keyData);
        }

        private void soundIcon_Click(object sender, EventArgs e)
        {
            bool muted = audioManager.ToggleMute();

            soundIcon.BackgroundImage = muted
                ? Properties.Resources.mute_icon
                : Properties.Resources.sound_icon;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            gameStarted = true;

            game = new Game();
            game.SetViewPort(ClientSize);

            audioManager = new AudioManager();
            audioManager.SetEffectsVolume(0.05f); // Ajusta o volume dos efeitos sonoros
            game.SoundEffectRequested += AudioManager_PlayEffect;

            audioManager.LoadEffects();
            audioManager.PlayMusic("keygen.wav");

            menuPanel.Visible = false;
            soundIcon.Visible = true;

            Focus();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
