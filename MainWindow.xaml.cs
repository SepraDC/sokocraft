using System;
using System.Collections.Generic;
using System.IO;
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

namespace sokocraft_core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game _game;
        public MainWindow()
        {
            InitializeComponent();
            CnvGame.Visibility = Visibility.Hidden;
            CnvGame.IsEnabled = false;
            GenerateButton();
            DisplayMenu();
        }

         private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
             if (e.Key.Equals(Key.Right) || e.Key.Equals(Key.Left) || e.Key.Equals(Key.Down) || e.Key.Equals(Key.Up))
             {
                 _game.KeyDown(e.Key);
                 Draw();

                 if (_game.GameOver())
                 {
                     MessageBoxResult msg = MessageBox.Show("Bravo vous avez gagné en " + _game.MoveCount + " mouvements\n Recommencer ?", "Recommencer ?", MessageBoxButton.YesNo, MessageBoxImage.Information);
                     if (msg == MessageBoxResult.Yes)
                     {
                         _game.Restart();
                         Draw();
                     }
                     else if (msg == MessageBoxResult.No)
                     {
                         DisplayMenu();
                         _game = null;
                         CnvMobiles.Children.Clear();
                         CnvGrid.Children.Clear();
                     }
                 }
             }

             if(e.Key.Equals(Key.R))
             {
                 _game.Restart();
                 Draw();
             }

             if (e.Key.Equals(Key.Escape))
             {
                 DisplayMenu();
                 _game = null;
                 CnvMobiles.Children.Clear();
                 CnvGrid.Children.Clear();
             }
        }

        private void Draw() {
            CnvMobiles.Children.Clear();
            DrawPlayer();
            DrawCrates();
            DisplayMoveCount();
        }

        private void InitDraw()
        {
            DrawMap();
            DrawCrates();
            DrawPlayer();
            DisplayMoveCount();
        }

        private void DisplayMoveCount()
        {
            TxtMoveCount.Text = _game.MoveCount.ToString();
        }

        private void DrawPlayer()
        {
            Rectangle r = new Rectangle();
            r.Width = 30;
            r.Height = 30;
            r.Margin = new Thickness(_game.Player.Y * 50 + 10, _game.Player.X * 50 + 10, 0, 0);
            r.Fill = new ImageBrush(new BitmapImage(new Uri("./images/player.jpg", UriKind.Relative)));
            CnvMobiles.Children.Add(r);
        }

        private void DrawCrates()
        {
            foreach (Position pos in _game.Crates)
            {
                Rectangle r = new Rectangle();
                r.Width = 42;
                r.Height = 42;
                r.Margin = new Thickness(pos.Y * 50 + 4, pos.X * 50 + 4, 0, 0);
                r.Fill = new ImageBrush(new BitmapImage(new Uri("./images/crate.png", UriKind.Relative)));
                CnvMobiles.Children.Add(r);
            }
        }

         private void DrawMap()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int column = 0; column < 10; column++)
                {
                    Rectangle r = new Rectangle
                    {
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(column * 50, row * 50, 0, 0)
                    };

                    switch (_game.Box(row, column))
                    {
                        case Game.Status.Empty:
                            r.Fill = new ImageBrush(new BitmapImage(new Uri("./images/floor.png", UriKind.Relative)));
                            break;
                        case Game.Status.Wall:
                            r.Fill = new ImageBrush(new BitmapImage(new Uri("./images/wall.png", UriKind.Relative)));
                            break;
                        case Game.Status.Target:
                            r.Fill = new ImageBrush(new BitmapImage(new Uri("./images/target.png", UriKind.Relative)));
                            break;
                    }


                    CnvGrid.Children.Add(r);
                }
            }
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            _game.Restart();
            Draw();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            _game = new Game(btn?.Tag.ToString());
            DisplayGame();
        }

        private void DisplayGame()
        {
            this.KeyDown += MainWindow_KeyDown;
            CnvGame.IsEnabled = true;
            CnvGame.Visibility = Visibility.Visible;
            CnvMenu.IsEnabled = false;
            CnvMenu.Visibility = Visibility.Hidden;
            InitDraw();
        }

        private void DisplayMenu()
        {
            this.KeyDown -= MainWindow_KeyDown;
            CnvGame.Visibility = Visibility.Hidden;
            CnvGame.IsEnabled = false;
            CnvMenu.Visibility = Visibility.Visible;
            CnvMenu.IsEnabled = true;
        }

        private void GenerateButton()
        {
            string path = "./fields/";
            int fileCount = Directory.GetFiles(path, "*.txt", SearchOption.TopDirectoryOnly).Length;
            for (int i = 1; i <= fileCount; i++)
            {
                Button btn = new Button
                {
                    Margin = new Thickness(10, 15, 10, 15),
                    Background = new SolidColorBrush(Color.FromRgb(225, 217, 216)),
                    Foreground = new SolidColorBrush(Color.FromRgb(117, 95, 43)),
                    FontSize = 16,
                    FontFamily = new FontFamily("Impact"),
                    Tag = i,
                    Content = "Level " + i.ToString()
                };
                btn.Click += Button_Click;
                UniGridListLevel.Children.Add(btn);
            }
        }

        private void Square_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayMenu();
            _game = null;
            CnvMobiles.Children.Clear();
            CnvGrid.Children.Clear();
        }
    }
}
