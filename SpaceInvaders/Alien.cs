using System.Drawing;
using SpaceInvaders;

namespace SpaceInvaders
{
    public class Alien
    {
        public const int DefaultWidth = 36;
        public const int DefaultHeight = 24;

        public Rectangle Bounds { get; private set; }
        public bool IsActive { get; set; }

        public Alien(int x, int y)
        {
            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
            IsActive = true;
        }

        public void Move(int dx, int dy)
        {
            Bounds = new Rectangle(
                Bounds.X + dx,
                Bounds.Y + dy,
                Bounds.Width,
                Bounds.Height);
        }

        public Projectile CreateProjectile()
        {
            int x = Bounds.X + Bounds.Width / 2 - Projectile.DefaultWidth / 2;
            int y = Bounds.Bottom;

            return new Projectile(
                x,
                y,
                5,
                ProjectileOwner.Alien);
        }

        public void Draw(Graphics graphics)
        {
            if (!IsActive)
            {
                return;
            }

            graphics.FillRectangle(Brushes.LimeGreen, Bounds);

            int eyeSize = 4;

            graphics.FillRectangle(
                Brushes.Black,
                Bounds.X + 8,
                Bounds.Y + 7,
                eyeSize,
                eyeSize);

            graphics.FillRectangle(
                Brushes.Black,
                Bounds.Right - 12,
                Bounds.Y + 7,
                eyeSize,
                eyeSize);
        }
    }
}