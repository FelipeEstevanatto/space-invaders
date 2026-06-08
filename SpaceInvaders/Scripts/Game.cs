using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public class Game
    {
        private int backgroundOffsetY;
        private readonly Random random;
        private PlayerShip player;
        private readonly List<Alien> aliens;
        private readonly List<Projectile> projectiles;
        public event Action<SoundEffectType> SoundEffectRequested;
        private readonly List<Explosion> explosions;

        private bool isInitialized;
        private bool isMovingLeft;
        private bool isMovingRight;
        private bool isShooting;
        private Size viewportSize;

        private int shootCooldown;
        private int alienShootCooldown;
        private int alienDirection;

        private bool gameOver;
        private bool gameWon;
        public static readonly Size VirtualSize = new Size(910, 501);

        private const int PlayerShootCooldownMax = GameSettings.PlayerShootCooldown;
        private const int AlienMoveSpeed = GameSettings.AlienBaseSpeed;
        private const int AlienDropDistance = GameSettings.AlienDropDistance;

        public int Score { get; private set; }
        public int Lives { get; private set; }
        private int currentLevel = 1;
        private readonly Image backgroundImage;
        private readonly Font hudFont = new Font("Consolas", 9, FontStyle.Bold);
        private readonly Brush hudBackgroundBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0));

        public Game()
        {
            random = new Random();
            aliens = new List<Alien>();
            projectiles = new List<Projectile>();
            explosions = new List<Explosion>();

            alienDirection = 1;
            Lives = GameSettings.InitialLives;
            Score = 0;

            player = new PlayerShip(0, 0);
            backgroundImage = Properties.Resources.space_background;
        }

        private void RequestSound(SoundEffectType effectType)
        {
            if (SoundEffectRequested != null)
            {
                SoundEffectRequested(effectType);
            }
        }

        public void SetViewPort(Size size)
        {
            viewportSize = size;

            player.ClampToBounds(viewportSize);
        }

        public void KeyDown(Keys key)
        {
            if (key == Keys.Left || key == Keys.A)
            {
                isMovingLeft = true;
            }

            if (key == Keys.Right || key == Keys.D)
            {
                isMovingRight = true;
            }

            if (key == Keys.Space)
            {
                isShooting = true;
            }

            if (key == Keys.R && (gameOver || gameWon))
            {
                Reset();
            }
        }

        public void KeyUp(Keys key)
        {
            if (key == Keys.Left || key == Keys.A)
            {
                isMovingLeft = false;
            }

            if (key == Keys.Right || key == Keys.D)
            {
                isMovingRight = false;
            }

            if (key == Keys.Space)
            {
                isShooting = false;
            }
        }

        public void Update(Size viewportSize)
        {
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
            {
                return;
            }

            backgroundOffsetY += 1;

            if (backgroundOffsetY >= viewportSize.Height)
            {
                backgroundOffsetY = 0;
            }

            EnsureInitialized(viewportSize);

            if (gameOver || gameWon)
            {
                //UpdateExplosions();
                RemoveInactiveObjects(viewportSize);
                return;
            }

            HandlePlayerInput(viewportSize);
            UpdateAliens(viewportSize);
            UpdateAlienShooting();
            UpdateProjectiles();

            CollisionManager.Resolve(
                player,
                aliens,
                projectiles,
                OnAlienDestroyed,
                OnPlayerHit);

            UpdateExplosions();
            RemoveInactiveObjects(viewportSize);
            CheckGameState(viewportSize);

            if (shootCooldown > 0)
            {
                shootCooldown--;
            }

            if (alienShootCooldown > 0)
            {
                alienShootCooldown--;
            }
        }

        private void UpdateExplosions()
        {
            foreach (Explosion explosion in explosions)
            {
                explosion.Update();
            }
        }

        // Moving effect
        private void DrawBackground(Graphics graphics, Size viewportSize)
        {
            // Add +2 pixels to the height. Because the top image is drawn second, 
            // it will perfectly paint over any 1-pixel rounding gaps (in larger screens).
            int overlapHeight = viewportSize.Height + 2;

            Rectangle first = new Rectangle(
                0,
                backgroundOffsetY,
                viewportSize.Width,
                overlapHeight);

            Rectangle second = new Rectangle(
                0,
                backgroundOffsetY - viewportSize.Height,
                viewportSize.Width,
                overlapHeight);

            graphics.DrawImage(backgroundImage, first);
            graphics.DrawImage(backgroundImage, second);
        }

        public void Draw(Graphics graphics, Size actualViewportSize)
        {
            // 1. Calculate how much the actual window is stretched compared to the virtual window
            float scaleX = (float)actualViewportSize.Width / VirtualSize.Width;
            float scaleY = (float)actualViewportSize.Height / VirtualSize.Height;

            // 2. MAGICAL GDI+ METHOD: This stretches everything drawn after this line!
            graphics.ScaleTransform(scaleX, scaleY);

            // 3. Draw everything as normal, but pass the VirtualSize instead!
            DrawBackground(graphics, VirtualSize);
            
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            if (!isInitialized)
            {
                graphics.ResetTransform(); // Clean up before returning
                return;
            }

            player.Draw(graphics);

            foreach (Alien alien in aliens)
            {
                alien.Draw(graphics);
            }

            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(graphics);
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(graphics);
            }

            DrawHud(graphics);

            if (gameOver)
            {
                DrawGameOver(graphics, VirtualSize);
            }

            if (gameWon)
            {
                DrawWinScreen(graphics, VirtualSize);
            }
            
            // 4. Reset the transform so we don't mess up WinForms native UI drawing
            graphics.ResetTransform();
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

        private void EnsureInitialized(Size viewportSize)
        {
            if (isInitialized)
            {
                return;
            }

            int playerX = viewportSize.Width / 2 - PlayerShip.DefaultWidth / 2;
            int playerY = viewportSize.Height - PlayerShip.DefaultHeight - 30;

            player = new PlayerShip(playerX, playerY);
            CreateAliens(viewportSize);

            alienShootCooldown = GameSettings.InitialAlienShootCooldown;
            isInitialized = true;
        }

        private void HandlePlayerInput(Size viewportSize)
        {
            if (isMovingLeft)
            {
                player.MoveLeft();
            }

            if (isMovingRight)
            {
                player.MoveRight();
            }

            player.ClampToBounds(viewportSize);

            if (isShooting)
            {
                TryShoot();
            }
        }

        private void TryShoot()
        {
            if (shootCooldown > 0)
            {
                return;
            }

            projectiles.Add(player.CreateProjectile());
            shootCooldown = PlayerShootCooldownMax;

            RequestSound(SoundEffectType.Shoot);
        }

        private void UpdateProjectiles()
        {
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update();
            }
        }

        private void UpdateAliens(Size viewportSize)
        {
            bool shouldDrop = false;

            foreach (Alien alien in aliens)
            {
                if (!alien.IsActive)
                {
                    continue;
                }

                // int nextX = alien.Bounds.X + AlienMoveSpeed * alienDirection;
                int nextX = alien.Bounds.X + Math.Min(AlienMoveSpeed + (currentLevel - 1) * 8, 12) * alienDirection;

                if (nextX < 0 || nextX + alien.Bounds.Width > viewportSize.Width)
                {
                    shouldDrop = true;
                    break;
                }
            }

            if (shouldDrop)
            {
                alienDirection *= -1;

                foreach (Alien alien in aliens)
                {
                    alien.Move(0, AlienDropDistance);
                }
            }
            else
            {
                foreach (Alien alien in aliens)
                {
                    alien.Move(AlienMoveSpeed * alienDirection, 0);
                }
            }
        }

        private void UpdateAlienShooting()
        {
            if (alienShootCooldown > 0)
            {
                return;
            }

            List<Alien> activeAliens = new List<Alien>();

            foreach (Alien alien in aliens)
            {
                if (alien.IsActive)
                {
                    activeAliens.Add(alien);
                }
            }

            if (activeAliens.Count == 0)
            {
                return;
            }

            Alien shooter = activeAliens[random.Next(activeAliens.Count)];
            projectiles.Add(shooter.CreateProjectile());

            alienShootCooldown = random.Next(GameSettings.AlienShootCooldownMin, GameSettings.AlienShootCooldownMax);
        }

        private void RemoveInactiveObjects(Size viewportSize)
        {
            projectiles.RemoveAll(projectile =>
                !projectile.IsActive ||
                projectile.Bounds.Bottom < 0 ||
                projectile.Bounds.Top > viewportSize.Height);

            aliens.RemoveAll(alien => !alien.IsActive);

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (!explosions[i].IsActive)
                {
                    explosions[i].Dispose();
                    explosions.RemoveAt(i);
                }
            }
        }

        private void CheckGameState(Size viewportSize)
        {
            foreach (Alien alien in aliens)
            {
                if (alien.Bounds.Bottom >= player.Bounds.Top)
                {
                    gameOver = true;
                    return;
                }
            }

            if (aliens.Count == 0)
            {
                if (currentLevel > 1)
                {
                    gameWon = true;
                    return;
                }

                currentLevel++;
                CreateAliens(viewportSize);
                alienDirection = 1;
            }
        }

        private void OnAlienDestroyed(Alien alien)
        {
            Score += alien.PointValue;

            explosions.Add(new Explosion(alien.Bounds));

            RequestSound(SoundEffectType.AlienDestroyed);
        }

        private void OnPlayerHit()
        {
            Lives--;

            RequestSound(SoundEffectType.PlayerHit);
            explosions.Add(new Explosion(player.Bounds));

            if (Lives <= 0)
            {
                gameOver = true;
            }
        }

        private void DrawHud(Graphics graphics)
        {
            Rectangle hudArea = new Rectangle(0, 0, 520, 28);
            graphics.FillRectangle(hudBackgroundBrush, hudArea);

            string text = $"Score: {Score}    Lives: {Lives}    Level: {currentLevel}    Space: Shoot    A/D or Arrows: Move    R: Restart";
            graphics.DrawString(text, hudFont, Brushes.White, 6, 7);
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

        private void CreateAliens(Size viewportSize)
        {
            aliens.Clear();

            const int rows = GameSettings.AlienRows;
            const int columns = GameSettings.AlienColumns;
            const int spacingX = GameSettings.AlienSpacingX;
            const int spacingY = GameSettings.AlienSpacingY;

            int totalWidth =
                columns * Alien.DefaultWidth +
                (columns - 1) * spacingX;

            int startX = Math.Max(20, viewportSize.Width / 2 - totalWidth / 2);
            int startY = 50;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    int x = startX + column * (Alien.DefaultWidth + spacingX);
                    int y = startY + row * (Alien.DefaultHeight + spacingY);

                    aliens.Add(new Alien(x, y, row));
                }
            }
        }

        private void Reset()
        {
            isInitialized = false;
            isMovingLeft = false;
            isMovingRight = false;
            isShooting = false;
            shootCooldown = 0;
            alienShootCooldown = GameSettings.InitialAlienShootCooldown;
            alienDirection = 1;
            gameOver = false;
            gameWon = false;
            currentLevel = 1;
            Score = 0;
            Lives = GameSettings.InitialLives;

            aliens.Clear();
            projectiles.Clear();
            foreach (Explosion explosion in explosions)
            {
                explosion.Dispose();
            }

            explosions.Clear();
        }
    }
}