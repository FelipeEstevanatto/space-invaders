using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceInvaders
{
    public class Alien
    {
        public const int DefaultWidth = 30;
        public const int DefaultHeight = 30;
        // Static cache to hold only 4 colors of alien images (one for each row)
        private static readonly Dictionary<int, Image> rowImageCache = new Dictionary<int, Image>();

        private readonly Image image;

        public Rectangle Bounds { get; private set; }
        public bool IsActive { get; private set; }

        public int RowIndex { get; private set; }
        public int PointValue { get; private set; }

        public Alien(int x, int y, int rowIndex)
        {
            RowIndex = rowIndex;
            PointValue = GameSettings.AlienRowPoints[rowIndex];

            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
            IsActive = true;

            // CHECK THE CACHE BEFORE GENERATING
            if (!rowImageCache.ContainsKey(rowIndex))
            {
                rowImageCache[rowIndex] = CreateTintedImage(
                    Properties.Resources.alien_png,
                    GameSettings.AlienRowColors[rowIndex],
                    DefaultWidth,
                    DefaultHeight);
            }

            // ASSIGN THE CACHED IMAGE
            image = rowImageCache[rowIndex];
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

                    // 1. Preserve transparency
                    if (originalPixel.A == 0)
                    {
                        tintedBitmap.SetPixel(x, y, Color.Transparent);
                        continue;
                    }

                    // 2. Calculate brightness to detect black/dark pixels
                    int brightness = (originalPixel.R + originalPixel.G + originalPixel.B) / 3;

                    // 3. If it's a dark pixel, preserve the original color
                    // A threshold of 50 safely catches black and very dark gray without catching the white body
                    if (brightness < 50) 
                    {
                        tintedBitmap.SetPixel(x, y, Color.FromArgb(originalPixel.A, originalPixel.R, originalPixel.G, originalPixel.B));
                    }
                    // 4. If it's a light pixel, apply the bright row color
                    else 
                    {
                        tintedBitmap.SetPixel(x, y, Color.FromArgb(originalPixel.A, color.R, color.G, color.B));
                    }
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

        public void Destroy()
        {
            IsActive = false;
        }
    }
}