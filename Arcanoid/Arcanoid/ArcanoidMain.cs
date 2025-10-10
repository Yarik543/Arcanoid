namespace Arcanoid
{
    public partial class ArcanoidMain : Form
    {

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Block[,] blocks;
        Ball ball;
        Rectangle paddle;
        int rows = 5, cols = 10;
        int blockW, blockH;
        Random rnd = new Random();
        public ArcanoidMain()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ClientSize = new Size(800, 600);

            InitLevel();
            InitBallAndPaddle();

            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            timer.Start();

            Paint += Form1_Paint;
            MouseMove += Form1_MouseMove;
        }


        // ==== СОЗДАЁМ УРОВЕНЬ ====
        void InitLevel()
        {
            int margin = 20;
            int spacing = 5;
            blockW = (ClientSize.Width - margin * 2 - spacing * (cols - 1)) / cols;
            blockH = 25;
            blocks = new Block[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int x = margin + c * (blockW + spacing);
                    int y = margin + r * (blockH + spacing);
                    Color color = Color.FromArgb(rnd.Next(100, 256), rnd.Next(100, 256), rnd.Next(100, 256));
                    blocks[r, c] = new Block(new Rectangle(x, y, blockW, blockH), color);
                }
            }
        }

        void InitBallAndPaddle()
        {
            paddle = new Rectangle(ClientSize.Width / 2 - 50, ClientSize.Height - 50, 100, 12);
            ball = new Ball(ClientSize.Width / 2, paddle.Y - 20, 3, -4);
        }

        // ==== УПРАВЛЕНИЕ ====
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int newX = e.X - paddle.Width / 2;

            // Проверка границ
            if (newX < 0) newX = 0;
            if (newX + paddle.Width > ClientSize.Width) newX = ClientSize.Width - paddle.Width;

            paddle.X = newX;
        }

        // ==== ГЛАВНЫЙ ЦИКЛ ====
        private void Timer_Tick(object sender, EventArgs e)
        {
            ball.X += ball.VX;
            ball.Y += ball.VY;

            // Стенки
            if (ball.X - ball.R < 0) { ball.X = ball.R; ball.VX = -ball.VX; }
            if (ball.X + ball.R > ClientSize.Width) { ball.X = ClientSize.Width - ball.R; ball.VX = -ball.VX; }
            if (ball.Y - ball.R < 0) { ball.Y = ball.R; ball.VY = -ball.VY; }

            // Платформа
            RectangleF ballRect = new RectangleF(ball.X - ball.R, ball.Y - ball.R, ball.R * 2, ball.R * 2);
            if (ballRect.IntersectsWith(paddle))
            {
                ball.VY = -Math.Abs(ball.VY);
            }

            // Блоки
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var b = blocks[r, c];
                    if (b == null || !b.Alive) continue;

                    if (ballRect.IntersectsWith(b.Rect))
                    {
                        b.Alive = false;
                        ball.VY = -ball.VY;
                        goto AfterBlocks;
                    }
                }
            }

        AfterBlocks:

            // Мяч упал вниз — рестарт
            if (ball.Y > ClientSize.Height)
            {
                InitBallAndPaddle();
            }

            Invalidate();
        }

        // ==== ОТРИСОВКА ====
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Блоки
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var b = blocks[r, c];
                    if (b != null && b.Alive)
                    {
                        using (Brush br = new SolidBrush(b.Color))
                            g.FillRectangle(br, b.Rect);

                    }
                }
            }

            // Платформа
            g.FillRectangle(Brushes.DodgerBlue, paddle);

            // Мяч
            g.FillEllipse(Brushes.Red, ball.X - ball.R, ball.Y - ball.R, ball.R * 2, ball.R * 2);
        }
    }
}