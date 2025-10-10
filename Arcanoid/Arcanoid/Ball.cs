
namespace Arcanoid
{
    public class Ball
    {
        public float X, Y;    // Позиция
        public float VX, VY;  // Скорость
        public float R = 8;   // Радиус

        public Ball(float x, float y, float vx, float vy)
        {
            X = x;
            Y = y;
            VX = vx;
            VY = vy;
        }
    }
}
