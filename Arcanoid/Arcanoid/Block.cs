using System.Drawing;

namespace Arcanoid
{
    public class Block
    {
        public Rectangle rect;
        public bool IsAlive = true;
        private Color color;

        // добавили width и height
        public Block(int x, int y, Color c, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
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
