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

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        // Declaração dos nossos objetos (Associação)
        private Game game;
        private MediaPlayer _mediaPlayer;
        private bool _isPaused = false;


        public GameForm()
        {
            InitializeComponent();

            // Make sure the form is double buffered to reduce flickering
            DoubleBuffered = true;
            KeyPreview = true;

            game = new Game();
            game.SetViewPort(ClientSize);

            Resize += GameForm_Resize;

            // create new timer
            Timer gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // 2. Garanta que o objeto seja criado (evita o NullReferenceException)
            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer();
            }

            // 3. Aponte para a pasta Resources combinando com o caminho absoluto
            string caminhoRelativo = System.IO.Path.Combine("Resources", "keygen.wav");
            string caminhoAbsoluto = System.IO.Path.GetFullPath(caminhoRelativo);

            // 4. Configure e toque o arquivo
            _mediaPlayer.Open(new Uri(caminhoAbsoluto));
            _mediaPlayer.Volume = 0.2;
            // Garante que o evento está associado (remova antes para não duplicar se chamar o método várias vezes)
            _mediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            _mediaPlayer.Play();

            _isPaused = false;
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // Reset the track position to the beginning and play again
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            return;
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
                _mediaPlayer.Play();
                _isPaused = false;
                soundIcon.BackgroundImage = Properties.Resources.sound_icon;
            }
            else
            {
                _mediaPlayer.Pause();
                _isPaused = true;
                soundIcon.BackgroundImage = Properties.Resources.mute_icon;
            }
        }
    }
}
