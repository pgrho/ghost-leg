internal static class Program
{
    static int Main(string[] args)
    {
        var rand = new Random();

        var lefts = (args?.Length > 0 ? args : Enumerable.Range(1, 32).Select(e => e.ToString("'@'0"))).OrderBy(e => rand.NextDouble()).ToList();
        var rights = Enumerable.Range(1, lefts.Count).Select(e => e.ToString("D")).OrderBy(e => rand.NextDouble()).ToList();

        var leftMax = lefts.Max(e => e.Length) + 1;
        lefts = lefts.Select(e => e.PadRight(leftMax)).ToList();

        var rightMax = rights.Max(e => e.Length) + 1;
        rights = rights.Select(e => e.PadLeft(rightMax)).ToList();

        var h = rights.Count;

        if (Console.BufferHeight < h)
        {
            Console.WriteLine("Insufficient BufferHeight.");
        }
        Console.WriteLine("Hit any key to start.");
        Console.ReadKey();

        while (Console.BufferHeight < h)
        {
            Console.WriteLine("Insufficient BufferHeight.");
            Console.WriteLine("Hit any key to start.");
            Console.ReadKey();
        }


        Console.CursorVisible = false;
        Console.Clear();

        var w = Console.BufferWidth;
        var length = w - leftMax - rightMax;

        var lines = new bool[length, h - 1];
        for (var x = 0; x < length; x++)
        {
        SET_LADDER:
            var prev = false;
            for (var y = 0; y < h - 1; y++)
            {
                if (rand.NextDouble() < 0.5)
                {
                    if (prev)
                    {
                        goto SET_LADDER;
                    }
                    prev = lines[x, y] = true;
                }
                else
                {
                    prev = lines[x, y] = false;
                }
            }
        }

        for (var y = 0; y < h; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(lefts[y]);

            for (var x = 0; x < length; x++)
            {
                Console.SetCursorPosition(x + leftMax, y);
                Console.Write(
                    y < h - 1 && lines[x, y] ? '┬'
                    : y > 0 && lines[x, y - 1] ? '┴'
                    : '─');
            }

            Console.SetCursorPosition(w - rights[y].Length, y);
            Console.Write(rights[y]);
        }
        Console.ReadKey();

        var wait = (int)Math.Min(250, 15_000.0 / length);
        for (var x = 0; x < length; x++)
        {
            Thread.Sleep(wait);
            for (var y = 0; y < h - 1; y++)
            {
                if (lines[x, y])
                {
                    lefts.Exchange(y);

                    Console.SetCursorPosition(0, y);
                    Console.Write(lefts[y]);

                    Console.SetCursorPosition(0, y + 1);
                    Console.Write(lefts[y + 1]);
                }
            }
            for (var y = 0; y < h; y++)
            {
                Console.SetCursorPosition(x + leftMax, y);
                Console.Write('━');
            }
        }

        for (; ; )
        {
            var changed = false;
            for (var y = 0; y < h - 1; y++)
            {
                if (rights[y].CompareTo(rights[y + 1]) > 0)
                {
                    rights.Exchange(y);
                    lefts.Exchange(y);

                    Console.SetCursorPosition(0, y);
                    Console.Write(lefts[y]);
                    Console.SetCursorPosition(w - rights[y].Length, y);
                    Console.Write(rights[y]);

                    y++;

                    Console.SetCursorPosition(0, y);
                    Console.Write(lefts[y]);
                    Console.SetCursorPosition(w - rights[y].Length, y);
                    Console.Write(rights[y]);

                    changed = true;
                }
            }

            if (!changed)
            {
                break;
            }
            Thread.Sleep(wait);
        }

        Console.ReadKey();
        return 0;
    }

    private static void Exchange<T>(this IList<T> ary, int a, int b = -1)
    {
        if (b < 0)
        {
            b = a + 1;
        }
        var l1 = ary[a];
        var l2 = ary[b];
        ary[b] = l1;
        ary[a] = l2;
    }
}