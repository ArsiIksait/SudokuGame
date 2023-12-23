namespace SudokuGame
{
    public struct Block
    {
        public int Value { get; set; }
        public bool IsPinned { get; set; }

        public Block(int value, bool isPinned = false) { Value = value; IsPinned = isPinned; }

        public static implicit operator Block(int value) => new() { Value = value };
        public static implicit operator int(Block block) => block.Value;

        public static Block Pin(int value) => new(value, true);
    }

    public class Sudoku
    {
        public Block[,] Create() // 从空数独创建
        {
            var board = new Block[9, 9];

            MakePart(board, 0, 0);

            return board;
        }

        public Block[,] CreateCustom() // 创建指定数独
        {
            var board = new Block[9, 9];
            board[0, 0] = Block.Pin(1);
            board[0, 1] = Block.Pin(2);
            board[0, 2] = Block.Pin(3);
            board[1, 0] = Block.Pin(4);
            board[1, 1] = Block.Pin(5);
            board[1, 2] = Block.Pin(6);
            board[2, 0] = Block.Pin(7);
            board[2, 1] = Block.Pin(8);
            board[2, 2] = Block.Pin(9);

            board[0, 3] = Block.Pin(4);
            board[0, 4] = Block.Pin(5);
            board[0, 5] = Block.Pin(6);
            board[0, 6] = Block.Pin(7);
            board[0, 7] = Block.Pin(8);
            board[0, 8] = Block.Pin(9);

            Sudoku test = new();
            test.MakePart(board, 0, 0);

            //test.PrintBoard(board);

            //Console.WriteLine($"数独验证: {test.VerifyAll(board)}");

            return board;
        }

        public bool MakePart(Block[,] arr, int y, int x)
        {
            if (arr[y, x].IsPinned)
            {
                int newX = x == 8 ? 0 : x + 1;
                int newY = x == 8 ? y + 1 : y;
                if (newY > 8)
                {
                    return true; // 已经到达末尾, 生成完成
                }

                if (MakePart(arr, newY, newX))
                    return true; // 生成成功, 结束递归
            }

            int ava = 0; // 已经生成过的数字
            while (true)
            {
                if (ava == 0b1111111110)
                {
                    arr[y, x].Value = 0; //回溯
                    return false;
                }

                var rand = GetRandom();
                if (((1 << rand) & ava) != 0)
                {
                    //生成重复
                    continue;
                }

                ava |= 1 << rand;
                if (!Verify(arr, x, y, rand))
                    continue;

                arr[y, x].Value = rand;

                int newX = x == 8 ? 0 : x + 1;
                int newY = x == 8 ? y + 1 : y;
                if (newY > 8)
                {
                    return true; // 已经到达末尾, 生成完成
                }

                if (MakePart(arr, newY, newX))
                    return true; // 生成成功, 结束递归

                //被回溯的地方
                continue;
            }
        }

        public bool Verify(Block[,] arr, int x, int y, int newValue)
        {
            for (int i = 0; i < 9; i++) // 检测当前行是否有相同
            {
                if (arr[y, i].Value == newValue)
                    return false;
            }
            for (int i = 0; i < 9; i++) // 检测当前列是否有相同
            {
                if (arr[i, x].Value == newValue)
                    return false;
            }

            //判断当前宫格是否有重复
            var xRange = x - x % 3;
            var yRange = y - y % 3;
            for (int i = yRange; i < yRange + 3; i++)
            {
                for (int j = xRange; j < xRange + 3; j++)
                {
                    if (arr[i, j].Value == newValue)
                        return false;
                }
            }
            return true;
        }

        public bool VerifyAll(Block[,] arr)
        {
            arr = (Block[,])arr.Clone();
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    var temp = arr[y, x].Value;
                    arr[y, x].Value = 0;
                    if (!Verify(arr, x, y, temp))
                    {
                        return false;
                    }
                    arr[y, x].Value = temp;
                }
            }
            return true;
        }

        public bool VerifyAllIgnoreZero(Block[,] arr)
        {
            arr = (Block[,])arr.Clone();
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    var temp = arr[y, x].Value;
                    arr[y, x].Value = 0;

                    if (temp == 0)
                        continue;

                    if (!Verify(arr, x, y, temp))
                    {
                        return false;
                    }
                    arr[y, x].Value = temp;
                }
            }
            return true;
        }

#if DEBUG
        public void PrintBoard(Block[,] board)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Console.Write($"{board[y, x].Value} ");
                }
                Console.WriteLine();
            }
        }
        public void ShowBoard(Block[,] board)
        {
            var sb = new System.Text.StringBuilder();
            int c = 1;

            foreach (var i in board)
            {
                if (c < 9)
                {
                    sb.Append(i.Value.ToString() + "    ");
                }
                else
                {
                    sb.Append(i.Value.ToString() + '\n');
                    c = 0;
                }

                c++;
            }

            MessageBox.Show(sb.ToString());
        }
#endif

        private int GetRandom()
        {
            return Random.Shared.Next(1, 10);
        }
    }
}
