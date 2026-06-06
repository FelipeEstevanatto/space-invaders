using System;
using System.Drawing;

namespace SpaceInvaders
{
    public sealed class Explosion : IDisposable
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;

        private const int DurationTicks = 25;

        private readonly Image gifImage;
        private int currentTick;
        private bool disposed;

        public Rectangle Bounds { get; private set; }
        public bool IsActive { get; private set; }

        public Explosion(Rectangle targetBounds)
        {
            gifImage = (Image)Properties.Resources.explosion.Clone();

            int x = targetBounds.X + targetBounds.Width / 2 - DefaultWidth / 2;
            int y = targetBounds.Y + targetBounds.Height / 2 - DefaultHeight / 2;

            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
            IsActive = true;

            ImageAnimator.Animate(gifImage, OnFrameChanged);
        }

        public void Update()
        {
            currentTick++;

            if (currentTick >= DurationTicks)
            {
                IsActive = false;
            }
        }

        public void Draw(Graphics graphics)
        {
            if (!IsActive || disposed)
            {
                return;
            }

            ImageAnimator.UpdateFrames(gifImage);
            graphics.DrawImage(gifImage, Bounds);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            // The game loop already invalidates the form.
            // Nothing is needed here.
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            ImageAnimator.StopAnimate(gifImage, OnFrameChanged);
            gifImage.Dispose();

            disposed = true;
        }
    }
}