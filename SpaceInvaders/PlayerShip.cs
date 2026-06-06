using System;
using System.Drawing;
using SpaceInvaders;

namespace SpaceInvaders
{
    public class PlayerShip
    {
        public const int DefaultWidth = 80;
        public const int DefaultHeight = 40;

        private const int Speed = 6;

        public Rectangle Bounds { get; private set; }

        public PlayerShip(int x, int y)
        {
            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
        }

        public void MoveLeft()
        {
            Move(-Speed);
        }

        public void MoveRight()
        {
            Move(Speed);
        }

        private void Move(int amount)
        {
            Bounds = new Rectangle(
                Bounds.X + amount,
                Bounds.Y,
                Bounds.Width,
                Bounds.Height);
        }

        public void ClampToBounds(Size viewportSize)
        {
            int maxX = Math.Max(0, viewportSize.Width - Bounds.Width);
            int maxY = Math.Max(0, viewportSize.Height - Bounds.Height);

            int x = Clamp(Bounds.X, 0, maxX);
            int y = Clamp(Bounds.Y, 0, maxY);

            Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public Projectile CreateProjectile()
        {
            int x = Bounds.X + Bounds.Width / 2 - Projectile.DefaultWidth / 2;
            int y = Bounds.Y - Projectile.DefaultHeight;

            return new Projectile(
                x,
                y,
                -8,
                ProjectileOwner.Player);
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(Properties.Resources.nave_png, Bounds);
        }
    }
}