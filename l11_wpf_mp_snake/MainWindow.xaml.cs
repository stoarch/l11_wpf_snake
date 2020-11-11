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
        const int WALL = 3;
        const int EGG = 4;


        int[,] gameField = new int[GRID_WIDTH,GRID_HEIGHT];
        int posRow = 8;
        int posCol = 8;

        int eggRow;
        int eggCol;
        bool eggExists = false;
        private int eggSpawnTimer;

        HashSet<int> blockers;
        List<>

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
        private Random rnd;

        public MainWindow()
        {
            InitializeComponent();

            rnd = new Random();

            blockers = new HashSet<int>();
            blockers.Add(SNAKE_BODY);
            blockers.Add(WALL);
            
            ticker = new DispatcherTimer();
            ticker.Interval = new TimeSpan( 33300*30 ); //30fps
            ticker.Tick += UpdateGame;
            ticker.Start();

        }

        private void UpdateGame(object sender, EventArgs e)
        {
            gameField[posRow, posCol] = EMPTY_CELL;

            switch (snakeDir)
            {
                case Direction.Down:
                    {
                        posRow += 1;
                        break;
                    }
                case Direction.Up:
                    {
                        posRow -= 1;
                        break;
                    }
                case Direction.Left:
                    {
                        posCol -= 1;
                        break;
                    }
                case Direction.Right:
                    {
                        posCol += 1;
                        break;
                    }
            }

            if((posCol < 0) || (posCol >= GRID_WIDTH) ||
                (posRow < 0) || (posRow >= GRID_HEIGHT))
            {
                GameOver();
                return;
            }

            //Spawn egg if not found
            if (!eggExists)
            {
                if(eggSpawnTimer > 0)
                {
                    eggSpawnTimer -= 1;
                }

                if (eggSpawnTimer == 0)
                {
                    int repCount = 0;
                    bool freeFound = true;

                    do
                    {
                        eggRow = rnd.Next(0, GRID_WIDTH);
                        eggCol = rnd.Next(0, GRID_HEIGHT);

                        if (gameField[eggRow, eggCol] == EMPTY_CELL)
                        {
                            freeFound = true;
                            break;
                        }

                        repCount += 1;
                    } while (repCount < 3);

                    if (freeFound)
                    {
                        gameField[eggRow, eggCol] = EGG;
                        Grid.SetRow(egg, eggRow);
                        Grid.SetColumn(egg, eggCol);
                        egg.Visibility = Visibility.Visible;
                        eggExists = true;
                    }
                }
            }

            //Check collision with egg (to grow snake)
            if((posRow == eggRow)&&(posCol == eggCol))//eat egg and make thyself bigger
            {
                eggExists = false;
                eggSpawnTimer = 10;//ticks
                //TODO: Grow snake
                egg.Visibility = Visibility.Collapsed;
            }

            //Check collision with self or walls
            if (blockers.Contains(gameField[posRow, posCol]))
            {
                GameOver();
                return;
            }

            gameField[posRow, posCol] = SNAKE_HEAD;            
            
            Grid.SetRow(snakeHead, posRow);
            Grid.SetColumn(snakeHead, posCol);
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
            posCol = 8; //DEFAULT_POS
            posRow = 8;
            snakeDir = Direction.Left;
            Grid.SetRow(snakeHead, posRow);
            Grid.SetColumn(snakeHead, posCol);
            snakeHead.Visibility = Visibility.Visible;
            labelGameOver.Visibility = Visibility.Collapsed;

            egg.Visibility = Visibility.Collapsed;
            eggExists = false;
            eggSpawnTimer = 0;

            ticker.Start();
        }
    }
}
