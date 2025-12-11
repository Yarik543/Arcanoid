using System.Drawing;

namespace Arcanoid
{
    public class Paddle
    {
        public Rectangle Rect { get; private set; }

        public Paddle(Size clientSize)
        {
            Rect = new Rectangle(clientSize.Width / 2 - 50, clientSize.Height - 40, 100, 15);
        }

        public void MoveToMouse(int mouseX, Size clientSize)
        {
            var half = Rect.Width / 2;
            var newX = mouseX - half;

            if (newX < 0)
            {
                newX = 0;
            }
            else if (newX + Rect.Width > clientSize.Width)
            {
                newX = clientSize.Width - Rect.Width;
            }

            Rect = new Rectangle(newX, Rect.Y, Rect.Width, Rect.Height);
        }
    }
}
