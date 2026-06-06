using System.Drawing;

namespace SpaceInvaders
{
    public enum ProjectileOwner
    {
        Player,
        Alien
    }

    public class Projectile
    {
        public const int DefaultWidth = 4;
        public const int DefaultHeight = 12;

        private readonly int speedY;

        public Rectangle Bounds { get; private set; }
        public ProjectileOwner Owner { get; private set; }
        public bool IsActive { get; set; }

        public Projectile(int x, int y, int speedY, ProjectileOwner owner)
        {
            this.speedY = speedY;

            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
            Owner = owner;
            IsActive = true;
        }

        public void Update()
        {
            Bounds = new Rectangle(
                Bounds.X,
                Bounds.Y + speedY,
                Bounds.Width,
                Bounds.Height);
        }

        public void Draw(Graphics graphics)
        {
            if (!IsActive)
            {
                return;
            }

            Brush brush = Owner == ProjectileOwner.Player
                ? Brushes.White
                : Brushes.Red;

            graphics.FillRectangle(brush, Bounds);
        }
    }
}