using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Reflection;

namespace Tic_Tac_Toe
{
    public partial class MainWindow : Window
    {
        private int counter = 0;
        private bool switchPlayer { get; set; } = true;
        private Button[,] buttons = new Button[3, 3];
        bool mainDiag, nextDiag;
        private bool playCp = false;

        public MainWindow()
        {
            InitializeComponent();
            GameMenu();
        }

        private void GameMenu()
        {
            myGrid.Children.Clear();
            myGrid.RowDefinitions.Clear();
            myGrid.ColumnDefinitions.Clear();

            myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            myGrid.Background = Brushes.Black;


            for (int i = 0; i < 5; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }

            Button singel = new Button();                      
            singel.Content = "Singel";                         
            Grid.SetRow(singel, 1);                            
            Grid.SetColumn(singel, 1);                         
            singel.Background = Brushes.Green;                 
            singel.FontFamily = new FontFamily("Arial Black"); 
            singel.FontWeight = FontWeights.Bold;              
            singel.Click += SingleMode;                        
            myGrid.Children.Add(singel);

            Button multi = new Button();
            multi.Content = "Two";
            Grid.SetRow(multi, 3);
            Grid.SetColumn(multi, 1);
            multi.Background = Brushes.Green;
            multi.FontFamily = new FontFamily("Arial Black");
            multi.FontWeight = FontWeights.Bold;
            multi.Click += TwoMode;
            myGrid.Children.Add(multi);
        }

        private void SingleMode(object sender, RoutedEventArgs e)
        {
            playCp = true;
            NewGame();
        }

        private void TwoMode(object sender, RoutedEventArgs e)
        {
            playCp = false;
            NewGame();
        }

        private void NewGame()
        {
            myGrid.Children.Clear();
            myGrid.RowDefinitions.Clear();
            myGrid.ColumnDefinitions.Clear();
            myGrid.Background = Brushes.Cornsilk;

            foreach (var x in myGrid.Children.OfType<Button>())
            {
                if (x.Content != null)
                {
                    x.Content = null;
                }
            }
            counter = 0;
            CreateButtons();
        }

