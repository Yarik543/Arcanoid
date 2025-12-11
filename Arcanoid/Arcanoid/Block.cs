using System.Drawing;

namespace Arcanoid
{
    public class Block
    {
        public Rectangle Rect { get; private set; }
        public bool IsAlive { get; set; } = true;
        public Color Color { get; }

        public Block(int x, int y, Color c, int width, int height)
        {
            Rect = new Rectangle(x, y, width, height);
            Color = c;
        }
    }
}
