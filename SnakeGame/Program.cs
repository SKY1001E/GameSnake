using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using static System.Console;

namespace SnakeGame
{
    class Program
    {
        private const int MapWidth = 30;
        private const int MapHeight = 20;

        private const int ScreenWidth = MapWidth * 3 + 1;
        private const int ScreenHeight = MapHeight * 3 + 1;
        private const ConsoleColor BorderColor = ConsoleColor.Gray;

        private const int FrameMs = 200;

        private const ConsoleColor HeadColor = ConsoleColor.DarkBlue;
        private const ConsoleColor BodyColor = ConsoleColor.Yellow;

        private static readonly Random Random = new Random();
        private const ConsoleColor FoodColor = ConsoleColor.Green;


        [SupportedOSPlatform("windows")]
        static void Main()
        {
            SetWindowSize(ScreenWidth, ScreenHeight);
            SetBufferSize(ScreenWidth, ScreenHeight);
            CursorVisible = false;

            while (true)
            {
                StartGame();
                Thread.Sleep(1000);

                Console.ReadKey();
            }
        }

        // All proces which using in game
        static void StartGame()
        {
            Clear();

            DrawBorder();

            Direction currentMovement = Direction.Right;

            var snake = new Snake(10, 5, HeadColor, BodyColor);

            int score = 0;

            Pixel food = GenFood(snake);
            food.Draw();

            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();
                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= FrameMs)
                {
                    if (currentMovement == oldMovement)
                        currentMovement = ReadMovement(currentMovement);
                }

                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(ReadMovement(currentMovement), true);

                    food = GenFood(snake);
                    food.Draw();

                    score++;
                }
                else
                    snake.Move(ReadMovement(currentMovement));

                if (snake.Head.X == MapWidth - 1
                || snake.Head.X == 0
                || snake.Head.Y == MapHeight - 1
                || snake.Head.Y == 0
                || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                {
                    break;
                }
            }

            snake.Clear();
            SetCursorPosition(ScreenWidth / 3 + 8, ScreenHeight / 2);
            Write("Game Over, Score: {0}", score);
        }

        //Generate Foods
        static Pixel GenFood(Snake snake)
        {
            Pixel food;

            do
            {
                food = new Pixel(Random.Next(1, MapWidth - 2), Random.Next(1, MapHeight - 2), FoodColor);
                return food;
            }
            while (snake.Head.X == food.X && snake.Head.Y == food.Y
                || snake.Body.Any(b => b.X == food.X && b.Y == food.Y));
        }

        //Snake Control
        static Direction ReadMovement(Direction currentDirection)
        {
            if (!KeyAvailable)
                return currentDirection;

            ConsoleKey key = ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                _ => currentDirection
            };

            return currentDirection;
        }

        static void DrawBorder()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                new Pixel(i, 0, BorderColor).Draw();
                new Pixel(i, MapHeight - 1, BorderColor).Draw();
            }

            for (int i = 0; i < MapHeight; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWidth - 1, i, BorderColor).Draw();
            }
        }
    }
}
