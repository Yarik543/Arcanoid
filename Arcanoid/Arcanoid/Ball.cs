using System.Drawing;

namespace Arcanoid
{
    public class Ball
    {
        public Rectangle rect;
        private int dx = 6, dy = -6;
        public bool IsLost { get; private set; }
        public bool IsLaunched { get; set; }

        public Ball(Paddle paddle)
        {
            rect = new Rectangle(0, 0, 16, 16);
            FollowPaddle(paddle);
        }

        public void FollowPaddle(Paddle paddle)
        {
            rect.X = paddle.rect.X + paddle.rect.Width / 2 - rect.Width / 2;
            rect.Y = paddle.rect.Y - rect.Height;
        }

        public void Move(Size clientSize, Paddle paddle, Block[] blocks)
        {
            rect.X += dx;
            rect.Y += dy;

            // столкновение со стенками
            if (rect.Left < 0 || rect.Right > clientSize.Width) dx = -dx;
            if (rect.Top < 0) dy = -dy;

            // мяч улетел вниз
            if (rect.Bottom > clientSize.Height)
            {
                IsLost = true;
                IsLaunched = false;
                return;
            }

            // от платформы
            if (rect.IntersectsWith(paddle.rect))
            {
                dy = -dy;

                // изменение направления от точки касания
                int hitPoint = rect.X + rect.Width / 2 - paddle.rect.X;
                if (hitPoint < paddle.rect.Width / 3) dx = -Math.Abs(dx);
                else if (hitPoint > paddle.rect.Width * 2 / 3) dx = Math.Abs(dx);
            }

            // от блоков
            foreach (var b in blocks)
            {
                if (b.IsAlive && rect.IntersectsWith(b.rect))
                {
                    b.IsAlive = false;
                    dy = -dy;
                    break;
                }
            }
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.White, rect);
        }
    }
}
