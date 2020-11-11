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

        internal struct PointInt
        {
            internal int row;
            internal int col;
        }

        int[,] gameField = new int[GRID_WIDTH,GRID_HEIGHT];
        PointInt pos;
        PointInt eggPos;

        bool eggExists = false;
        private int eggSpawnTimer;

        HashSet<int> blockers;
        List<PointInt> snakeBodyPos = new List<PointInt>();
        List<Border> snakeBodyView = new List<Border>();

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

            pos.row = 8;
            pos.col = 8;

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
            PointInt prevPos = pos;

            gameField[pos.row, pos.col] = EMPTY_CELL;

            switch (snakeDir)
            {
                case Direction.Down:
                    {
                        pos.row += 1;
                        break;
                    }
                case Direction.Up:
                    {
                        pos.row -= 1;
                        break;
                    }
                case Direction.Left:
                    {
                        pos.col -= 1;
                        break;
                    }
                case Direction.Right:
                    {
                        pos.col += 1;
                        break;
                    }
            }

            if((pos.col < 0) || (pos.col >= GRID_WIDTH) ||
                (pos.row < 0) || (pos.row >= GRID_HEIGHT))
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
                        eggPos.row = rnd.Next(0, GRID_WIDTH);
                        eggPos.col = rnd.Next(0, GRID_HEIGHT);

                        if (gameField[eggPos.row, eggPos.col] == EMPTY_CELL)
                        {
                            freeFound = true;
                            break;
                        }

                        repCount += 1;
                    } while (repCount < 3);

                    if (freeFound)
                    {
                        gameField[eggPos.row, eggPos.col] = EGG;
                        Grid.SetRow(egg, eggPos.row);
                        Grid.SetColumn(egg, eggPos.col);
                        egg.Visibility = Visibility.Visible;
                        eggExists = true;
                    }
                }
            }

            //Check collision with egg (to grow snake)
            if((pos.row == eggPos.row)&&(pos.col == eggPos.col))//eat egg and make thyself bigger
            {
                eggExists = false;
                eggSpawnTimer = 10;//ticks
                egg.Visibility = Visibility.Collapsed;

                var newSegment = new Border();
                newSegment.Background = new SolidColorBrush(Color.FromRgb(0,0,0));//black
                grdBackground.Children.Add(newSegment);

                Grid.SetRow(newSegment, prevPos.row);
                Grid.SetColumn(newSegment, prevPos.col);

                snakeBodyView.Add(newSegment);
                snakeBodyPos.Add(prevPos);
            }

            //Check collision with self or walls
            if (blockers.Contains(gameField[pos.row, pos.col]))
            {
                GameOver();
                return;
            }

            gameField[pos.row, pos.col] = SNAKE_HEAD;            
            
            Grid.SetRow(snakeHead, pos.row);
            Grid.SetColumn(snakeHead, pos.col);

            if (snakeBodyPos.Count != 0)//we have body to move
            {
                //Move positions one step further towards head
                for (int i = 0; i < snakeBodyPos.Count - 1; i++)
                {
                    snakeBodyPos[i] = snakeBodyPos[i + 1];
                }
                snakeBodyPos[snakeBodyPos.Count - 1] = prevPos;

                //Show new segment pos
                for (int i = 0; i < snakeBodyPos.Count; i++)
                {
                    var curSegmentPos = snakeBodyPos[i];
                    var curSegmentView = snakeBodyView[i];
                    Grid.SetRow(curSegmentView, curSegmentPos.row);
                    Grid.SetColumn(curSegmentView, curSegmentPos.col);
                }
            }
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
            pos.col = 8; //DEFAULT_POS
            pos.row = 8;
            snakeDir = Direction.Left;
            Grid.SetRow(snakeHead, pos.row);
            Grid.SetColumn(snakeHead, pos.col);
            snakeHead.Visibility = Visibility.Visible;
            labelGameOver.Visibility = Visibility.Collapsed;

            egg.Visibility = Visibility.Collapsed;
            eggExists = false;
            eggSpawnTimer = 0;

            ticker.Start();
        }
    }
}
