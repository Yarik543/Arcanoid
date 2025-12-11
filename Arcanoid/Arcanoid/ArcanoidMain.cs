using System.Reflection.Metadata;

namespace Arcanoid
{
    public partial class ArcanoidMain : Form
    {

        private readonly System.Windows.Forms.Timer timer = new();
        private Paddle paddle;
        private Ball ball;
        private Block[] blocks;

        // кисти и ручки
        private readonly Brush paddleBrush = Brushes.Blue;
        private readonly Brush ballBrush = Brushes.White;
        private readonly Pen blockBorderPen = Pens.Black;

        // кеш кистей для блоков
        private readonly Dictionary<Color, Brush> blockBrushes = new();


        public ArcanoidMain()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Width = 800;
            Height = 600;

            StartGame();

            timer.Interval = 16;
            timer.Tick += Update;
            timer.Start();

            MouseMove += GameForm_MouseMove;
            MouseDown += GameForm_MouseDown;
            Paint += GameDraw;
        }

        // после создания блоков наполним кеш кистей (в StartGame или сразу после CreateBlocks)
        private void EnsureBlockBrushes()
        {
            blockBrushes.Clear();
            foreach (var b in blocks)
            {
                if (!blockBrushes.ContainsKey(b.Color))
                {
                    blockBrushes[b.Color] = new SolidBrush(b.Color);
                }
            }
        }

        private void GameDraw(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            // платформа
            g.FillRectangle(paddleBrush, paddle.Rect);
            g.DrawRectangle(Pens.Black, paddle.Rect);

            // мяч
            g.FillEllipse(ballBrush, ball.Rect);
            g.DrawEllipse(Pens.Black, ball.Rect);

            // блоки
            foreach (var b in blocks)
            {
                if (!b.IsAlive) continue;

                if (blockBrushes.TryGetValue(b.Color, out var br))
                {
                    g.FillRectangle(br, b.Rect);
                }

                g.DrawRectangle(blockBorderPen, b.Rect);
            }
        }

        private void StartGame()
        {
            paddle = new Paddle(this.ClientSize);
            ball = new Ball(paddle, this.ClientSize);
            blocks = CreateBlocks();
            EnsureBlockBrushes(); 
        }

        private void ResetGame()
        {
            StartGame();
            Invalidate(); // сразу перерисовываем
        }

        private void Update(object? sender, EventArgs e)
        {
            if (ball.IsLaunched)
            {
                // движение
                ball.Rect = new Rectangle(
                    ball.Rect.X + ball.Dx,
                    ball.Rect.Y + ball.Dy,
                    ball.Rect.Width,
                    ball.Rect.Height
                );

                // стены
                if (ball.Rect.Left < 0 || ball.Rect.Right > ClientSize.Width)
                {
                    ball.Dx = -ball.Dx;
                }

                if (ball.Rect.Top < 0)
                {
                    ball.Dy = -ball.Dy;
                }

                // платформа
                if (ball.Rect.IntersectsWith(paddle.Rect))
                {
                    ball.Dy = -ball.Dy;

                    var hitPoint = ball.Rect.X + ball.Rect.Width / 2 - paddle.Rect.X;
                    if (hitPoint < paddle.Rect.Width / 3)
                    {
                        ball.Dx = -Math.Abs(ball.Dx);
                    }
                    else if (hitPoint > paddle.Rect.Width * 2 / 3)
                    {
                        ball.Dx = Math.Abs(ball.Dx);
                    }
                }

                // блоки
                foreach (var b in blocks)
                {
                    if (b.IsAlive && ball.Rect.IntersectsWith(b.Rect))
                    {
                        var intersection = Rectangle.Intersect(ball.Rect, b.Rect);

                        if (intersection.Width > intersection.Height)
                        {
                            ball.Dy = -ball.Dy;
                        }
                        else
                        {
                            ball.Dx = -ball.Dx;
                        }

                        b.IsAlive = false;
                        break;
                    }
                }
            }

            // поражение
            if (ball.Rect.Bottom > ClientSize.Height)
            {
                timer.Stop();
                MessageBox.Show("Вы проиграли!", "Арканоид");
                ResetGame();
                timer.Start();
                return;
            }

            // победа
            var allBroken = true;
            foreach (var b in blocks)
            {
                if (b.IsAlive)
                {
                    allBroken = false;
                    break;
                }
            }

            if (allBroken)
            {
                timer.Stop();
                MessageBox.Show("Вы выиграли!", "Арканоид");
                ResetGame();
                timer.Start();
                return;
            }

            Invalidate();
        }
        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            paddle.MoveToMouse(e.X, this.ClientSize);

            // если мяч ещё не запущен — он двигается вместе с платформой
            if (!ball.IsLaunched)
            {
                ball.FollowPaddle(paddle);
            }
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            // по клику запускаем мяч
            ball.IsLaunched = true;
        }


        private Block[] CreateBlocks()
        {
            // Цвета строк
            var colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green };

            var rows = colors.Length;
            var cols = 10; // сколько блоков в ряду
            var total = rows * cols;
            var arr = new Block[total];

            // ширина блока = ширина окна / кол-во столбцов
            var blockWidth = this.ClientSize.Width / cols;
            var blockHeight = 25;

            var index = 0;
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    arr[index++] = new Block(
                        x * blockWidth,
                        50 + y * blockHeight,
                        colors[y],
                        blockWidth,
                        blockHeight
                    );
                }
            }
            return arr;
        }

    }
}