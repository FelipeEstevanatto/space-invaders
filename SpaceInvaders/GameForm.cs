using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        private Game game;
        private GameRenderer renderer;
        private InputController inputController;
        private AudioManager audioManager;
        private MenuManager menuManager; // NEW: Replaces all the UI variables
        private Timer gameTimer;

        public GameForm()
        {
            InitializeComponent();

            this.AutoSize = false;
            this.MinimumSize = new Size(910, 501);

            DoubleBuffered = true;
            KeyPreview = true;
            Resize += GameForm_Resize;

            // 1. Initialize Core Systems
            inputController = new InputController();
            game = new Game(inputController);
            game.SetViewPort(Game.VirtualSize);
            renderer = new GameRenderer();

            // 2. Initialize Audio
            audioManager = new AudioManager();
            audioManager.SetEffectsVolume(0.08f);
            audioManager.SetMusicVolume(0.2f);
            audioManager.LoadEffects();
            game.SoundEffectRequested += AudioManager_PlayEffect;

            // 3. Initialize UI (Clean!)
            menuManager = new MenuManager(this);
            menuManager.OnStartClicked += StartGame;
            menuManager.OnExitClicked += Close;
            soundIcon.Visible = false;

            // 4. Start Game Loop
            gameTimer = new Timer();
            gameTimer.Interval = GameSettings.TimerIntervalMs;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void StartGame()
        {
            game.StartPlaying();
            audioManager.PlayMusic("keygen.wav");

            menuManager.Hide();
            soundIcon.Visible = true;

            Focus();
        }

        private void ReturnToMenu()
        {
            game.ReturnToMenu();
            audioManager.StopMusic();

            menuManager.Show();
            soundIcon.Visible = false;
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            if (game != null) game.SetViewPort(Game.VirtualSize);
            if (menuManager != null) menuManager.Resize();
            Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (game.CurrentState == GameState.Menu)
            {
                menuManager.UpdateAnimation();
            }
            else
            {
                game.Update(Game.VirtualSize);
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (game == null || renderer == null || game.CurrentState == GameState.Menu) return;
            renderer.Render(e.Graphics, game, ClientSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (game.CurrentState == GameState.Menu)
            {
                if (e.KeyCode == Keys.Enter) StartGame();
                if (e.KeyCode == Keys.Escape) Close();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                ReturnToMenu();
                return;
            }

            inputController.KeyDown(e.KeyCode);
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (game.CurrentState == GameState.Menu) return;

            inputController.KeyUp(e.KeyCode);
            e.Handled = true;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            return key == Keys.Left || key == Keys.Right || key == Keys.Space ||
                   key == Keys.A || key == Keys.D || key == Keys.R ||
                   base.IsInputKey(keyData);
        }

        private void soundIcon_Click(object sender, EventArgs e)
        {
            bool muted = audioManager.ToggleMute();
            soundIcon.BackgroundImage = muted
                ? Properties.Resources.mute_icon
                : Properties.Resources.sound_icon;
        }

        private void AudioManager_PlayEffect(SoundEffectType effectType)
        {
            if (game.CurrentState != GameState.Playing) return;
            audioManager.PlayEffect(effectType);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }

            if (audioManager != null) audioManager.Dispose();
            base.OnFormClosed(e);
        }
    }
}