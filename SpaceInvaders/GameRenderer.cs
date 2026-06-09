using System.Drawing;

namespace SpaceInvaders
{
    public class GameRenderer
    {
        private readonly Image heartImage;
        private readonly Image backgroundImage;
        private readonly Font hudFont = new Font("Consolas", 9, FontStyle.Bold);
        private readonly Brush hudBackgroundBrush = new SolidBrush(Color.FromArgb(150, 150, 150, 150));
        private readonly Brush overlayBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
        private readonly Font titleFont = new Font("Consolas", 36, FontStyle.Bold);
        private readonly Font subtitleFont = new Font("Consolas", 14, FontStyle.Bold);
        private readonly Brush shieldBrush = new SolidBrush(Color.LimeGreen);

        public GameRenderer()
        {
            backgroundImage = Properties.Resources.space_background;
            heartImage = Properties.Resources.red_heart;
        }

        public void Render(Graphics graphics, Game game, Size actualViewportSize)
        {
            float scaleX = (float)actualViewportSize.Width / Game.VirtualSize.Width;
            float scaleY = (float)actualViewportSize.Height / Game.VirtualSize.Height;

            graphics.ScaleTransform(scaleX, scaleY);

            DrawBackground(graphics, game.BackgroundOffsetY, Game.VirtualSize);

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            if (!game.IsInitialized)
            {
                graphics.ResetTransform();
                return;
            }

            game.Player.Draw(graphics);

            foreach (Alien alien in game.Aliens)
            {
                alien.Draw(graphics);
            }

            foreach (Explosion explosion in game.Explosions)
            {
                explosion.Draw(graphics);
            }

            foreach (Projectile projectile in game.Projectiles)
            {
                projectile.Draw(graphics);
            }

            foreach (Shield shield in game.Shields)
            {
                // Draw the green block
                graphics.FillRectangle(shieldBrush, shield.Bounds);

                // Draw the health number centered inside the block in black
                string hpText = shield.Health.ToString();
                SizeF textSize = graphics.MeasureString(hpText, hudFont);
                
                float textX = shield.Bounds.X + (shield.Bounds.Width / 2f) - (textSize.Width / 2f);
                float textY = shield.Bounds.Y + (shield.Bounds.Height / 2f) - (textSize.Height / 2f);
                
                graphics.DrawString(hpText, hudFont, Brushes.Black, textX, textY);
            }

            DrawHud(graphics, game);

            if (game.CurrentState == GameState.GameOver)
            {
                DrawGameOver(graphics, Game.VirtualSize);
            }

            if (game.CurrentState == GameState.Won)
            {
                DrawWinScreen(graphics, Game.VirtualSize);
            }

            graphics.ResetTransform();
        }

        private void DrawBackground(Graphics graphics, int offsetY, Size viewportSize)
        {
            int overlapHeight = viewportSize.Height + 2;

            Rectangle first = new Rectangle(0, offsetY, viewportSize.Width, overlapHeight);
            Rectangle second = new Rectangle(0, offsetY - viewportSize.Height, viewportSize.Width, overlapHeight);

            graphics.DrawImage(backgroundImage, first);
            graphics.DrawImage(backgroundImage, second);
        }

        private void DrawHud(Graphics graphics, Game game)
        {
            // Draw the translucent black background bar
            Rectangle hudArea = new Rectangle(0, 0, 220, 28); // Made slightly wider to fit icons
            graphics.FillRectangle(hudBackgroundBrush, hudArea);

            string text = $"Score: {game.Score}    Level: {game.CurrentLevel}";
            string text2 = $"Space: Shoot    A/D or Arrows: Move";
            graphics.DrawString(text, hudFont, Brushes.White, 6, 7);
            graphics.DrawString(text2, hudFont, Brushes.White, 6, 480);

            int heartWidth = 16;
            int heartHeight = 16;
            int spacing = 4;
            int startX = 490;
            int startY = 6;

            graphics.DrawString("Lives:", hudFont, Brushes.White, startX - 60, 7);

            for (int i = 0; i < game.Lives; i++)
            {
                // Calculate the X position for this specific heart
                int currentX = startX + (i * (heartWidth + spacing));
                
                graphics.DrawImage(heartImage, currentX, startY, heartWidth, heartHeight);
            }
        }

        private void DrawGameOver(Graphics graphics, Size viewportSize)
        {
            // Semi-transparent dark overlay to dim the game behind the text
            graphics.FillRectangle(overlayBrush, 0, 0, viewportSize.Width, viewportSize.Height);

            string title = "GAME OVER";
            string subtitle = "Press R to restart";
            string subtitle2 = "Press ESC for Menu";

            using (Font titleFont = new Font("Consolas", 36, FontStyle.Bold))
            using (Font subtitleFont = new Font("Consolas", 14, FontStyle.Bold))
            {
                SizeF titleSize = graphics.MeasureString(title, titleFont);
                SizeF subSize = graphics.MeasureString(subtitle, subtitleFont);
                SizeF sub2Size = graphics.MeasureString(subtitle2, subtitleFont);

                float titleX = viewportSize.Width / 2f - titleSize.Width / 2f;
                float titleY = viewportSize.Height / 2f - titleSize.Height;

                graphics.DrawString(title, titleFont, Brushes.Red, titleX, titleY);
                
                graphics.DrawString(subtitle, subtitleFont, Brushes.White, viewportSize.Width / 2f - subSize.Width / 2f, titleY + titleSize.Height + 10);
                graphics.DrawString(subtitle2, subtitleFont, Brushes.White, viewportSize.Width / 2f - sub2Size.Width / 2f, titleY + titleSize.Height + 40);
            }
        }

        private void DrawWinScreen(Graphics graphics, Size viewportSize)
        {
            // Semi-transparent overlay for the win screen
            graphics.FillRectangle(overlayBrush, 0, 0, viewportSize.Width, viewportSize.Height);

            string title = "YOU WIN!";
            string subtitle = "Press R to restart";
            string subtitle2 = "Press ESC for Menu";

            using (Font titleFont = new Font("Consolas", 36, FontStyle.Bold))
            using (Font subtitleFont = new Font("Consolas", 14, FontStyle.Bold))
            {
                SizeF titleSize = graphics.MeasureString(title, titleFont);
                SizeF subSize = graphics.MeasureString(subtitle, subtitleFont);
                SizeF sub2Size = graphics.MeasureString(subtitle2, subtitleFont);

                float titleX = viewportSize.Width / 2f - titleSize.Width / 2f;
                float titleY = viewportSize.Height / 2f - titleSize.Height;

                graphics.DrawString(title, titleFont, Brushes.LimeGreen, titleX, titleY);
                
                graphics.DrawString(subtitle, subtitleFont, Brushes.White, viewportSize.Width / 2f - subSize.Width / 2f, titleY + titleSize.Height + 10);
                graphics.DrawString(subtitle2, subtitleFont, Brushes.White, viewportSize.Width / 2f - sub2Size.Width / 2f, titleY + titleSize.Height + 40);
            }
        }
    }
}