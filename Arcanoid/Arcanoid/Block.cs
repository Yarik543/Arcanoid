using System.Drawing;

namespace Arcanoid
{
    public class Block
    {
        public Rectangle Rect;
        public bool Alive = true;
        public Color Color;

        public Block(Rectangle rect, Color color)
        {
            Rect = rect;
            Color = color;
        }
    }
}
