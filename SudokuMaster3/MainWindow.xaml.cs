using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace SudokuMaster3
{
    public class CustomButton : Button
    {
        public bool HasStartValue { get; set; }

        public int Value { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
    }

    public partial class MainWindow
    {
        private readonly int[,] CellValues = new int[10, 10];

        private readonly List<CustomButton> customButtonsList = new List<CustomButton>();

        private string FileContents { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
        }

        private static string RemoveLineFeeds(string input)
        {
            return !string.IsNullOrEmpty(input)
                ? string.Join(string.Empty, Regex.Split(input, @"(?:\r\n|\n|\r|[ ])"))
                : string.Empty;
        }

        private void InitializeBoard()
        {
            // This loads the customButtonsList and sets the initial properties for each cell.
            // The buttons were added in the XAML so we are getting references to them here.
            foreach (int row in Enumerable.Range(1, 9))
            {
                foreach (int col in Enumerable.Range(1, 9))
                {
                    var customButton = GetCustomButtonByName($"cr{col}{row}");
                    customButton.Content = string.Empty;
                    customButton.Background = new SolidColorBrush(Colors.LightYellow);
                }
            }
        }

        private void OpenSavedGame()
        {
            Array.Clear(CellValues, 0, CellValues.Length);

            // This loads the values for each cell from a filed saved on disk. If the value for a cell is greater than 0
            // it is a start value and cannot be changed by the user.
            var contents = FileContents = RemoveLineFeeds(LoadGameFromDisk());
            if (contents.Length != 81) return;
            int counter = 0;
            customButtonsList.Clear();
            customButtonsList.Capacity = 81;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(contents[counter++].ToString());
                    var customButton = GetCustomButtonByName($"cr{col}{row}");
                    if (value > 0)
                    {
                        SetLockedCell(col, row);
                        customButton.Content = $"{value}";
                    }
                    else if (value == 0)
                    {
                        SetMarkupCell(col, row);
                        customButton.Content = string.Empty;
                        customButton.ContextMenu = (ContextMenu)FindResource("contextMenu");
                    }

                    customButton.Row = row;
                    customButton.Column = col;
                    customButton.Value = value;

                    // CellValues array is populated first from file contents
                    CellValues[col, row] = value;
                    customButtonsList.Insert(0, customButton);

                }
            }

            ShowMarkups();

            var nl = Environment.NewLine;
            textBox1.Text += contents.Substring(0, 9) + nl
                                                          + contents.Substring(9, 9) + nl
                                                          + contents.Substring(18, 9) + nl
                                                          + contents.Substring(27, 9) + nl
                                                          + contents.Substring(36, 9) + nl
                                                          + contents.Substring(45, 9) + nl
                                                          + contents.Substring(54, 9) + nl
                                                          + contents.Substring(63, 9) + nl
                                                          + contents.Substring(72, 9) + nl;
        }

        private void ShowMarkups()
        {
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var customButton = GetCustomButtonByName($"cr{col}{row}");
                    if (customButton.HasStartValue)
                    {
                        SetLockedCell(col, row);
                        continue;
                    }
                    SetMarkupCell(col, row);
                    customButton.Content = TransformCandidateValues(FindCandidates(col, row));
                }
            }
        }

        private void Reset()
        {
            // This loads the values for each cell from a filed saved on disk. If the value for a cell is greater than 0
            // it is a start value and cannot be changed by the user.
            var contents = FileContents;
            if (contents.Length != 81) return;
            int counter = 0;
            customButtonsList.Clear();
            customButtonsList.Capacity = 81;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(contents[counter++].ToString());
                    var customButton = GetCustomButtonByName($"cr{col}{row}");
                    if (value > 0)
                    {
                        SetLockedCell(col, row);
                        customButton.Content = $"{value}";
                    }
                    else if (value == 0)
                    {
                        SetMarkupCell(col, row);
                        customButton.Content = string.Empty;
                        customButton.ContextMenu = (ContextMenu)FindResource("contextMenu");
                    }

                    customButton.Row = row;
                    customButton.Column = col;
                    customButton.Value = value;

                    // CellValues array is populated first from file contents
                    CellValues[col, row] = value;
                    customButtonsList.Insert(0, customButton);

                }
            }

            ShowMarkups();
        }

        private static string TransformCandidateValues(string possibleValues)
        {
            if (possibleValues.Length < 9)
            {
                return possibleValues;
            }

            var lf = Environment.NewLine;
            var s = possibleValues;
            var values = string.Format("{0}{1}{2}{1}{3}", s.Substring(0, 3), lf, s.Substring(3, 3), s.Substring(6, 3));
            return values;
        }

        private string FindCandidates(int col, int row)
        {
            var str = "123456789";

            int r;
            int c;
            var space = new string(' ', 1);

            // check by column
            for (r = 1; r <= 9; r++)
            {
                if (CellValues[col, r] != 0)
                {
                    // that means there is a actual value in it
                    str = str.Replace(CellValues[col, r].ToString(), space);
                }
            }

            // check by row
            for (c = 1; c <= 9; c++)
            {
                if (CellValues[c, row] != 0)
                {
                    // that means there is a actual value in it
                    str = str.Replace(CellValues[c, row].ToString(), space);
                }
            }

            // check the boxes
            var startColumn = col - (col - 1) % 3;
            var startRow = row - (row - 1) % 3;
            for (var rr = startRow; rr <= startRow + 2; rr++)
            {
                for (var cc = startColumn; cc <= startColumn + 2; cc++)
                {
                    if (CellValues[cc, rr] != 0)
                    {
                        str = str.Replace(CellValues[cc, rr].ToString(), space);
                    }
                }
            }

            return str;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                if (mi.CommandParameter is ContextMenu cm)
                {
                    if (cm.PlacementTarget is CustomButton customButton)
                    {
                        if (customButton.HasStartValue)
                        {
                            MessageBox.Show("This cell is contains a start value and cannot be changed.");
                            return;
                        }

                        var content = mi.Header.ToString();
                        customButton.Content = content;

                        int col = int.Parse(customButton.Name.Substring(2, 1));
                        int row = int.Parse(customButton.Name.Substring(3, 1));

                        if (content.All(char.IsDigit))
                        {
                            if (IsMoveValid(col, row, int.Parse(content)))
                            {
                                CellValues[col, row] = int.Parse(content);
                                Reset();
                                SetFontOnly(col, row);
                                customButton.Content = content;
                            }
                            else
                            {
                                customButton.Content = string.Empty;
                                textBox1.Text = "Input was invalid.";
                            }
                        }
                        else if (content == "Erase")
                        {
                            CellValues[col, row] = 0;
                        }
                    }

                }
            }
        }

        private static int ConvertToLinear(int col, int row)
        {
            int value = int.Parse($"{col}{row}");
            int returnValue = 0;
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
            }

            return returnValue;
        }

        private void SetLockedCell(int col, int row)
        {
            var customButton = GetCustomButtonByName($"cr{col}{row}");
            customButton.HasStartValue = true;
            customButton.Background = new SolidColorBrush(Colors.LightSteelBlue);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 12;
            customButton.FontWeight = FontWeights.Bold;
        }

        private void SetFontOnly(int col, int row)
        {
            var customButton = GetCustomButtonByName($"cr{col}{row}");
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 12;
            customButton.FontWeight = FontWeights.Bold;
        }

        private void SetMarkupCell(int col, int row)
        {
            var customButton = GetCustomButtonByName($"cr{col}{row}");
            customButton.HasStartValue = false;
            customButton.Background = new SolidColorBrush(Colors.LightYellow);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 8;
            customButton.FontWeight = FontWeights.SemiBold;
        }

        private bool IsMoveValid(int col, int row, int value)
        {

            // scan through columns
            for (int r = 1; r <= 9; r++)
            {
                if (CellValues[col, r] == value) // duplicate
                {
                    return false;
                }
            }

            // scan through rows
            for (int c = 1; c <= 9; c++)
            {
                if (CellValues[c, row] == value)
                {
                    return false;
                }

            }

            // scan through boxes
            var startC = col - (col - 1) % 3;
            var startR = row - (row - 1) % 3;

            for (int rr = 0; rr <= 2; rr++)
            {
                for (int cc = 0; cc <= 2; cc++)
                {
                    if (CellValues[startC + cc, startR + rr] == value) //duplicate
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private CustomButton GetCustomButtonByName(string name)
        {
            var container = PuzzleGrid;
            var customButton = (CustomButton)container.FindName(name);
            return customButton;
        }

        private string LoadGameFromDisk()
        {
            var contents = string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"SDO files (*.sdo)|*.sdo|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,

            };

            if (openFileDialog.ShowDialog() == true)
            {
                Title = openFileDialog.FileName;
                contents = File.ReadAllText(openFileDialog.FileName);
            }

            return contents;
        }

        private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OpenSavedGame();
        }

        private void MenuClearText_OnClick(object sender, RoutedEventArgs e)
        {
            textBox1.Text = string.Empty;
        }

    }
}
