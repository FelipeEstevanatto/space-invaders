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

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        // Declaração dos nossos objetos (Associação)
        private Game game;
        private AudioManager audioManager;
        private bool _isPaused = false;


        public GameForm()
        {
            InitializeComponent();

            // Make sure the form is double buffered to reduce flickering
            DoubleBuffered = true;
            KeyPreview = true;

            game = new Game();
            game.SetViewPort(ClientSize);

            audioManager = new AudioManager();
            audioManager.PlayMusic("keygen.wav");
            audioManager.SetEffectsVolume(0.05f); // Ajusta o volume dos efeitos sonoros

            game.SoundEffectRequested += AudioManager_PlayEffect;

            Resize += GameForm_Resize;

            // create new timer
            Timer gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

        }

        private void AudioManager_PlayEffect(SoundEffectType effectType)
        {
            audioManager.PlayEffect(effectType);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            game.SetViewPort(ClientSize);
            Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            game.Update(ClientSize);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            game.Draw(e.Graphics, ClientSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            game.KeyDown(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            game.KeyUp(e.KeyCode);
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
            if (_isPaused)
            {
                audioManager.PlayMusic("keygen.wav");
                _isPaused = false;
                soundIcon.BackgroundImage = Properties.Resources.sound_icon;
            }
            else
            {
                audioManager.ToggleMute();
                _isPaused = true;
                soundIcon.BackgroundImage = Properties.Resources.mute_icon;
            }
        }
    }
}
