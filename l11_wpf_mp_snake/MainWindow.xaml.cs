using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace l11_wpf_mp_snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int GRID_WIDTH = 32;
        const int GRID_HEIGHT = 32;

        const int EMPTY_CELL = 0;
        const int SNAKE_HEAD = 1;
        const int SNAKE_BODY = 2;

        int[,] gameField = new int[GRID_WIDTH,GRID_HEIGHT];
        int pos_row = 8;
        int pos_col = 8;

        enum Direction
        {
            Unknown,
            Left,
            Right,
            Up,
            Down
        }

        Direction snakeDir = Direction.Left;
        private DispatcherTimer ticker;

        public MainWindow()
        {
            InitializeComponent();
            
            ticker = new DispatcherTimer();
            ticker.Interval = new TimeSpan( 33300*30 ); //30fps
            ticker.Tick += UpdateGame;
            ticker.Start();
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            gameField[pos_row, pos_col] = EMPTY_CELL;

            switch (snakeDir)
            {
                case Direction.Down:
                    {
                        pos_row += 1;
                        break;
                    }
                case Direction.Up:
                    {
                        pos_row -= 1;
                        break;
                    }
                case Direction.Left:
                    {
                        pos_col -= 1;
                        break;
                    }
                case Direction.Right:
                    {
                        pos_col += 1;
                        break;
                    }
            }

            if((pos_col < 0) || (pos_col >= GRID_WIDTH) ||
                (pos_row < 0) || (pos_row >= GRID_HEIGHT))
            {
                GameOver();
                return;
            }

            if(gameField[pos_row, pos_col] != EMPTY_CELL)
            {
                GameOver();
                return;
            }

            gameField[pos_row, pos_col] = SNAKE_HEAD;            
            
            Grid.SetRow(snakeHead, pos_row);
            Grid.SetColumn(snakeHead, pos_col);
        }

        private void GameOver()
        {
            ticker.Stop();
            snakeHead.Visibility = Visibility.Hidden;
            labelGameOver.Visibility = Visibility.Visible;
        }

        private void grdBackground_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        snakeDir = Direction.Up;
                        break;
                    }
                case Key.Down:
                    {
                        snakeDir = Direction.Down;
                        break;
                    }
                case Key.Left:
                    {
                        snakeDir = Direction.Left;
                        break;
                    }
                case Key.Right:
                    {
                        snakeDir = Direction.Right;
                        break;
                    }
                case Key.R:
                    {
                        Restart();
                        break;
                    }
            }
        }

        private void Restart()
        {
            pos_col = 8; //DEFAULT_POS
            pos_row = 8;
            snakeDir = Direction.Left;
            Grid.SetRow(snakeHead, pos_row);
            Grid.SetColumn(snakeHead, pos_col);
            snakeHead.Visibility = Visibility.Visible;
            labelGameOver.Visibility = Visibility.Collapsed;
            ticker.Start();
        }
    }
}
