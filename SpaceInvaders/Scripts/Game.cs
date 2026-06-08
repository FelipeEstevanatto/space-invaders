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
        private readonly List<Explosion> explosions;
        private readonly InputController input;

        public event Action<SoundEffectType> SoundEffectRequested;

        private bool isInitialized;
        private Size viewportSize;

        private int shootCooldown;
        private int alienShootCooldown;
        private int alienDirection;

        private int currentLevel = 1;
        public GameState CurrentState { get; private set; } = GameState.Menu;

        public static readonly Size VirtualSize = new Size(910, 501);

        // Exposed Public Properties for the Renderer
        public IReadOnlyList<Alien> Aliens => aliens;
        public IReadOnlyList<Projectile> Projectiles => projectiles;
        public IReadOnlyList<Explosion> Explosions => explosions;
        public PlayerShip Player => player;
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int CurrentLevel => currentLevel;
        public bool IsInitialized => isInitialized;
        public int BackgroundOffsetY => backgroundOffsetY;

        public Game(InputController inputController)
        {
            input = inputController;
            random = new Random();
            aliens = new List<Alien>();
            projectiles = new List<Projectile>();
            explosions = new List<Explosion>();

            alienDirection = 1;
            Lives = GameSettings.InitialLives;
            Score = 0;

            player = new PlayerShip(0, 0);
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

        public void Update(Size viewportSize)
        {
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0) return;

            if (CurrentState == GameState.Menu) return;

            backgroundOffsetY += 1;
            if (backgroundOffsetY >= viewportSize.Height) backgroundOffsetY = 0;

            EnsureInitialized(viewportSize);

            if (CurrentState == GameState.GameOver || CurrentState == GameState.Won)
            {
                if (input.IsRestarting)
                {
                    Reset();
                    input.ClearRestart();
                }
                RemoveInactiveObjects(viewportSize);
                return;
            }

            HandlePlayerInput(viewportSize);
            UpdateAliens(viewportSize);
            UpdateAlienShooting();
            UpdateProjectiles();

            CollisionManager.Resolve(player, aliens, projectiles, OnAlienDestroyed, OnPlayerHit);

            UpdateExplosions();
            RemoveInactiveObjects(viewportSize);
            CheckGameState(viewportSize);

            if (shootCooldown > 0) shootCooldown--;
            if (alienShootCooldown > 0) alienShootCooldown--;
        }

        private void UpdateExplosions()
        {
            foreach (Explosion explosion in explosions) explosion.Update();
        }

        private void EnsureInitialized(Size viewportSize)
        {
            if (isInitialized) return;

            int playerX = viewportSize.Width / 2 - PlayerShip.DefaultWidth / 2;
            int playerY = viewportSize.Height - PlayerShip.DefaultHeight - 30;

            player = new PlayerShip(playerX, playerY);
            CreateAliens(viewportSize);

            alienShootCooldown = GameSettings.InitialAlienShootCooldown;
            isInitialized = true;
        }

        private void HandlePlayerInput(Size viewportSize)
        {
            if (input.IsMovingLeft) player.MoveLeft();
            if (input.IsMovingRight) player.MoveRight();

            player.ClampToBounds(viewportSize);

            if (input.IsShooting) TryShoot();
        }

        private void TryShoot()
        {
            if (shootCooldown > 0) return;

            projectiles.Add(player.CreateProjectile());
            shootCooldown = GameSettings.PlayerShootCooldown;
            RequestSound(SoundEffectType.Shoot);
        }

        private void UpdateProjectiles()
        {
            foreach (Projectile projectile in projectiles) projectile.Update();
        }

        private void UpdateAliens(Size viewportSize)
        {
            bool shouldDrop = false;

            int currentAlienSpeed = Math.Min(
                GameSettings.AlienBaseSpeed + currentLevel, 
                GameSettings.AlienMaxSpeed
            );

            foreach (Alien alien in aliens)
            {
                if (!alien.IsActive) continue;

                int nextX = alien.Bounds.X + (currentAlienSpeed * alienDirection);

                if (nextX < 0 || nextX + alien.Bounds.Width > viewportSize.Width)
                {
                    shouldDrop = true;
                    break;
                }
            }

            if (shouldDrop)
            {
                alienDirection *= -1;
                foreach (Alien alien in aliens) alien.Move(0, GameSettings.AlienDropDistance);
            }
            else
            {
                foreach (Alien alien in aliens) alien.Move(currentAlienSpeed * alienDirection, 0);
            }
        }

        private void UpdateAlienShooting()
        {
            if (alienShootCooldown > 0) return;

            List<Alien> activeAliens = new List<Alien>();
            foreach (Alien alien in aliens)
            {
                if (alien.IsActive) activeAliens.Add(alien);
            }

            if (activeAliens.Count == 0) return;

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
                    CurrentState = GameState.GameOver;
                    return;
                }
            }

            if (aliens.Count == 0)
            {
                if (currentLevel > 1)
                {
                    CurrentState = GameState.Won;
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

            if (Lives <= 0) CurrentState = GameState.GameOver;
        }

        private void CreateAliens(Size viewportSize)
        {
            aliens.Clear();

            const int rows = GameSettings.AlienRows;
            const int columns = GameSettings.AlienColumns;
            const int spacingX = GameSettings.AlienSpacingX;
            const int spacingY = GameSettings.AlienSpacingY;

            int totalWidth = columns * Alien.DefaultWidth + (columns - 1) * spacingX;
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
        public void StartPlaying()
        {
            CurrentState = GameState.Playing;
        }
        public void ReturnToMenu()
        {
            Reset();
            CurrentState = GameState.Menu;
        }
        
        private void Reset()
        {
            isInitialized = false;
            shootCooldown = 0;
            alienShootCooldown = GameSettings.InitialAlienShootCooldown;
            alienDirection = 1;
            CurrentState = GameState.Playing;
            currentLevel = 1;
            Score = 0;
            Lives = GameSettings.InitialLives;

            aliens.Clear();
            projectiles.Clear();
            foreach (Explosion explosion in explosions) explosion.Dispose();
            explosions.Clear();
        }
    }
}