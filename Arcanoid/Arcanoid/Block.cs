using System.Drawing;

namespace Arcanoid
{
    public class Block
    {
        public Rectangle rect;
        public bool IsAlive = true;
        public Color Color { get; private set; }

        public Block(int x, int y, Color c, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
            Color = c;
        }
    }
}
