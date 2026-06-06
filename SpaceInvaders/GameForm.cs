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
        private Game game;

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
            gameTimer.Interval = 16; // ~60 FPS            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
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
    }
}
