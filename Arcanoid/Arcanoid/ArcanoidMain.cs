using System.Reflection.Metadata;

namespace Arcanoid
{
    public partial class ArcanoidMain : Form
    {

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Paddle paddle;
        Ball ball;
        Block[] blocks;

        public ArcanoidMain()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 800;
            this.Height = 600;

            StartGame();

            timer.Interval = 16; // ~60 FPS
            timer.Tick += Update;
            timer.Start();

            this.MouseMove += GameForm_MouseMove;
            this.MouseDown += GameForm_MouseDown; // клик для запуска
        }

        private void StartGame()
        {
            paddle = new Paddle(this.ClientSize);
            ball = new Ball(paddle);
            blocks = CreateBlocks();
        }

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            paddle.MoveToMouse(e.X, this.ClientSize);

            // если мяч ещё не запущен — он двигается вместе с платформой
            if (!ball.IsLaunched)
                ball.FollowPaddle(paddle);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            // по клику запускаем мяч
            ball.IsLaunched = true;
        }

        private void Update(object sender, EventArgs e)
        {
            if (ball.IsLaunched)
                ball.Move(this.ClientSize, paddle, blocks);

            bool allBroken = true;
            foreach (var b in blocks)
                if (b.IsAlive) { allBroken = false; break; }

            if (ball.IsLost)
            {
                timer.Stop();
                MessageBox.Show("Вы проиграли!", "Арканоид");
                StartGame();
                timer.Start();
            }

            if (allBroken)
            {
                timer.Stop();
                MessageBox.Show("Вы выиграли!", "Арканоид");
                StartGame();
                timer.Start();
            }

            Invalidate(); // перерисовать форму
        }

        private Block[] CreateBlocks()
        {
            // Цвета линий
            Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green };
            int cols = 8, rows = colors.Length;
            Block[] arr = new Block[cols * rows];
            int index = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    arr[index++] = new Block(
                        80 + x * 80,
                        50 + y * 30,
                        colors[y]
                    );
                }
            }
            return arr;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            paddle.Draw(g);
            ball.Draw(g);
            foreach (var b in blocks)
                if (b.IsAlive) b.Draw(g);
        }
    }
}