using System.Drawing;

namespace Arcanoid
{
    public class Ball
    {
        // поля
        private readonly Size _clientSize;
        private int _dx = 8;
        private int _dy = -8;

        // свойства
        public Rectangle Rect { get; set; }
        public int Dx { get => _dx; set => _dx = value; }
        public int Dy { get => _dy; set => _dy = value; }
        public bool IsLaunched { get; set; }
        public bool IsLost => Rect.Bottom > _clientSize.Height;



        // конструктор
        public Ball(Paddle paddle, Size clientSize)
        {
            _clientSize = clientSize;
            Rect = new Rectangle(0, 0, 16, 16);
            FollowPaddle(paddle);
        }

        // методы
        public void FollowPaddle(Paddle paddle)
        {
            Rect = new Rectangle(
                paddle.Rect.X + paddle.Rect.Width / 2 - Rect.Width / 2,
                paddle.Rect.Y - Rect.Height,
                Rect.Width,
                Rect.Height
            );
        }

    }
}
