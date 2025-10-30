using System.Reflection.Metadata;

namespace Arcanoid
{
    public partial class ArcanoidMain : Form
    {

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private Paddle paddle;

        private Ball ball;

        private Block[] blocks;

        // создаем кисть в память
        private Brush paddleBrush = Brushes.Blue;
        private Brush ballBrush = Brushes.White;
        private Pen blockBorderPen = Pens.Black;

        // кеш кистей для цветов блоков 
        private Dictionary<Color, Brush> blockBrushes = new Dictionary<Color, Brush>();

        public ArcanoidMain()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.Width = 800;
            this.Height = 600;

            StartGame();

            timer.Interval = 16;
            timer.Tick += Update;
            timer.Start();

            this.MouseMove += GameForm_MouseMove;
            this.MouseDown += GameForm_MouseDown;

            // Подписываемся на событие Paint
            this.Paint += GameDraw;
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
            Graphics g = e.Graphics;

            // отрисовка платформы
            g.FillRectangle(paddleBrush, paddle.rect);
            g.DrawRectangle(Pens.Black, paddle.rect);

            // отрисовка мяча
            g.FillEllipse(ballBrush, ball.rect);
            g.DrawEllipse(Pens.Black, ball.rect);

            // отрисовка блоков (используем кешированные кисти)
            foreach (var b in blocks)
            {
                if (!b.IsAlive)
                {
                    continue;
                }

                if (blockBrushes.TryGetValue(b.Color, out Brush br))
                {
                    g.FillRectangle(br, b.rect);
                }

                g.DrawRectangle(blockBorderPen, b.rect);
            }
        }

        private void StartGame()
        {
            paddle = new Paddle(this.ClientSize);
            ball = new Ball(paddle, this.ClientSize);
            blocks = CreateBlocks();
            EnsureBlockBrushes(); // <- добавь сюда
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
                ball.rect.X += ball.dx;
                ball.rect.Y += ball.dy;

                // стены
                if (ball.rect.Left < 0 || ball.rect.Right > this.ClientSize.Width)
                {
                    ball.dx = -ball.dx;
                }

                if (ball.rect.Top < 0)
                {
                    ball.dy = -ball.dy;
                }

                // платформа
                if (ball.rect.IntersectsWith(paddle.rect))
                {
                    ball.dy = -ball.dy;

                    var hitPoint = ball.rect.X + ball.rect.Width / 2 - paddle.rect.X;
                    if (hitPoint < paddle.rect.Width / 3)
                    {
                        ball.dx = -Math.Abs(ball.dx);
                    }
                    else if (hitPoint > paddle.rect.Width * 2 / 3)
                    {
                        ball.dx = Math.Abs(ball.dx);
                    }
                }

                // блоки
                foreach (var b in blocks)
                {
                    if (b.IsAlive && ball.rect.IntersectsWith(b.rect))
                    {
                        var intersection = Rectangle.Intersect(ball.rect, b.rect);

                        if (intersection.Width > intersection.Height)
                        {
                            ball.dy = -ball.dy;
                        }
                        else
                        {
                            ball.dx = -ball.dx;
                        }

                        b.IsAlive = false;
                        break;
                    }
                }
            }

            // поражение
            if (ball.rect.Bottom > this.ClientSize.Height)
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