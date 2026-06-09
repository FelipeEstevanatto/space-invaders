using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public class MenuManager
    {
        private readonly Form parentForm;
        private Panel menuPanel;
        private Button startButton;
        private Button exitButton;
        private Label titleLabel;
        private PictureBox menuShip;

        // Ship animation variables
        private int shipSpeedX = 4;
        private int shipSpeedY = 4;

        // Events that GameForm will listen to
        public event Action OnStartClicked;
        public event Action OnExitClicked;

        public bool IsVisible => menuPanel.Visible;

        public MenuManager(Form form)
        {
            parentForm = form;
            CreateMenu();
        }

        private void CreateMenu()
        {
            menuPanel = new DoubleBufferedPanel();
            menuPanel.Dock = DockStyle.Fill;
            
            menuPanel.BackgroundImage = Properties.Resources.space_background; // Ensure you have a background image in your resources
            menuPanel.BackgroundImageLayout = ImageLayout.Stretch;

            menuShip = new PictureBox();
            menuShip.Image = Properties.Resources.nave_png;
            menuShip.SizeMode = PictureBoxSizeMode.StretchImage;
            menuShip.Size = new Size(240, 120);
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
            startButton.Click += (s, e) => OnStartClicked?.Invoke();

            exitButton = new Button();
            exitButton.Text = "EXIT";
            exitButton.Font = new Font("Consolas", 14, FontStyle.Bold);
            exitButton.Width = 180;
            exitButton.Height = 45;
            exitButton.Click += (s, e) => OnExitClicked?.Invoke();

            menuPanel.Controls.Add(titleLabel);
            menuPanel.Controls.Add(startButton);
            menuPanel.Controls.Add(exitButton);
            menuPanel.Controls.Add(menuShip); // Add ship first so it's in the background

            menuShip.SendToBack();

            parentForm.Controls.Add(menuPanel);
            menuPanel.BringToFront();

            Resize();
        }

        public void Resize()
        {
            if (menuPanel == null) return;

            int centerX = parentForm.ClientSize.Width / 2;
            int centerY = parentForm.ClientSize.Height / 2;

            titleLabel.Left = centerX - titleLabel.Width / 2;
            titleLabel.Top = centerY - 130;

            startButton.Left = centerX - startButton.Width / 2;
            startButton.Top = centerY - 30;

            exitButton.Left = centerX - exitButton.Width / 2;
            exitButton.Top = startButton.Bottom + 15;
        }

        public void UpdateAnimation()
        {
            if (!IsVisible) return;

            menuShip.Left += shipSpeedX;
            menuShip.Top += shipSpeedY;

            if (menuShip.Left <= 0 || menuShip.Right >= menuPanel.Width)
                shipSpeedX = -shipSpeedX;

            if (menuShip.Top <= 0 || menuShip.Bottom >= menuPanel.Height)
                shipSpeedY = -shipSpeedY;
        }

        public void Show()
        {
            menuPanel.Visible = true;
            // Reset ship to a safe position when menu opens
            menuShip.Location = new Point(100, 100);
        }

        public void Hide()
        {
            menuPanel.Visible = false;
        }

        public class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                            ControlStyles.OptimizedDoubleBuffer | 
                            ControlStyles.UserPaint, true);
            }
        }
    }
}