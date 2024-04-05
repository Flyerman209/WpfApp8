using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp8
{
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 20;
        private const int GameAreaSize = 600;
        private const int InitialSnakeLength = 3;
        private const int SnakeSpeed = 200;

        private DispatcherTimer gameTimer;
        private List<Point> snake;
        private Point food;
        private int score;
        private int bestScore;
        private bool isGameStarted = false;

        private Direction direction;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(SnakeSpeed);

            snake = new List<Point>();
            score = 0;
            bestScore = 0;

            StartNewGame();
        }

        private void StartNewGame()
        {
            snake.Clear();
            score = 0;

            for (int i = 0; i < InitialSnakeLength; i++)
            {
                snake.Add(new Point((GameAreaSize / 2) - i * SnakeSquareSize, GameAreaSize / 2));
            }

            food = GenerateFood();

            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            UpdateGameArea();
        }

        private void MoveSnake()
        {
            Point head = snake[0];
            Point newHead = new Point();

            switch (direction)
            {
                case Direction.Up:
                    newHead = new Point(head.X, head.Y - SnakeSquareSize);
                    break;
                case Direction.Down:
                    newHead = new Point(head.X, head.Y + SnakeSquareSize);
                    break;
                case Direction.Left:
                    newHead = new Point(head.X - SnakeSquareSize, head.Y);
                    break;
                case Direction.Right:
                    newHead = new Point(head.X + SnakeSquareSize, head.Y);
                    break;
            }

            if (newHead.X < 0 || newHead.X >= GameAreaSize || newHead.Y < 0 || newHead.Y >= GameAreaSize)
            {
                EndGame();
                return;
            }

            snake.Insert(0, newHead);
            if (snake.Count > score + InitialSnakeLength)
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private void CheckCollision()
        {
            Point head = snake[0];

            if (head.X < 0 || head.X >= GameAreaSize || head.Y < 0 || head.Y >= GameAreaSize)
            {
                EndGame();
                return;
            }

            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[i] == head)
                {
                    EndGame();
                    return;
                }
            }

            if (head == food)
            {
                score++;
                if (score > bestScore)
                {
                    bestScore = score;
                }
                food = GenerateFood();
            }
        }

        private void EndGame()
        {
            gameTimer.Stop();
            GameOverMessage.Visibility = Visibility.Visible;

            string nickname = NicknameTextBox.Text;

            HighScores newScore = new HighScores
            {
                Nickname = nickname,
                Score = score
            };

            using (var context = new SnakeGameDBEntities1())
            {
                context.HighScores.Add(newScore); // Используйте Add, чтобы добавить экземпляр HighScore
                context.SaveChanges();

                var maxScore = context.HighScores.Max(s => s.Score);
                bestScore = maxScore > bestScore ? maxScore : bestScore;
            }

            MessageBox.Show("Game Over! Your score: " + score, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateGameArea()
        {
            GameArea.Children.Clear();

            foreach (Point point in snake)
            {
                Rectangle snakePart = CreateSnakePart(point);
                GameArea.Children.Add(snakePart);
            }

            Rectangle foodRectangle = CreateFoodRectangle(food);
            GameArea.Children.Add(foodRectangle);

            ScoreLabel.Text = "Score: " + score;
            BestScoreLabel.Text = "Best Score: " + bestScore;
        }

        private Rectangle CreateSnakePart(Point position)
        {
            return new Rectangle
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = Brushes.White,
                Margin = new Thickness(position.X, position.Y, 0, 0)
            };
        }

        private Rectangle CreateFoodRectangle(Point position)
        {
            return new Rectangle
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = Brushes.Red,
                Margin = new Thickness(position.X, position.Y, 0, 0)
            };
        }

        private Point GenerateFood()
        {
            Random random = new Random();
            int maxX = GameAreaSize / SnakeSquareSize;
            int maxY = GameAreaSize / SnakeSquareSize;

            int foodX = random.Next(0, maxX) * SnakeSquareSize;
            int foodY = random.Next(0, maxY) * SnakeSquareSize;

            foreach (Point point in snake)
            {
                if (point.X == foodX && point.Y == foodY)
                {
                    return GenerateFood();
                }
            }

            return new Point(foodX, foodY);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isGameStarted)
            {
                if (e.Key == Key.Up && direction != Direction.Down)
                {
                    direction = Direction.Up;
                }
                else if (e.Key == Key.Down && direction != Direction.Up)
                {
                    direction = Direction.Down;
                }
                else if (e.Key == Key.Left && direction != Direction.Right)
                {
                    direction = Direction.Left;
                }
                else if (e.Key == Key.Right && direction != Direction.Left)
                {
                    direction = Direction.Right;
                }
            }
            else
            {
                // Действия по нажатию клавиш во время игры
            }
        }



        private void EndGameButton_Click(object sender, RoutedEventArgs e)
        {
            EndGame();
        }
        private void ShowLeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardWindow leaderboardWindow = new LeaderboardWindow();
            leaderboardWindow.Show();
        }
        


        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
            else
            {
                gameTimer.Start();
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
