using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceInvaders
{
    public class Alien
    {
        public const int DefaultWidth = 36;
        public const int DefaultHeight = 24;

        private readonly Image image;

        public Rectangle Bounds { get; private set; }
        public bool IsActive { get; set; }

        public int RowIndex { get; private set; }
        public int PointValue { get; private set; }

        public Alien(int x, int y, int rowIndex)
        {
            RowIndex = rowIndex;
            PointValue = GameSettings.AlienRowPoints[rowIndex];

            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
            IsActive = true;

            image = CreateTintedImage(
                Properties.Resources.alien_png,
                GameSettings.AlienRowColors[rowIndex],
                DefaultWidth,
                DefaultHeight);
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

        private static Bitmap CreateTintedImage(
            Image source,
            Color color,
            int width,
            int height)
        {
            Bitmap scaledBitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(scaledBitmap))
            {
                graphics.InterpolationMode =
                    System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                graphics.PixelOffsetMode =
                    System.Drawing.Drawing2D.PixelOffsetMode.Half;

                graphics.DrawImage(source, 0, 0, width, height);
            }

            Bitmap tintedBitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color originalPixel = scaledBitmap.GetPixel(x, y);

                    if (originalPixel.A == 0)
                    {
                        tintedBitmap.SetPixel(x, y, Color.Transparent);
                        continue;
                    }

                    int brightness = (originalPixel.R + originalPixel.G + originalPixel.B) / 3;

                    Color tintedPixel = Color.FromArgb(
                        originalPixel.A,
                        color.R * brightness / 255,
                        color.G * brightness / 255,
                        color.B * brightness / 255);

                    tintedBitmap.SetPixel(x, y, tintedPixel);
                }
            }

            scaledBitmap.Dispose();

            return tintedBitmap;
        }

        public void Draw(Graphics graphics)
        {
            if (!IsActive)
            {
                return;
            }

            graphics.DrawImageUnscaled(image, Bounds.X, Bounds.Y);
        }
    }
}