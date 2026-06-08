using System.Drawing;

namespace SpaceInvaders
{
    public class GameRenderer
    {
        private readonly Image backgroundImage;
        private readonly Font hudFont = new Font("Consolas", 9, FontStyle.Bold);
        private readonly Brush hudBackgroundBrush = new SolidBrush(Color.FromArgb(150, 150, 150, 150));
        private readonly Image heartImage;

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
            string text = "GAME OVER - Press R to restart";
            using (Font font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold))
            {
                SizeF size = graphics.MeasureString(text, font);
                float x = viewportSize.Width / 2f - size.Width / 2f;
                float y = viewportSize.Height / 2f - size.Height / 2f;
                graphics.DrawString(text, font, Brushes.White, x, y);
            }
        }

        private void DrawWinScreen(Graphics graphics, Size viewportSize)
        {
            string title = "YOU WIN!";
            string subtitle = "Press R to restart";

            using (Font titleFont = new Font(FontFamily.GenericSansSerif, 28, FontStyle.Bold))
            using (Font subtitleFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold))
            {
                SizeF titleSize = graphics.MeasureString(title, titleFont);
                SizeF subtitleSize = graphics.MeasureString(subtitle, subtitleFont);

                float titleX = viewportSize.Width / 2f - titleSize.Width / 2f;
                float titleY = viewportSize.Height / 2f - titleSize.Height;
                float subtitleX = viewportSize.Width / 2f - subtitleSize.Width / 2f;
                float subtitleY = titleY + titleSize.Height + 10;

                graphics.DrawString(title, titleFont, Brushes.LimeGreen, titleX, titleY);
                graphics.DrawString(subtitle, subtitleFont, Brushes.White, subtitleX, subtitleY);
            }
        }
    }
}