using System.Drawing;

namespace Arcanoid
{
    public class Paddle
    {
        public Rectangle rect;

        public Paddle(Size clientSize)
        {
            rect = new Rectangle(clientSize.Width / 2 - 50, clientSize.Height - 40, 100, 15);
        }

        public void MoveToMouse(int mouseX, Size clientSize)
        {
            int half = rect.Width / 2;
            rect.X = mouseX - half;

            // чтобы не выходила за границы
            if (rect.X < 0) rect.X = 0;
            if (rect.X + rect.Width > clientSize.Width)
                rect.X = clientSize.Width - rect.Width;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, rect);
        }
    }
}
