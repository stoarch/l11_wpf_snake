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

        public MainWindow()
        {
            InitializeComponent();
            
            var ticker = new DispatcherTimer();
            ticker.Interval = new TimeSpan( 33300*30 ); //30fps
            ticker.Tick += UpdateGame;
            ticker.Start();
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            pos_row -= 1; //up

            Grid.SetRow(snakeHead, pos_row);
        }
    }
}
