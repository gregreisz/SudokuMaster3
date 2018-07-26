using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace SudokuMaster3
{
    public class CustomButton : Button
    {
        public bool? IsGiven { get; set; }

        public int Value { get; set; }

        public int Region { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public List<int> Candidates { get; set; }
    }

    public partial class MainWindow
    {
        private int LastSelectedValue { get; set; }
        private readonly List<CustomButton> _customButtonList = new List<CustomButton>();
        private const int ButtonHeight = 30;
        private const int ButtonWidth = 30;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            FileContents = string.Empty;
        }

        private string FileContents { get; set; }

        private static string RemoveNewLines(string input)
        {
            return !string.IsNullOrEmpty(input)
                ? string.Join(string.Empty, Regex.Split(input, @"(?:\r\n|\n|\r|[ ])"))
                : string.Empty;
        }

        private static bool IsNumeric(object expression)
        {
            var isNumber = double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            return isNumber;
        }

        private void InitializeBoard()
        {
            // InitializeBoard sets up the property values for the custom button controls and
            // adds them as children of the PuzzleGrid and also as items in customButtonList.

            foreach (var row in Enumerable.Range(1, 9))
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var border = new Border
                    {
                        BorderThickness = new Thickness(1)
                    };
                    var customButton = new CustomButton
                    {
                        Height = ButtonHeight,
                        Width = ButtonWidth,
                        Name = $"cr{col}{row}",
                        Row = row,
                        Column = col,
                        Value = 0,
                        Region = GetRegion(col, row),
                        Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                        Margin = new Thickness(0)
                    };
                    customButton.Click += Cell_Click;
                    PuzzleGrid.Children.Add(border);
                    border.Child = customButton;
                    _customButtonList.Add(customButton);
                }
        }

        private static int GetRegion(int col, int row)
        {
            switch (int.Parse($"{col}{row}"))
            {
                case 11:
                case 21:
                case 31:
                case 12:
                case 22:
                case 32:
                case 13:
                case 23:
                case 33:
                    return 1;
                case 41:
                case 51:
                case 61:
                case 42:
                case 52:
                case 62:
                case 43:
                case 53:
                case 63:
                    return 2;
                case 71:
                case 81:
                case 91:
                case 72:
                case 82:
                case 92:
                case 73:
                case 83:
                case 93:
                    return 3;
                case 14:
                case 24:
                case 34:
                case 15:
                case 25:
                case 35:
                case 16:
                case 26:
                case 36:
                    return 4;
                case 44:
                case 54:
                case 64:
                case 45:
                case 55:
                case 65:
                case 46:
                case 56:
                case 66:
                    return 5;
                case 74:
                case 84:
                case 94:
                case 75:
                case 85:
                case 95:
                case 76:
                case 86:
                case 96:
                    return 6;
                case 17:
                case 27:
                case 37:
                case 18:
                case 28:
                case 38:
                case 19:
                case 29:
                case 39:
                    return 7;
                case 47:
                case 57:
                case 67:
                case 48:
                case 58:
                case 68:
                case 49:
                case 59:
                case 69:
                    return 8;
                case 77:
                case 87:
                case 97:
                case 78:
                case 88:
                case 98:
                case 79:
                case 89:
                case 99:
                    return 9;
                default:
                    return 0;
            }
        }

        private void LoadSavedGame()
        {
            // OpenSavedGame loads the values for each cell from a filed saved on disk.
            if (FileContents == string.Empty)
            {
                FileContents = RemoveNewLines(LoadGameFromDisk()).Replace("X", "0");
                if (FileContents.Length != 81)
                {
                    MessageBox.Show(@"Missing file or incorrect file format.");
                    return;
                }
            }

            // This reads in the file contents.
            var counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(FileContents[counter++].ToString());
                    var button = _customButtonList.FirstOrDefault(cb => cb.Name == $"cr{col}{row}");
                    if (button == null) return;

                    // If the value for a cell is greater than 0 it is a given and cannot be changed by the user.
                    if (value > 0 && button.IsGiven == null)
                    {
                        SetAsLockedCell(ref button);
                        button.Value = value;
                        button.Content = value.ToString();
                    }
                    if (value == 0)
                    {
                        SetAsMarkupCell(ref button);
                        button.Value = value;
                        button.ContextMenu = FindResource("ContextMenu1") as ContextMenu;
                        UpdateCandidates(ref button);
                    }

                }
            }


        }

        private void UpdateCandidates(ref CustomButton button)
        {
            if (button.IsGiven == true) return;

            var col = button.Column;
            var row = button.Row;
            var region = button.Region;

            foreach (var cb in _customButtonList.Where(cb => cb.Column == col && cb.Value > 0))
            {
                button.Candidates.Remove(cb.Value);
            }
            foreach (var cb in _customButtonList.Where(cb => cb.Row == row && cb.Value > 0))
            {
                button.Candidates.Remove(cb.Value);
            }
            foreach (var cb in _customButtonList.Where(cb => cb.Region == region && cb.Value > 0))
            {
                button.Candidates.Remove(cb.Value);
            }

            var candidates = string.Empty;
            foreach (var number in Enumerable.Range(1, 9))
            {
                if (button.Candidates.Contains(number))
                {
                    candidates += $"{number}";
                }
                else
                {
                    candidates += " ";
                }
            }

            var nl = Environment.NewLine;
            button.Content = string.Format("{0}{1}{2}{1}{3}", candidates.Substring(0, 3), nl, candidates.Substring(3, 3), candidates.Substring(6, 3));
        }

        //private void DisplayCandidates(ref CustomButton button)
        //{
        //    var candidates = string.Empty;
        //    foreach (var cb in _customButtonList.Where(cb => !cb.IsGiven))
        //    {
        //        foreach (var number in Enumerable.Range(1, 9))
        //        {
        //            if (cb.Candidates.Contains(number))
        //            {
        //                candidates += $"{number}";
        //            }
        //            else
        //            {
        //                candidates += " ";
        //            }
        //        }
        //    }

        //    var nl = Environment.NewLine;
        //    button.Content = string.Format("{0}{1}{2}{1}{3}", candidates.Substring(0, 3), nl, candidates.Substring(3, 3), candidates.Substring(6, 3));
        //}


        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem mi)) return;
            if (!(mi.CommandParameter is ContextMenu cm)) return;
            if (!(cm.PlacementTarget is CustomButton button)) return;
            if (button.IsGiven == true) return;

            var content = mi.Header.ToString();
            if (content == "Erase")
            {
                button.Value = 0;
                button.Content = string.Empty;
                return;
            }

            var col = int.Parse(button.Name.Substring(2, 1));
            var row = int.Parse(button.Name.Substring(3, 1));
            var region = GetRegion(col, row);
            var oldString = FileContents.Replace("X", "0");
            if (oldString.Length != 81)
            {
                MessageBox.Show("Input string format was not valid.");
                return;
            }
            var sb = new StringBuilder(oldString);
            var startPosition = GetPositionInString(col, row);
            sb.Remove(startPosition, 1);
            sb.Insert(startPosition, content);
            var newString = sb.ToString();
            FileContents = newString;
            LastSelectedValue = int.Parse(content);

            if (IsMoveValid(col, row, region))
            {
                button.Value = LastSelectedValue;
                UpdateCandidates(ref button);
                button.Content = LastSelectedValue;
                LoadSavedGame();
            }
            else
            {
                MessageBox.Show("This move is not valid.");
            }
        }

        private static int GetPositionInString(int col, int row)
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

        private static void SetAsLockedCell(ref CustomButton customButton)
        {
            customButton.Background = new SolidColorBrush(Colors.LightSteelBlue);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 12;
            customButton.FontWeight = FontWeights.Bold;
        }

        private static void SetAsMarkupCell(ref CustomButton customButton)
        {
            customButton.Background = new SolidColorBrush(Colors.LightYellow);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 7.25;
            customButton.FontWeight = FontWeights.Bold;
        }

        private static void SetAsNewValueCell(ref CustomButton customButton)
        {
            customButton.Background = new SolidColorBrush(Colors.LightYellow);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 12;
            customButton.FontWeight = FontWeights.Bold;
        }

        private bool IsMoveValid(int col, int row, int region)
        {
            // scan through column
            foreach (var customButton in _customButtonList.Where(cb => cb.Column == col && cb.Value != 0))
            {
                if (customButton.Value == LastSelectedValue) return false;
            }

            // scan through row
            foreach (var customButton in _customButtonList.Where(cb => cb.Row == row && cb.Value != 0))
            {
                if (customButton.Value == LastSelectedValue) return false;
            }

            // scan through region
            foreach (var customButton in _customButtonList.Where(cb => cb.Region == region && cb.Value != 0))
            {
                if (customButton.Value == LastSelectedValue) return false;
            }

            return true;
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadSavedGame();
        }

        private string LoadGameFromDisk()
        {
            var contents = string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"SS files (*.ss)|*.ss|SDO files (*.sdo)|*.sdo|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,

            };

            if (openFileDialog.ShowDialog() == true)
            {
                Title = openFileDialog.FileName.Substring(23);
                contents = File.ReadAllText(openFileDialog.FileName);
            }
            contents = contents.Replace(".", "0").Replace("|", "").Replace("\r\n", "").Replace("-", "");
            return contents.Replace(".", "0").Replace("|", "").Replace("\r\n", "");
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
