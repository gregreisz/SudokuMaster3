using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace SudokuMaster3
{
    public partial class MainWindow : INotifyPropertyChanged
    {

        public MainWindow()
        {
            InitializeComponent();
            InitializePuzzleBoard();
        }

        public string GameState { get; set; } = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        private void ShowMessage(string input)
        {
            TextBlock1.Text = string.Empty;
            TextBlock1.Inlines.Add(new Run(input));
            TextBlock1.Inlines.Add(new LineBreak());
        }

        private List<TextBlock> _textBlocks = new List<TextBlock>();

        private Button SelectedButton { get; set; }

        private int SelectedValue { get; set; }

        private void InitializePuzzleBoard()
        {
            _textBlocks.Clear();
            foreach (var control in PuzzleGrid.GetChildren())
            {
                if (control is TextBlock textBlock)
                {
                    if (textBlock.Name.Substring(0, 2) != "Cr") continue;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.Text = textBlock.Name;
                    _textBlocks.Add(textBlock);
                }
            }

        }

        private void LoadSavedGame(bool isReload = false)
        {
            // LoadGameFromDisk loads the values for each cell from a filed saved on disk.
            if (GameState == string.Empty)
            {
                GameState = Tools.RemoveNewLines(LoadGameFromDisk());
                Trace.WriteLine(GameState);
                if (GameState.Length != 81)
                {
                    throw new Exception(@"Missing file or incorrect file format.");
                }
            }

            // This reads in the file contents.
            var counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(GameState[counter++].ToString());
                    var button = (Button)FindName($"Cr{col}{row}");
                    if (button == null) throw new Exception("Button control not found.");

                    if (!isReload && value > 0)
                    {
                        SudokuPuzzle.SetValue(button, value);
                        SudokuPuzzle.SetIsGiven(button, true);
                        button.Content = value.ToString();
                        button.IsEnabled = false;
                        SetAsGivenCell(ref button);
                    }
                    else if (!isReload && value == 0)
                    {
                        SudokuPuzzle.SetValue(button, value);
                        SudokuPuzzle.SetIsGiven(button, false);
                        button.ContextMenu = FindResource("ContextMenu1") as ContextMenu;
                        SetCandidates(ref button);
                        SetAsMarkupCell(ref button);
                    }
                    else if (isReload && value > 0 && button.IsEnabled)
                    {
                        button.Content = value.ToString();
                        SetAsUserCell(ref button);
                        SetCandidates(ref button);
                    }
                }
            }
        }

        private void SetCandidates(ref Button button)
        {
            var candidates = SudokuPuzzle.GetCandidates(button);
            candidates = candidates.Replace(SelectedValue.ToString(), " ");
            SudokuPuzzle.SetCandidates(button, candidates);
            button.Content = Tools.FormatCandidates(candidates);
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem mi))
            {
                return;
            }

            if (!(mi.CommandParameter is ContextMenu cm)) return;
            if (!(cm.PlacementTarget is Button button)) return;

            if (mi.Header.ToString() == @"Erase")
            {
                button.Content = string.Empty;
                return;
            }

            SelectedButton = button;
            SelectedValue = int.Parse(mi.Header.ToString().Substring(5));
            var col = SudokuPuzzle.GetColumn(button);
            var row = SudokuPuzzle.GetRow(button);

            if (GameState == string.Empty)
            {
                GameState = LoadGameFromDisk();
            }

            if (GameState.Length != 81)
            {
                throw new Exception("Input string format was not valid!");
            }

            var oldString = GameState;
            Trace.WriteLine($"GameState = {oldString}");

            var sb = new StringBuilder(oldString);
            var startPosition = GetPositionInFileContents(col, row);
            sb.Remove(startPosition, 1);
            sb.Insert(startPosition, SelectedValue);
            var newString = sb.ToString();
            GameState = newString;
            Trace.WriteLine($"GameState = {newString}");
            OnPropertyRaised(GameState);
            if (IsMoveValid())
            {
                button.Content = SelectedValue;
                SetCandidates(ref button);
                SetAsUserCell(ref button);
                LoadSavedGame(true);
            }
            else
            {
                ShowMessage("Invalid move!");
            }
        }

        private static int GetPositionInFileContents(int col, int row)
        {
            var value = int.Parse($"{col}{row}");
            int returnValue;
            switch (value)
            {
                case 11:
                    returnValue = 0;
                    break;
                case 21:
                    returnValue = 1;
                    break;
                case 31:
                    returnValue = 2;
                    break;
                case 41:
                    returnValue = 3;
                    break;
                case 51:
                    returnValue = 4;
                    break;
                case 61:
                    returnValue = 5;
                    break;
                case 71:
                    returnValue = 6;
                    break;
                case 81:
                    returnValue = 7;
                    break;
                case 91:
                    returnValue = 8;
                    break;
                case 12:
                    returnValue = 9;
                    break;
                case 22:
                    returnValue = 10;
                    break;
                case 32:
                    returnValue = 11;
                    break;
                case 42:
                    returnValue = 12;
                    break;
                case 52:
                    returnValue = 13;
                    break;
                case 62:
                    returnValue = 14;
                    break;
                case 72:
                    returnValue = 15;
                    break;
                case 82:
                    returnValue = 16;
                    break;
                case 92:
                    returnValue = 17;
                    break;
                case 13:
                    returnValue = 18;
                    break;
                case 23:
                    returnValue = 19;
                    break;
                case 33:
                    returnValue = 20;
                    break;
                case 43:
                    returnValue = 21;
                    break;
                case 53:
                    returnValue = 22;
                    break;
                case 63:
                    returnValue = 23;
                    break;
                case 73:
                    returnValue = 24;
                    break;
                case 83:
                    returnValue = 25;
                    break;
                case 93:
                    returnValue = 26;
                    break;
                case 14:
                    returnValue = 27;
                    break;
                case 24:
                    returnValue = 28;
                    break;
                case 34:
                    returnValue = 29;
                    break;
                case 44:
                    returnValue = 30;
                    break;
                case 54:
                    returnValue = 31;
                    break;
                case 64:
                    returnValue = 32;
                    break;
                case 74:
                    returnValue = 33;
                    break;
                case 84:
                    returnValue = 34;
                    break;
                case 94:
                    returnValue = 35;
                    break;
                case 15:
                    returnValue = 36;
                    break;
                case 25:
                    returnValue = 37;
                    break;
                case 35:
                    returnValue = 38;
                    break;
                case 45:
                    returnValue = 39;
                    break;
                case 55:
                    returnValue = 40;
                    break;
                case 65:
                    returnValue = 41;
                    break;
                case 75:
                    returnValue = 42;
                    break;
                case 85:
                    returnValue = 43;
                    break;
                case 95:
                    returnValue = 44;
                    break;
                case 16:
                    returnValue = 45;
                    break;
                case 26:
                    returnValue = 46;
                    break;
                case 36:
                    returnValue = 47;
                    break;
                case 46:
                    returnValue = 48;
                    break;
                case 56:
                    returnValue = 49;
                    break;
                case 66:
                    returnValue = 50;
                    break;
                case 76:
                    returnValue = 51;
                    break;
                case 86:
                    returnValue = 52;
                    break;
                case 96:
                    returnValue = 53;
                    break;
                case 17:
                    returnValue = 54;
                    break;
                case 27:
                    returnValue = 55;
                    break;
                case 37:
                    returnValue = 56;
                    break;
                case 47:
                    returnValue = 57;
                    break;
                case 57:
                    returnValue = 58;
                    break;
                case 67:
                    returnValue = 59;
                    break;
                case 77:
                    returnValue = 60;
                    break;
                case 87:
                    returnValue = 61;
                    break;
                case 97:
                    returnValue = 62;
                    break;
                case 18:
                    returnValue = 63;
                    break;
                case 28:
                    returnValue = 64;
                    break;
                case 38:
                    returnValue = 65;
                    break;
                case 48:
                    returnValue = 66;
                    break;
                case 58:
                    returnValue = 67;
                    break;
                case 68:
                    returnValue = 68;
                    break;
                case 78:
                    returnValue = 69;
                    break;
                case 88:
                    returnValue = 70;
                    break;
                case 98:
                    returnValue = 71;
                    break;
                case 19:
                    returnValue = 72;
                    break;
                case 29:
                    returnValue = 73;
                    break;
                case 39:
                    returnValue = 74;
                    break;
                case 49:
                    returnValue = 75;
                    break;
                case 59:
                    returnValue = 76;
                    break;
                case 69:
                    returnValue = 77;
                    break;
                case 79:
                    returnValue = 78;
                    break;
                case 89:
                    returnValue = 79;
                    break;
                case 99:
                    returnValue = 80;
                    break;
                default:
                    returnValue = 0;
                    break;
            }

            return returnValue;
        }

        private static void SetAsGivenCell(ref Button button)
        {
            button.Background = new SolidColorBrush(Colors.LightSteelBlue);
            button.Foreground = new SolidColorBrush(Colors.Black);
            button.FontSize = 12;
        }

        private static void SetAsUserCell(ref Button button)
        {
            button.Background = new SolidColorBrush(Colors.LightYellow);
            button.Foreground = new SolidColorBrush(Colors.Black);
            button.FontSize = 12;
        }

        private static void SetAsMarkupCell(ref Button button)
        {
            button.Background = new SolidColorBrush(Colors.LightYellow);
            button.Foreground = new SolidColorBrush(Colors.Black);
            button.FontSize = 8;
        }

        private bool IsMoveValid()
        {

            foreach (var button in _buttons.Where(button => SudokuPuzzle.GetColumn(button) == SudokuPuzzle.GetColumn(SelectedButton)))
            {
                if (SudokuPuzzle.GetValue(button) == SelectedValue) return false;
            }
            foreach (var button in _buttons.Where(button => SudokuPuzzle.GetRow(button) == SudokuPuzzle.GetRow(SelectedButton)))
            {
                if (SudokuPuzzle.GetValue(button) == SelectedValue) return false;
            }
            foreach (var button in _buttons.Where(button => SudokuPuzzle.GetRegion(button) == SudokuPuzzle.GetRegion(SelectedButton)))
            {
                if (SudokuPuzzle.GetValue(button) == SelectedValue) return false;
            }

            TextBlock1.Text = string.Empty;
            return true;
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadSavedGame();
        }

        private string LoadGameFromDisk()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"SS files (*easy*.ss)|*easy*.ss|SDO files (*.sdo)|*.sdo|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,

            };

            if (openFileDialog.ShowDialog() != true) return string.Empty;
            Title = $@"Sudoku Master 3 - {openFileDialog.FileName}";
            return File.ReadAllText(openFileDialog.FileName).Replace(@".", "0").Replace(@"|", "").Replace(@"\r\n", "").Replace(@"-", "").Replace("X", "0");
        }

        private void MenuClearText_Click(object sender, RoutedEventArgs e)
        {
            TextBlock1.Text = string.Empty;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
