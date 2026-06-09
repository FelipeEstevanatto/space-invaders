using System.Windows.Forms;

namespace SpaceInvaders
{
    public class InputController
    {
        public bool IsMovingLeft { get; private set; }
        public bool IsMovingRight { get; private set; }
        public bool IsShooting { get; private set; }
        public bool IsRestarting { get; private set; }

        public void KeyDown(Keys key)
        {
            if (key == Keys.Left || key == Keys.A) IsMovingLeft = true;
            if (key == Keys.Right || key == Keys.D) IsMovingRight = true;
            if (key == Keys.Space) IsShooting = true;
            if (key == Keys.R) IsRestarting = true;
        }

        public void KeyUp(Keys key)
        {
            if (key == Keys.Left || key == Keys.A) IsMovingLeft = false;
            if (key == Keys.Right || key == Keys.D) IsMovingRight = false;
            if (key == Keys.Space) IsShooting = false;
            if (key == Keys.R) IsRestarting = false;
        }

        public void ClearRestart()
        {
            IsRestarting = false;
        }
    }
}