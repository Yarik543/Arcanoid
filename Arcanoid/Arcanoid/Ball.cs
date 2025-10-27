using System.Drawing;

namespace Arcanoid
{
    public class Ball
    {
        public Rectangle rect;

        public int dx = 6;

        public int dy = -6;

        public bool IsLaunched { get; set; }
        public bool IsLost => rect.Bottom > _clientSize.Height;

        private Size _clientSize;

        public Ball(Paddle paddle, Size clientSize)
        {
            _clientSize = clientSize;
            rect = new Rectangle(0, 0, 16, 16);
            FollowPaddle(paddle);
        }

        public void FollowPaddle(Paddle paddle)
        {
            rect.X = paddle.rect.X + paddle.rect.Width / 2 - rect.Width / 2;
            rect.Y = paddle.rect.Y - rect.Height;
        }

    }
}
