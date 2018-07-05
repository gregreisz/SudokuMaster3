using System;
using System.Collections.Generic;
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
        public bool HasStartValue { get; set; }

        public int Value { get; set; }

        public int Region { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public List<int> Candidates { get; set; }
    }

    public partial class MainWindow
    {
        private readonly List<CustomButton> customButtonList = new List<CustomButton>();

        private const int buttonHeight = 30;

        private const int buttonWidth = 30;

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

        private static bool IsNumeric(object expression)
        {
            bool isNumber = double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            return isNumber;
        }

        private void InitializeBoard()
        {
            foreach (int row in Enumerable.Range(1, 9))
                foreach (int col in Enumerable.Range(1, 9))
                {
                    var customButton = new CustomButton
                    {
                        Height = buttonHeight,
                        Width = buttonWidth,
                        Name = $"cr{col}{row}",
                        Content = $"{col}{row}",
                        Region = GetRegion(col, row),
                        Row = row,
                        Column = col,
                        Value = 0
                    };

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
                            customButton.Region = 1;
                            break;
                        case 41:
                        case 51:
                        case 61:
                        case 42:
                        case 52:
                        case 62:
                        case 43:
                        case 53:
                        case 63:
                            customButton.Region = 2;
                            break;
                        case 71:
                        case 81:
                        case 91:
                        case 72:
                        case 82:
                        case 92:
                        case 73:
                        case 83:
                        case 93:
                            customButton.Region = 3;
                            break;
                        case 14:
                        case 24:
                        case 34:
                        case 15:
                        case 25:
                        case 35:
                        case 16:
                        case 26:
                        case 36:
                            customButton.Region = 4;
                            break;
                        case 44:
                        case 54:
                        case 64:
                        case 45:
                        case 55:
                        case 65:
                        case 46:
                        case 56:
                        case 66:
                            customButton.Region = 5;
                            break;
                        case 74:
                        case 84:
                        case 94:
                        case 75:
                        case 85:
                        case 95:
                        case 76:
                        case 86:
                        case 96:
                            customButton.Region = 6;
                            break;
                        case 17:
                        case 27:
                        case 37:
                        case 18:
                        case 28:
                        case 38:
                        case 19:
                        case 29:
                        case 39:
                            customButton.Region = 7;
                            break;
                        case 47:
                        case 57:
                        case 67:
                        case 48:
                        case 58:
                        case 68:
                        case 49:
                        case 59:
                        case 69:
                            customButton.Region = 8;
                            break;
                        case 77:
                        case 87:
                        case 97:
                        case 78:
                        case 88:
                        case 98:
                        case 79:
                        case 89:
                        case 99:
                            customButton.Region = 9;
                            break;
                        default:
                            customButton.Region = 0;
                            break;
                    }

                    customButton.Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    customButton.Click += Menu_Click;
                    PuzzleGrid.Children.Add(customButton);
                    customButtonList.Add(customButton);
                }
        }

        private static int GetRegion(int col, int row)
        {
            var value = $"{col}{row}";
            int region;

            switch (value)
            {
                case "11":
                case "21":
                case "31":
                case "12":
                case "22":
                case "32":
                case "13":
                case "23":
                case "33":
                    region = 1;
                    break;

                case "41":
                case "51":
                case "61":
                case "42":
                case "52":
                case "62":
                case "43":
                case "53":
                case "63":
                    region = 2;
                    break;

                case "71":
                case "81":
                case "91":
                case "72":
                case "82":
                case "92":
                case "73":
                case "83":
                case "93":
                    region = 3;
                    break;

                case "14":
                case "24":
                case "34":
                case "15":
                case "25":
                case "35":
                case "16":
                case "26":
                case "36":
                    region = 4;
                    break;

                case "44":
                case "54":
                case "64":
                case "45":
                case "55":
                case "65":
                case "46":
                case "56":
                case "66":
                    region = 5;
                    break;

                case "74":
                case "84":
                case "94":
                case "75":
                case "85":
                case "95":
                case "76":
                case "86":
                case "96":
                    region = 6;
                    break;

                case "17":
                case "27":
                case "37":
                case "18":
                case "28":
                case "38":
                case "19":
                case "29":
                case "39":
                    region = 7;
                    break;

                case "47":
                case "57":
                case "67":
                case "48":
                case "58":
                case "68":
                case "49":
                case "59":
                case "69":
                    region = 8;
                    break;

                case "77":
                case "87":
                case "97":
                case "78":
                case "88":
                case "98":
                case "79":
                case "89":
                case "99":
                    region = 9;
                    break;
                default:
                    region = 0;
                    break;
            }

            return region;
        }

        //private void CustomButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is MenuItem mi)
        //    {
        //        if (mi.CommandParameter is ContextMenu cm)
        //        {
        //            if (cm.PlacementTarget is CustomButton button)
        //            {
        //                if (button.HasStartValue)
        //                {
        //                    MessageBox.Show("This cell contains a start value and cannot be changed.");
        //                    return;
        //                }

        //                var content = mi.Header.ToString();
        //                int col = int.Parse(button.Name.Substring(2, 1));
        //                int row = int.Parse(button.Name.Substring(3, 1));
        //                int region = GetRegion(col, row);

        //                if (IsNumeric(content) && IsMoveValid(col, row, region, int.Parse(content)))
        //                {
        //                    var oldString = FileContents;
        //                    var sb = new StringBuilder(oldString);
        //                    var startPosition = ConvertToLinear(col, row);
        //                    sb.Remove(startPosition, 1);
        //                    sb.Insert(startPosition, content);
        //                    var newString = sb.ToString();
        //                    FileContents = newString;
        //                    SetFont(ref button);
        //                    button.Content = content;
        //                    RefreshGrid();
        //                    ShowMarkups();
        //                }
        //                else if (content == "Erase")
        //                {
        //                    button.Value = 0;
        //                    button.Content = string.Empty;
        //                }
        //            }

        //        }
        //    }
        //}

        private void OpenSavedGame()
        {
            // This loads the values for each cell from a filed saved on disk. If the value for a cell is greater than 0
            // it is a start value and cannot be changed by the user.
            var contents = FileContents = RemoveLineFeeds(LoadGameFromDisk());

            if (contents.Length != 81) return;
            int counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(contents[counter++].ToString());
                    var customButton = customButtonList.Single(cb => cb.Name == $"cr{col}{row}");
                    if (customButton == null) return;

                    if (value > 0)
                    {
                        customButton.HasStartValue = true;
                        SetLockedCell(ref customButton);
                        customButton.Value = value;
                        customButton.Content = value.ToString();
                    }
                    else if (value == 0)
                    {
                        customButton.ContextMenu = FindResource("contextMenu1") as ContextMenu;
                        customButton.HasStartValue = false;
                        SetMarkupCell(ref customButton);
                        customButton.Value = 0;
                        TransformCandidateValues(ref customButton);
                    }
                }
            }

        }

        private void ShowMarkups()
        {
            foreach (var customButton in customButtonList.Where(cb => cb.HasStartValue))
            {
                var cb = customButton;
                SetLockedCell(ref cb);
            }

            foreach (var customButton in customButtonList.Where(cb => !cb.HasStartValue))
            {
                var cb = customButton;
                SetMarkupCell(ref cb);
                FindCandidates(ref cb);
            }
        }

        private void RefreshGrid()
        {
            // This loads the values for each cell from the FileContents property instead of directly from the saved file. If the value for a cell is greater than 0
            // it is a start value and cannot be changed by the user.
            int counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(FileContents[counter++].ToString());
                    var customButton = (CustomButton)FindName($"cr{col}{row}");
                    if (customButton == null) return;
                    if (value > 0)
                    {
                        customButton.Content = $"{value}";
                        SetLockedCell(ref customButton);
                    }
                    else if (value == 0)
                    {
                        customButton.Content = value;
                        SetMarkupCell(ref customButton);
                    }

                }
            }
        }

        private static void TransformCandidateValues(ref CustomButton button)
        {
            string lf = Environment.NewLine;
            var candidatesString = string.Empty;

            foreach (var candidate in button.Candidates)
            {
                candidatesString += candidate.ToString();
            }

            button.Content = string.Format("{0}{1}{2}{1}{3}", candidatesString.Substring(0, 3), lf, candidatesString.Substring(3, 3), candidatesString.Substring(6, 3));
        }

        private void FindCandidates(ref CustomButton button)
        {
            var candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // check for candidates by column
            foreach (var customButton in customButtonList.Where(cb => cb.Column >= 1 && cb.Column <= 9))
            {
                customButton.Candidates.Remove(button.Value);
            }

            // check for candidates by row
            foreach (var customButton in customButtonList.Where(cb => cb.Row >= 1 && cb.Row <= 9))
            {
                customButton.Candidates.Remove(button.Value);
            }

            // check for candidates by region
            foreach (var customButton in customButtonList.Where(cb => cb.Region >= 1 && cb.Region <= 9))
            {
                customButton.Candidates.Remove(button.Value);
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                if (mi.CommandParameter is ContextMenu cm)
                {
                    if (cm.PlacementTarget is CustomButton customButton)
                    {
                        if (customButton.HasStartValue)
                        {
                            MessageBox.Show("This cell contains a start clue and cannot be erased.");
                            return;
                        }

                        var content = mi.Header.ToString();
                        int col = int.Parse(customButton.Name.Substring(2, 1));
                        int row = int.Parse(customButton.Name.Substring(3, 1));
                        int region = GetRegion(col, row);

                        if (IsNumeric(content) && IsMoveValid(col, row, region, int.Parse(content)))
                        {
                            var oldString = FileContents;
                            var sb = new StringBuilder(oldString);
                            var startPosition = ConvertToLinear(col, row);
                            sb.Remove(startPosition, 1);
                            sb.Insert(startPosition, content);
                            var newString = sb.ToString();
                            FileContents = newString;
                            customButton.Content = content;
                            RefreshGrid();
                            ShowMarkups();
                            SetFont(ref customButton);
                        }
                        else if (content == "Erase")
                        {
                            customButton.Value = 0;
                        }
                        else
                        {
                            MessageBox.Show("The selected number was invalid.");
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
                default:
                    returnValue = 0;
                    break;
            }

            return returnValue;
        }

        private static void SetLockedCell(ref CustomButton customButton)
        {
            customButton.Background = new SolidColorBrush(Colors.LightSteelBlue);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 12;
            customButton.FontWeight = FontWeights.Bold;
        }

        private static void SetFont(ref CustomButton customButton)
        {
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 12;
            customButton.FontWeight = FontWeights.Bold;
        }

        private static void SetMarkupCell(ref CustomButton customButton)
        {
            customButton.Background = new SolidColorBrush(Colors.LightYellow);
            customButton.Foreground = new SolidColorBrush(Colors.Black);
            customButton.FontFamily = new FontFamily("Consolas");
            customButton.FontSize = 8;
            customButton.FontWeight = FontWeights.SemiBold;
        }

        private bool IsMoveValid(int col, int row, int region, int value)
        {

            // scan through column
            foreach (var customButton in customButtonList.Where(cb => cb.Column == col))
            {
                if (customButton.Value == value) return false;
            }

            // scan through row
            foreach (var customButton in customButtonList.Where(cb => cb.Row == row))
            {
                if (customButton.Value == value) return false;
            }

            // scan through region
            foreach (var customButton in customButtonList.Where(cb => cb.Region == region))
            {
                if (customButton.Value == value) return false;
            }

            return true;

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

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenSavedGame();
        }

        private void MenuClearText_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = string.Empty;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