        private void CreateButtons()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Button button = new Button();
                    button.Click += SetSymbols;
                    button.Height = 90;
                    button.Width = 90;
                    button.Background = Brushes.Orange;
                    button.FontSize = 26;
                    button.FontFamily = new FontFamily("Arial Black");
                    button.FontWeight = FontWeights.Bold;
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Margin = new Thickness(col * 100, row * 100, 0, 0);
                    buttons[row, col] = button;
                    myGrid.Children.Add(button);
                }
            }
        }

        private async void SetSymbols(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (!playCp)
            {
                switchPlayer ^= true;
                button.Content = switchPlayer ? "X" : "O";
                button.Foreground = switchPlayer ? Brushes.Cornsilk : Brushes.Black;
                button.Click -= SetSymbols;

                CheckIfSomePlayerWin();
            }

            else if (playCp)
            {
                button.Content = switchPlayer ? "X" : "O";
                button.Foreground = switchPlayer ? Brushes.Cornsilk : Brushes.Black;
                button.Click -= SetSymbols;
               
                CheckIfSomePlayerWin();
                await Task.Delay(1000);
                ComputersMove();
              
            }
           
        }

        private List<Tuple<int, int>> FindAllOs(Button[,] board)
        {
            List<Tuple<int, int>> oPositions = new List<Tuple<int, int>>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j].Content != null && board[i, j].Content.ToString() == "O")
                    {
                        oPositions.Add(Tuple.Create(i, j));
                    }
                }
            }
            return oPositions;
        }

        private List<Tuple<int, int>> FindAllXs(Button[,] board)
        {
            List<Tuple<int, int>> xPositions = new List<Tuple<int, int>>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j].Content != null && board[i, j].Content.ToString() == "X")
                    {
                        xPositions.Add(Tuple.Create(i, j));
                    }
                }
            }
            return xPositions;
        }

        private void ComputersMove()
        {
            List<Tuple<int, int>> oPositions = FindAllOs(buttons);
            List<Tuple<int, int>> xPositions = FindAllXs(buttons);

            if (switchPlayer)
            {
                bool moveMade = false;
                bool canWin = false;
                bool canBlock = false;

                foreach (var position in oPositions)
                {
                    if (CheckRowForWinningMove(buttons, position.Item1, "O"))
                    {
                        PlaceRowMove(position.Item1, buttons, "O");
                        moveMade = true;
                        canWin = true;
                        break;
                    }

                    else if (!CheckRowForWinningMove(buttons, position.Item1, "O") &&
                              CheckColForWinningMove(buttons, position.Item2, "O"))
                    {
                        PlaceColMove(position.Item2, buttons, "O");
                        moveMade = true;
                        canWin = true;
                        break;
                    }

                    else if (!CheckRowForWinningMove(buttons, position.Item1, "O") &&
                             !CheckColForWinningMove(buttons, position.Item2, "O") &&
                              CheckDiagonalsForWinningMove(buttons, "O"))
                    {
                        if (mainDiag)
                        {
                            PlaceMainDiagonalMove(buttons, "O");
                        }
                        else if (nextDiag)
                        {
                            PlaceNextDiagonalMove(buttons, "O");
                        }
                        moveMade = true;
                        canWin = true;
                        break;
                    }
                }

                if (!moveMade && !canWin)
                {
                    foreach (var position in xPositions)
                    {
                        if (CheckRowForWinningMove(buttons, position.Item1, "X"))
                        {
                            PlaceRowMove(position.Item1, buttons, "O");
                            moveMade = true;
                            canBlock = true;
                            break;
                        }

                        else if (!CheckRowForWinningMove(buttons, position.Item1, "X") &&
                                  CheckColForWinningMove(buttons, position.Item2, "X"))
                        {
                            PlaceColMove(position.Item2, buttons, "O");
                            moveMade = true;
                            canBlock = true;
                            break;
                        }

                        else if (!CheckRowForWinningMove(buttons, position.Item1, "X") &&
                                 !CheckColForWinningMove(buttons, position.Item2, "X") &&
                                  CheckDiagonalsForWinningMove(buttons, "X"))
                        {
                            if (mainDiag)
                            {
                                PlaceMainDiagonalMove(buttons, "O");
                            }
                            else if (nextDiag)
                            {
                                PlaceNextDiagonalMove(buttons, "O");
                            }
                            moveMade = true;
                            canBlock = true;
                            break;
                        }
                    }
                }

                if (!moveMade && !canWin && !canBlock)
                {
                    int randomRow, randomCol;
                    do
                    {
                        Random random = new Random();
                        randomRow = random.Next(0, 3);
                        randomCol = random.Next(0, 3);
                    } while (buttons[randomRow, randomCol].Content != null);

                    buttons[randomRow, randomCol].Content = "O";
                    buttons[randomRow, randomCol].Click -= SetSymbols;
                    moveMade = true;
                }

                if (moveMade)
                {
                    switchPlayer = true;
                }
            }
            CheckIfSomePlayerWin();
        }

        private bool CheckRowForWinningMove(Button[,] board, int row, string symbol)
        {
            int countSymbol = 0;
            int countEmpty = 0;

            for (int j = 0; j < 3; j++)
            {
                if (board[row, j].Content == symbol)
                {
                    countSymbol++;
                }
                else if (board[row, j].Content == null)
                {
                    countEmpty++;
                }
            }

            return countSymbol >= 2 && countEmpty >= 1;
        }

        private bool CheckColForWinningMove(Button[,] board, int col, string symbol)
        {
            int countSymbol = 0;
            int countEmpty = 0;

            for (int i = 0; i < 3; i++)
            {
                if (board[i, col].Content == symbol)
                {
                    countSymbol++;
                }
                else if (board[i, col].Content == null)
                {
                    countEmpty++;
                }
            }

            return countSymbol >= 2 && countEmpty >= 1;
        }

        private bool CheckDiagonalsForWinningMove(Button[,] board, string symbol)
        {
            int countMainDiagonalSymbol = 0;
            int countAntiDiagonalSymbol = 0;
            int countMainDiagonalEmpty = 0;
            int countAntiDiagonalEmpty = 0;
            mainDiag = false;
            nextDiag = false;

            for (int i = 0; i < 3; i++)
            {
                if (board[0, 0].Content == symbol || board[2, 2].Content == symbol)
                {
                    mainDiag = true;
                }

                if (board[0, 2].Content == symbol || board[2, 0].Content == symbol)
                {
                    nextDiag = true;
                }

                if (board[i, i].Content == symbol)
                {
                    countMainDiagonalSymbol++;
                }
                else if (board[i, i].Content == null)
                {
                    countMainDiagonalEmpty++;
                }

                if (board[i, 2 - i].Content == symbol)
                {
                    countAntiDiagonalSymbol++;
                }
                else if (board[i, 2 - i].Content == null)
                {
                    countAntiDiagonalEmpty++;
                }
            }

            return (countMainDiagonalSymbol >= 2 && countMainDiagonalEmpty >= 1) ||
                   (countAntiDiagonalSymbol >= 2 && countAntiDiagonalEmpty >= 1);
        }

        private void PlaceColMove(int index, Button[,] board, string symbol)
        {
            if (index >= 0 && index < 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (board[i, index].Content == null)
                    {
                        board[i, index].Content = "O";
                        buttons[i, index].Click -= SetSymbols;
                        return;
                    }
                }
            }
        }

        private void PlaceRowMove(int index, Button[,] board, string symbol)
        {
            if (index >= 0 && index < 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (board[index, i].Content == null)
                    {
                        board[index, i].Content = "O";
                        buttons[index, i].Click -= SetSymbols;
                        return;
                    }
                }
            }
        }

        private void PlaceMainDiagonalMove(Button[,] board, string symbol) 
        {
            if (board[0, 0].Content == null)
            {
                board[0, 0].Content = symbol;
                buttons[0, 0].Click -= SetSymbols;
            }
            else if (board[1, 1].Content == null)
            {
                board[1, 1].Content = symbol;
                buttons[1, 1].Click -= SetSymbols;
            }
            else if (board[2, 2].Content == null)
            {
                board[2, 2].Content = symbol;
                buttons[2, 2].Click -= SetSymbols;
            }
        }

        private void PlaceNextDiagonalMove(Button[,] board, string symbol)
        {
            if (board[0, 2].Content == null)
            {
                board[0, 2].Content = symbol;
                buttons[0, 2].Click -= SetSymbols;
            }
            else if (board[1, 1].Content == null)
            {
                board[1, 1].Content = symbol;
                buttons[1, 1].Click -= SetSymbols;
            }
            else if (board[2, 0].Content == null)
            {
                board[2, 0].Content = symbol;
                buttons[2, 0].Click -= SetSymbols;
            }
        }

        public void CheckIfSomePlayerWin()
        {
       
            for (int i = 0; i < 3; i++)
            {
                if (buttons[0, i].Content != null && buttons[0, i].Content != string.Empty &&
                    buttons[0, i].Content == buttons[1, i].Content && buttons[1, i].Content == buttons[2, i].Content)
                {
                    HighlightWinningButtons(0, i, 1, i, 2, i);
                    ShowEndGameMessage(buttons[0, i].Content.ToString());
                    return;
                }

                if (buttons[i, 0].Content != null && buttons[i, 0].Content != string.Empty &&
                    buttons[i, 0].Content == buttons[i, 1].Content && buttons[i, 1].Content == buttons[i, 2].Content)
                {
                    HighlightWinningButtons(i, 0, i, 1, i, 2);
                    ShowEndGameMessage(buttons[i, 0].Content.ToString());
                    return;
                }
            }

            if (buttons[0, 0].Content != null && buttons[0, 0].Content != string.Empty &&
                buttons[0, 0].Content == buttons[1, 1].Content && buttons[1, 1].Content == buttons[2, 2].Content)
            {
                HighlightWinningButtons(0, 0, 1, 1, 2, 2);
                ShowEndGameMessage(buttons[0, 0].Content.ToString());
                return;
            }

            if (buttons[2, 0].Content != null && buttons[2, 0].Content != string.Empty &&
                buttons[2, 0].Content == buttons[1, 1].Content && buttons[1, 1].Content == buttons[0, 2].Content)
            {
                HighlightWinningButtons(2, 0, 1, 1, 0, 2);
                ShowEndGameMessage(buttons[2, 0].Content.ToString());
                return;
            }

            bool isBoardFull = true;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttons[i, j].Content == null)
                    {
                        isBoardFull = false;
                        break;
                    }
                }
                if (!isBoardFull)
                {
                    break;
                }
            }

            if (isBoardFull)
            {
                MessageBoxResult result = MessageBox.Show($"It's Draw\nNew Game?", "End Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    NewGame();
                }
                else
                {
                    this.Close();
                }
                return;
            }
        }

        private void HighlightWinningButtons(int row1, int col1, int row2, int col2, int row3, int col3)
        {
            buttons[row1, col1].Background = Brushes.Green;
            buttons[row2, col2].Background = Brushes.Green;
            buttons[row3, col3].Background = Brushes.Green;
        }

        private void ShowEndGameMessage(string winner)
        {
            MessageBoxResult result = MessageBox.Show($"{winner} Win\nNew Game?", "End Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NewGame();
            }
            else
            {
                this.Close();
            }
        }
    }
}
