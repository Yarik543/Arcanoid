using System.Drawing;

namespace Arcanoid
{
    public class Block
    {
        public Rectangle rect;
        public bool IsAlive = true;
        private Color color;

        public Block(int x, int y, Color c)
        {
            rect = new Rectangle(x, y, 70, 20);
            color = c;
        }

        public void Draw(Graphics g)
        {
            using (Brush br = new SolidBrush(color))
                g.FillRectangle(br, rect);
            g.DrawRectangle(Pens.Black, rect);
        }
    }
}
