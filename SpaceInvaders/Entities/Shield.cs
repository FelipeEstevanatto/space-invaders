using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    public class Shield
    {
        public const int DefaultWidth = 70;
        public const int DefaultHeight = 40;

        public Rectangle Bounds { get; private set; }
        public int Health { get; private set; }
        public bool IsActive => Health > 0;

        public Shield(int x, int y, int initialHealth)
        {
            Bounds = new Rectangle(x, y, DefaultWidth, DefaultHeight);
            Health = initialHealth;
        }

        public void TakeDamage()
        {
            if (Health > 0)
            {
                Health--;
            }
        }
    }
}
