using System;
using System.Drawing;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        private Game game;
        private GameRenderer renderer;
        private InputController inputController;
        private AudioManager audioManager;
        private Timer gameTimer;

        private Panel menuPanel;
        private Button startButton;
        private Button exitButton;
        private Label titleLabel;
        private PictureBox menuShip;

        public GameForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
            KeyPreview = true;
            Resize += GameForm_Resize;

            inputController = new InputController();
            game = new Game(inputController);
            game.SetViewPort(ClientSize);
            renderer = new GameRenderer();

            gameTimer = new Timer();
            gameTimer.Interval = GameSettings.TimerIntervalMs;
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

            menuShip = new PictureBox();
            menuShip.Image = Properties.Resources.nave_png; 
            menuShip.SizeMode = PictureBoxSizeMode.StretchImage;
            menuShip.Size = new Size(400, 300);
            menuShip.BackColor = Color.Transparent;

            titleLabel = new Label();
            titleLabel.Text = "SPACE INVADERS";
            titleLabel.ForeColor = Color.LimeGreen;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Consolas", 38, FontStyle.Bold);
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
            menuPanel.Controls.Add(menuShip);

            Controls.Add(menuPanel);
            menuPanel.BringToFront();

            PositionMenuControls();
        }

        private void PositionMenuControls()
        {
            if (menuPanel == null) return;

            menuShip.Left = ClientSize.Width / 2 - titleLabel.Width / 2 + 10;
            menuShip.Top = ClientSize.Height / 2 - 130;

            titleLabel.Left = ClientSize.Width / 2 - titleLabel.Width / 2;
            titleLabel.Top = ClientSize.Height / 2 - 130;

            startButton.Left = ClientSize.Width / 2 - startButton.Width / 2;
            startButton.Top = ClientSize.Height / 2 - 30;

            exitButton.Left = ClientSize.Width / 2 - exitButton.Width / 2;
            exitButton.Top = startButton.Bottom + 15;
        }

        private void AudioManager_PlayEffect(SoundEffectType effectType)
        {
            if (game.CurrentState != GameState.Playing) return;
            audioManager.PlayEffect(effectType);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            if (game != null)
            {
                game.SetViewPort(Game.VirtualSize); 
            }
            PositionMenuControls();
            Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (game.CurrentState != GameState.Playing) return;
            game.Update(Game.VirtualSize);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (game.CurrentState != GameState.Playing || game == null || renderer == null) return;
            renderer.Render(e.Graphics, game, ClientSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (game.CurrentState != GameState.Playing)
            {
                if (e.KeyCode == Keys.Enter) StartButton_Click(this, EventArgs.Empty);
                if (e.KeyCode == Keys.Escape) Close();
                return;
            }

            inputController.KeyDown(e.KeyCode);
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (game.CurrentState != GameState.Playing) return;

            inputController.KeyUp(e.KeyCode);
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
            game.StartPlaying();

            audioManager = new AudioManager();
            audioManager.SetEffectsVolume(0.05f); 
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }

            if (audioManager != null)
            {
                audioManager.Dispose();
            }

            base.OnFormClosed(e);
        }
    }
}