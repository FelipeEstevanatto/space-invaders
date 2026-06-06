using System.Drawing;
using SpaceInvaders;

namespace SpaceInvaders
{
    public class PlayerShip
    {
        public const int DefaultWidth = 50;
        public const int DefaultHeight = 24;

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
            int maxX = viewportSize.Width - Bounds.Width;
            int maxY = viewportSize.Height - Bounds.Height;

            if (maxX < 0)
            {
                maxX = 0;
            }

            if (maxY < 0)
            {
                maxY = 0;
            }

            int x = Bounds.X;
            int y = Bounds.Y;

            if (x < 0)
            {
                x = 0;
            }

            if (x > maxX)
            {
                x = maxX;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (y > maxY)
            {
                y = maxY;
            }

            Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
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
            graphics.FillRectangle(Brushes.DeepSkyBlue, Bounds);

            Point[] nose =
            {
                new Point(Bounds.X + Bounds.Width / 2, Bounds.Y - 10),
                new Point(Bounds.X + 8, Bounds.Y + 6),
                new Point(Bounds.Right - 8, Bounds.Y + 6)
            };

            graphics.FillPolygon(Brushes.DeepSkyBlue, nose);
        }
    }
}