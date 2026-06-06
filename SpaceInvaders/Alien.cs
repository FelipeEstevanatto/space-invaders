using System.Drawing;
using SpaceInvaders;

namespace SpaceInvaders
{
    public class Alien
    {
        public const int DefaultWidth = 36;
        public const int DefaultHeight = 24;
        private static readonly Image AlienImage = CreateScaledImage(
            Properties.Resources.alien_png,
            DefaultWidth,
            DefaultHeight
        );
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

        private static Bitmap CreateScaledImage(Image source, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode =
                    System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                graphics.PixelOffsetMode =
                    System.Drawing.Drawing2D.PixelOffsetMode.Half;

                graphics.DrawImage(source, 0, 0, width, height);
            }

            return bitmap;
        }

        public void Draw(Graphics graphics)
        {
            if (!IsActive)
            {
                return;
            }

            graphics.DrawImageUnscaled(AlienImage, Bounds.X, Bounds.Y);
        }
    }
}