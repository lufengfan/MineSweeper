using MineSweeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSLauncher.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.NewGame();
        }

        private void NewGame(int row = 20, int column = 25)
        {
            GameMain game = new GameMain(row, column);
            this.initializeGameGrid(game);
        }

        private void initializeGameGrid(GameMain game)
        {
            this.gGame.Children.Clear();
            this.gGame.RowDefinitions.Clear();
            this.gGame.ColumnDefinitions.Clear();

            double length = 25;
            if ((this.gGame.Width / this.gGame.Height) > ((double)game.Row / (double)game.Column))
            { // 长大于宽。
            }
            else
            { // 宽大于长。
            }

            for (int row = 0; row < game.Row; row++) this.gGame.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            for (int column = 0; column < game.Column; column++) this.gGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            
            for (int row = 0; row < game.Row; row++)
                for (int column = 0; column < game.Column; column++)
                {
                    Position position = new Position(row, column);
                    Position currentPosition = Position.Empty;
                    MouseButtonEventHandler buttonDown = (sender, e) =>
                    {
                        if (sender is FrameworkElement fe && fe.Tag is Position _position)
                            currentPosition = _position;
                    };
                    MouseButtonEventHandler leftButtonUp = (sender, e) =>
                    {
                        if (sender is FrameworkElement fe && fe.Tag is Position _position && _position == currentPosition)
                        {
                            currentPosition = Position.Empty;

                            if (e.RightButton == MouseButtonState.Pressed)
                            {
                                if (game.mask[_position.Row, _position.Column] == true)
                                    game.Dig(_position.Row, _position.Column, true);
                            }
                            else
                            {
                                if (game.mask[_position.Row, _position.Column] == null)
                                    game.Dig(_position.Row, _position.Column, false);
                            }

                            this.refreshMask(game);
                        }
                    };
                    MouseButtonEventHandler rightButtonUp = (sender, e) =>
                    {
                        if (sender is FrameworkElement fe && fe.Tag is Position _position && _position == currentPosition)
                        {
                            currentPosition = Position.Empty;

                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                if (game.mask[_position.Row, _position.Column] == true)
                                    game.Dig(_position.Row, _position.Column, true);
                            }
                            else
                            {
                                if (game.mask[_position.Row, _position.Column] != true)
                                    game.Flag(_position.Row, _position.Column);
                            }

                            this.refreshMask(game);
                        }
                    };

                    Label label = new Label();
                    label.BorderBrush = new SolidColorBrush(Colors.LightGray);
                    label.BorderThickness = new Thickness(0.5);
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;
                    int content = game.maps[row, column];
                    if (content != 0)
                    {
                        Run run;
                        if (content == -1)
                        {
                            run = new Run("*");
                        }
                        else
                        {
                            run = new Run(content.ToString());
                            switch (game.maps[row, column])
                            {
                                case 1: run.Foreground = new SolidColorBrush(Colors.Blue); break;
                                case 2: run.Foreground = new SolidColorBrush(Colors.Green); break;
                                case 3: run.Foreground = new SolidColorBrush(Colors.OrangeRed); break;
                                case 4: run.Foreground = new SolidColorBrush(Colors.Red); break;
                                case 5: run.Foreground = new SolidColorBrush(Colors.Brown); break;
                                case 6: run.Foreground = new SolidColorBrush(Colors.HotPink); break;
                                case 7: run.Foreground = new SolidColorBrush(Colors.MediumPurple); break;
                                case 8: run.Foreground = new SolidColorBrush(Colors.DarkBlue); break;
                            }
                        }

                        label.Content = run;
                    }
                    label.SetValue(Grid.RowProperty, row);
                    label.SetValue(Grid.ColumnProperty, column);
                    label.Width = label.Width = length;
                    label.Tag = position;
                    label.MouseLeftButtonDown += buttonDown;
                    label.MouseLeftButtonUp += leftButtonUp;
                    label.MouseRightButtonDown += buttonDown;
                    label.MouseRightButtonUp += rightButtonUp;
                    this.gGame.Children.Add(label);

                    Button button = new Button();
                    button.SetValue(Grid.RowProperty, row);
                    button.SetValue(Grid.ColumnProperty, column);
                    button.Width = button.Height = length;
                    button.Tag = position;
                    button.AddHandler(Button.MouseLeftButtonDownEvent, buttonDown, true);
                    button.AddHandler(Button.MouseLeftButtonUpEvent, leftButtonUp, true);
                    button.MouseRightButtonDown += buttonDown;
                    button.MouseRightButtonUp += rightButtonUp;

                    this.gGame.Children.Add(button);
                }
        }

        private void refreshMask(GameMain game)
        {
            foreach (var button in this.gGame.Children.OfType<Button>().Where(b=>b.Visibility == Visibility.Visible))
            {
                Position position = (Position)button.Tag;
                switch (game.mask[position.Row, position.Column])
                {
                    case true:
                        button.Visibility = Visibility.Hidden;
                        break;
                    case false:
                        button.Visibility = Visibility.Visible;
                        button.Content = "F";
                        break;
                    case null:
                        button.Visibility = Visibility.Visible;
                        button.Content = null;
                        break;
                }
            }

            if (game.IsGameOver)
            {
                foreach (var label in this.gGame.Children.OfType<Label>())
                {
                    Position position = (Position)label.Tag;
                    if (game.maps[position.Row, position.Column] == -1)
                        label.Background = new SolidColorBrush(Colors.LightPink);
                }
            }
        }

        private struct Position : IEquatable<Position>
        {
            public int Row;
            public int Column;

            public static Position Empty { get => new Position() { Row = -1, Column = -1 }; }

            public Position(int row, int column)
            {
                if (row < 0) throw new ArgumentOutOfRangeException(nameof(row), row, $"{nameof(row)} 小于零。");
                if (column < 0) throw new ArgumentOutOfRangeException(nameof(column), column, $"{nameof(column)} 小于零。");

                this.Row = row;
                this.Column = column;
            }

            public static bool operator ==(Position left, Position right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Position left, Position right)
            {
                return !(left == right);
            }


            public override bool Equals(object obj)
            {
                return obj != null && obj is Position position && this.Equals(position);
            }

            public bool Equals(Position other)
            {
                return this.Row == other.Row && this.Column == other.Column;
            }
        }

        private void miNewGame_Click(object sender, RoutedEventArgs e)
        {
            this.NewGame();
        }
    }
}
