
namespace Snake
{
    public partial class MainForm : Form
    {

        static bool locked = false;
        static MoveEnum? moveTo = null;
        static List<Label> lables = [];
        static Label? food;
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (locked) return;


            MoveEnum? temp = null;
            if (e.KeyCode == Keys.Left) temp = MoveEnum.Left;
            if (e.KeyCode == Keys.Up) temp = MoveEnum.Up;
            if (e.KeyCode == Keys.Down) temp = MoveEnum.Down;
            if (e.KeyCode == Keys.Right) temp = MoveEnum.Right;

            if (temp is null || ((int)temp.Value % 2 == ((int)(moveTo ?? temp.Value + 1)) % 2))
                return;

            if (temp is not null)
                locked = true;

            moveTo = temp;
            snakeTimer.Interval = 300;
            snakeTimer.Start();
        }
        Label CreateLable(int top, int left)
        {

            var lable = new Label()
            {
                Height = 20,
                Width = 20,
                BackColor = Color.Black,
                Top = top,
                Left = left
            };
            return lable;
        }

        private void snakeTimer_Tick(object sender, EventArgs e)
        {
            if (lables.Count() > 1)
                for (int i = lables.Count(); i > 1; i--)
                {
                    var item = lables[i - 1];
                    var lastItem = lables[i - 2];

                    item.Left = lastItem.Left;
                    item.Top = lastItem.Top;
                }

            if (moveTo.HasValue)
            {
                var head = lables[0];
                if (moveTo.Value == MoveEnum.Left)
                    head.Left -= 20;

                if (moveTo.Value == MoveEnum.Right)
                    head.Left += 20;

                if (moveTo.Value == MoveEnum.Up)
                    head.Top -= 20;

                if (moveTo.Value == MoveEnum.Down)
                    head.Top += 20;

                if (lables.Any(p => p.Top == head.Top && p.Left == head.Left && p.GetHashCode() != head.GetHashCode()))
                    RefreshGame();

                if (head.Top < 0 || head.Left < 0 || head.Left > this.DisplayRectangle.Width - 20 || head.Top > this.DisplayRectangle.Height - 20)
                    RefreshGame();

                if (food is not null && head.Top == food.Top && head.Left == food.Left)
                    Console.Beep();

            }
            if (lables.Any())
            {
                var lastitem = lables.Last();
                if (food is not null && lastitem.Top == food.Top && lastitem.Left == food.Left)
                {
                    Text = $"Snake ({lables.Count()})";
                    food.BackColor = Color.Black;
                    lables.Add(food);
                    CreateFood();
                }
            }
            locked = false;

        }

        private void RefreshGame()
        {
            snakeTimer.Stop();
            MessageBox.Show("You lose. Close the message to reset the game!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

            foreach (var item in lables)
                Controls.Remove(item);
            Controls.Remove(food);

            lables.Clear();
            moveTo = null;
            food = null;
            LoadGame();
            Text = "Snake";

        }
        private void CreateFood()
        {
            var random = new Random();
            var rndleft = random.Next((this.DisplayRectangle.Width - 20) / 20) * 20;
            var rndTop = random.Next((this.DisplayRectangle.Height - 20) / 20) * 20;
            food = CreateLable(rndTop, rndleft);
            food.BackColor = Color.Red;
            this.Controls.Add(food);
        }

        private void LoadGame()
        {
            var x = DisplayRectangle.Width / 2;
            var y = DisplayRectangle.Height / 2;
            x -= x % 20;
            y -= y % 20;
            lables.Add(CreateLable(y, x));
            Controls.Add(lables[0]);
            CreateFood();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            MainForm_ResizeEnd(sender, e);
            LoadGame();
        }
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            Width = DisplayRectangle.Width - (DisplayRectangle.Width % 20) + Width - DisplayRectangle.Width;
            Height = DisplayRectangle.Height - (DisplayRectangle.Height % 20) + Height - DisplayRectangle.Height;
        }
    }
    enum MoveEnum
    {
        Left,
        Up,
        Right,
        Down,
    }
}
