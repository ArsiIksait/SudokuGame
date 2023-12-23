using SudokuGame;
using System.Runtime.InteropServices;

namespace 数独游戏
{
    enum Difficulty
    {
        Easy, Medium, Hard
    }

    struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x = 0, int y = 0) { X = x; Y = y; }
    }

    public partial class Form1 : Form
    {
        bool gameStart = false;
        bool generate = false;
        TimeSpan time = TimeSpan.Zero;
        private delegate void SetLabelTextCallback(Label label, string text);
        private delegate void SetButtonTextCallback(Button button, string text);
        private delegate void SetButtonTextColorCallback(Button button, Color color);
        //private delegate void SetControlTextCallback<T>(T control, string text) where T : Control;

        Task? timeTask = null;
        CancellationTokenSource tokenSource = new();
        Button[,] buttonBlock = new Button[9, 9];
        Block[,] sudokuBlock = new Block[9, 9];
        List<Button> tipButton = new();
        List<Button> Flashing = new();
        Sudoku sudoku = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingButton();
            return;
        }

        //开始游戏
        private void StartGame(Difficulty difficulty)
        {
            if (generate)
                return;

            if (gameStart)
                StopTimer();

            Task.Run(() =>
            {
                generate = true;

                foreach (var btn in tipButton.ToList())
                {
                    SetTipButtonChangeAble(btn, true);
                }

                switch (difficulty)
                {
                    case Difficulty.Easy:
                        sudokuBlock = new Block[9, 9];
                        sudoku.MakePart(sudokuBlock, 0, 0);
#if DEBUG
                        sudoku.ShowBoard(sudokuBlock);
#endif
                        SudokuBlockDig(4, 5);
                        break;
                    case Difficulty.Medium:
                        sudokuBlock = new Block[9, 9];
                        sudoku.MakePart(sudokuBlock, 0, 0);
                        SudokuBlockDig(3, 4);
                        break;
                    case Difficulty.Hard:
                        sudokuBlock = new Block[9, 9];
                        sudoku.MakePart(sudokuBlock, 0, 0);
                        SudokuBlockDig(2, 3);
                        break;
                }

                generate = false;

#if DEBUG
                sudoku.ShowBoard(sudokuBlock);
#endif
                UpdateGame();
                StartTimer();
            });
        }

        private void SetLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                var d = new SetLabelTextCallback(SetLabelText);

                if (d != null && !string.IsNullOrEmpty(text))
                    try
                    {
                        Invoke(d, label, text);
                    }
                    catch (Exception) { }
            }
            else
            {
                label.Text = text;
            }
        }

        private void SetButtonText(Button button, string text)
        {
            if (button.InvokeRequired)
            {
                var d = new SetButtonTextCallback(SetButtonText);

                if (d != null && !string.IsNullOrEmpty(text))
                    try
                    {
                        Invoke(d, button, text);
                    }
                    catch (Exception) { }
            }
            else
            {
                button.Text = text;
            }
        }

        private void SetButtonTextColor(Button button, Color color)
        {
            if (button.InvokeRequired)
            {
                var d = new SetButtonTextColorCallback(SetButtonTextColor);

                if (d != null)
                    try
                    {
                        Invoke(d, button, color);
                    }
                    catch (Exception) { }
            }
            else
            {
                button.ForeColor = color;
            }
        }

        private void StartTimer()
        {
            gameStart = true;
            time = TimeSpan.Zero;
            SetLabelText(time_text, time.ToString());
            CancellationToken cancellationToken = tokenSource.Token;

            timeTask = new Task(async () =>
            {
                while (gameStart)
                {
                    await Task.Delay(10);
                    time += TimeSpan.FromSeconds(0.015);
                    var s = time.ToString();
                    SetLabelText(time_text, s[..^8]);
                }
            }, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            timeTask.Start();
        }

        private void StopTimer()
        {
            if (timeTask != null && gameStart)
            {
                gameStart = false;
                tokenSource.Cancel();
                tokenSource = new();
                timeTask = null;
            }
        }

        //选择简单难度并开始游戏
        private void button1_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.FromName("HotTrack");
            button2.ForeColor = Color.FromName("MenuHighlight");
            button3.ForeColor = Color.FromName("MenuHighlight");
            StartGame(Difficulty.Easy);
        }

        //选择中等难度并开始游戏
        private void button2_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.FromName("MenuHighlight");
            button2.ForeColor = Color.FromName("HotTrack");
            button3.ForeColor = Color.FromName("MenuHighlight");
            StartGame(Difficulty.Medium);
        }

        //选择困难难度并开始游戏
        private void button3_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.FromName("MenuHighlight");
            button2.ForeColor = Color.FromName("MenuHighlight");
            button3.ForeColor = Color.FromName("HotTrack");
            StartGame(Difficulty.Hard);
        }

        //鼠标点击按钮格子，然后按键盘上的数字键填写数字
        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!gameStart)
                return;

            if ((e.KeyChar >= 48 && e.KeyChar <= 57) || (e.KeyChar >= 96 && e.KeyChar <= 105))
            {
                switch (e.KeyChar)
                {
                    case (char)48 or (char)96:
                        InputNumber(0);
                        break;
                    case (char)49 or (char)97:
                        InputNumber(1);
                        break;
                    case (char)50 or (char)98:
                        InputNumber(2);
                        break;
                    case (char)51 or (char)99:
                        InputNumber(3);
                        break;
                    case (char)52 or (char)100:
                        InputNumber(4);
                        break;
                    case (char)53 or (char)101:
                        InputNumber(5);
                        break;
                    case (char)54 or (char)102:
                        InputNumber(6);
                        break;
                    case (char)55 or (char)103:
                        InputNumber(7);
                        break;
                    case (char)56 or (char)104:
                        InputNumber(8);
                        break;
                    case (char)57 or (char)105:
                        InputNumber(9);
                        break;
                }

                e.Handled = true;
                //sudoku.ShowBoard(sudokuBlock);

                if (sudoku.VerifyAll(sudokuBlock))
                {
                    StopTimer();
                    MessageBox.Show("您赢了！用时: " + time_text.Text);
                }
            }
        }

        void InputNumber(int n)
        {
            if (GetFocusedControl() is Button btn)
            {
                if (tipButton.Contains(btn))
                    return;

                var pos = GetButtonPosition(btn);

                if (pos == null)
                    return;

                if (n == 0)
                {
                    SetButtonText(buttonBlock[pos.Value.X, pos.Value.Y], " ");
                    sudokuBlock[pos.Value.X, pos.Value.Y] = 0;
                }
                else
                {
                    SetButtonText(buttonBlock[pos.Value.X, pos.Value.Y], n.ToString());
                    sudokuBlock[pos.Value.X, pos.Value.Y] = n;
                }

                UpdateGame();
                if (n != 0)
                    DisplayErrorInput(pos.Value);
            }
        }

        /// <summary>
        /// 数独九宫格挖洞器
        /// </summary>
        /// <param name="minTip"></param>
        /// <param name="maxTip"></param>
        private void SudokuBlockDig(int minTip, int maxTip)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Position tipPos = new(i, j);
                    var block = GetNineSquareGrid(tipPos);
                    int nowTipCount = 0;

                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            int tipCount = Random.Shared.Next(minTip, maxTip + 1);
                            int probability = Random.Shared.Next(tipCount, 100);

                            if (Random.Shared.Next(0, 100) >= probability && nowTipCount < maxTip)
                            {
                                nowTipCount++;
                            }
                            else
                            {
                                block[m, n] = 0;
                            }
                        }
                    }

                    SetNineSquareGrid(tipPos, block);
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuBlock[i, j] != 0)
                    {
                        SetTipButtonChangeAble(buttonBlock[i, j], false);
                    }
                }
            }
        }

        private Block[,] GetNineSquareGrid(Position pos)
        {
            var block = new Block[3, 3];
            int bx = pos.X * 3;
            int by = pos.Y * 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    block[i, j] = sudokuBlock[bx + i, by + j];
                }
            }

            return block;
        }

        private void SetNineSquareGrid(Position pos, Block[,] block)
        {
            int bx = pos.X * 3;
            int by = pos.Y * 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    sudokuBlock[bx + i, by + j] = block[i, j];
                }
            }
        }

        private void SetTipButtonChangeAble(Button btn, bool changeAble)
        {
            if (changeAble)
            {
                SetButtonTextColor(btn, Color.FromName("MenuHighlight"));
                tipButton.Remove(btn);
            }
            else
            {
                SetButtonTextColor(btn, Color.Black);
                tipButton.Add(btn);
            }
        }

        private void UpdateGame()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var btn = buttonBlock[i, j];
                    int n = sudokuBlock[i, j];

                    if (n == 0)
                    {
                        SetButtonText(btn, " ");
                    }
                    else
                    {
                        SetButtonText(btn, n.ToString());
                    }
                }
            }
        }

        private void DisplayErrorInput(Position pos)
        {
            if (!sudoku.VerifyAllIgnoreZero(sudokuBlock))
            {
                ButtonTextFlashing(buttonBlock[pos.X, pos.Y], Color.Red);
            }
        }

        private void ButtonTextFlashing(Button btn, Color newColor)
        {
            if (Flashing.Contains(btn))
                return;

            Flashing.Add(btn);

            new Task(async () =>
            {
                Color oldColor = btn.ForeColor;

                for (int i = 0; i < 4; i++)
                {
                    SetButtonTextColor(btn, Color.Red);
                    await Task.Delay(100);
                    SetButtonTextColor(btn, oldColor);
                    await Task.Delay(100);
                }

                Flashing.Remove(btn);
            }).Start();
        }

        private Position? GetButtonPosition(Button btn)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (buttonBlock[i, j].Name == btn.Name)
                        return new Position(i, j);
                }
            }

            return null;
        }

        private void BindingButton()
        {
            buttonBlock[0, 0] = block1;
            buttonBlock[0, 1] = block2;
            buttonBlock[0, 2] = block3;
            buttonBlock[0, 3] = block4;
            buttonBlock[0, 4] = block5;
            buttonBlock[0, 5] = block6;
            buttonBlock[0, 6] = block7;
            buttonBlock[0, 7] = block8;
            buttonBlock[0, 8] = block9;
            buttonBlock[1, 0] = block10;
            buttonBlock[1, 1] = block11;
            buttonBlock[1, 2] = block12;
            buttonBlock[1, 3] = block13;
            buttonBlock[1, 4] = block14;
            buttonBlock[1, 5] = block15;
            buttonBlock[1, 6] = block16;
            buttonBlock[1, 7] = block17;
            buttonBlock[1, 8] = block18;
            buttonBlock[2, 0] = block19;
            buttonBlock[2, 1] = block20;
            buttonBlock[2, 2] = block21;
            buttonBlock[2, 3] = block22;
            buttonBlock[2, 4] = block23;
            buttonBlock[2, 5] = block24;
            buttonBlock[2, 6] = block25;
            buttonBlock[2, 7] = block26;
            buttonBlock[2, 8] = block27;
            buttonBlock[3, 0] = block28;
            buttonBlock[3, 1] = block29;
            buttonBlock[3, 2] = block30;
            buttonBlock[3, 3] = block31;
            buttonBlock[3, 4] = block32;
            buttonBlock[3, 5] = block33;
            buttonBlock[3, 6] = block34;
            buttonBlock[3, 7] = block35;
            buttonBlock[3, 8] = block36;
            buttonBlock[4, 0] = block37;
            buttonBlock[4, 1] = block38;
            buttonBlock[4, 2] = block39;
            buttonBlock[4, 3] = block40;
            buttonBlock[4, 4] = block41;
            buttonBlock[4, 5] = block42;
            buttonBlock[4, 6] = block43;
            buttonBlock[4, 7] = block44;
            buttonBlock[4, 8] = block45;
            buttonBlock[5, 0] = block46;
            buttonBlock[5, 1] = block47;
            buttonBlock[5, 2] = block48;
            buttonBlock[5, 3] = block49;
            buttonBlock[5, 4] = block50;
            buttonBlock[5, 5] = block51;
            buttonBlock[5, 6] = block52;
            buttonBlock[5, 7] = block53;
            buttonBlock[5, 8] = block54;
            buttonBlock[6, 0] = block55;
            buttonBlock[6, 1] = block56;
            buttonBlock[6, 2] = block57;
            buttonBlock[6, 3] = block58;
            buttonBlock[6, 4] = block59;
            buttonBlock[6, 5] = block60;
            buttonBlock[6, 6] = block61;
            buttonBlock[6, 7] = block62;
            buttonBlock[6, 8] = block63;
            buttonBlock[7, 0] = block64;
            buttonBlock[7, 1] = block65;
            buttonBlock[7, 2] = block66;
            buttonBlock[7, 3] = block67;
            buttonBlock[7, 4] = block68;
            buttonBlock[7, 5] = block69;
            buttonBlock[7, 6] = block70;
            buttonBlock[7, 7] = block71;
            buttonBlock[7, 8] = block72;
            buttonBlock[8, 0] = block73;
            buttonBlock[8, 1] = block74;
            buttonBlock[8, 2] = block75;
            buttonBlock[8, 3] = block76;
            buttonBlock[8, 4] = block77;
            buttonBlock[8, 5] = block78;
            buttonBlock[8, 6] = block79;
            buttonBlock[8, 7] = block80;
            buttonBlock[8, 8] = block81;
        }

        //API声明：获取当前焦点控件句柄      
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        ///获取 当前拥有焦点的控件
        private Control? GetFocusedControl()
        {

            Control? focusedControl = null;

            // To get hold of the focused control:

            IntPtr focusedHandle = GetFocus();

            if (focusedHandle != IntPtr.Zero)
                focusedControl = FromChildHandle(focusedHandle);

            return focusedControl;
        }
    }
}
