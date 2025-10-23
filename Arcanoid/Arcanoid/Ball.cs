using System.Drawing;

namespace Arcanoid
{
    public class Ball
    {
        public Rectangle rect;
        private int dx = 6;
        private int dy = -6;

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

        public void Move(Paddle paddle, Block[] blocks)
        {
            if (!IsLaunched) return;

            rect.X += dx;
            rect.Y += dy;

            // стенки
            if (rect.Left < 0 || rect.Right > _clientSize.Width) dx = -dx;
            if (rect.Top < 0) dy = -dy;

            // платформа
            if (rect.IntersectsWith(paddle.rect))
            {
                dy = -dy;

                int hitPoint = rect.X + rect.Width / 2 - paddle.rect.X;
                if (hitPoint < paddle.rect.Width / 3) dx = -Math.Abs(dx);
                else if (hitPoint > paddle.rect.Width * 2 / 3) dx = Math.Abs(dx);
            }

            // от блоков
            foreach (var b in blocks)
            {
                if (b.IsAlive && rect.IntersectsWith(b.rect))
                {
                    // вычисляем, с какой стороны удар
                    Rectangle intersection = Rectangle.Intersect(rect, b.rect);

                    if (intersection.Width > intersection.Height)
                    {
                        // удар сверху/снизу
                        dy = -dy;
                    }
                    else
                    {
                        // удар слева/справа
                        dx = -dx;
                    }

                    b.IsAlive = false;
                    break;
                }
            }
        }
    }
}
