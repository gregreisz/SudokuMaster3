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

        private string fileContents;


        private const int buttonHeight = 30;

        private const int buttonWidth = 30;


        private int LastSelectedValue { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            //fileContents = "003084650260000080000650020000800900800000005006001000080079000040000096092430700";
            fileContents = string.Empty;

        }

        private string FileContents
        {
            get
            {
                return fileContents;
            }

            set
            {
                fileContents = value;
            }
        }

        private static string RemoveNewLines(string input)
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
            // InitializeBoard sets up the property values for the custom button controls and
            // adds them as children of the PuzzleGrid and also as items in customButtonList.
            foreach (int row in Enumerable.Range(1, 9))
                foreach (int col in Enumerable.Range(1, 9))
                {
                    var customButton = new CustomButton
                    {
                        Height = buttonHeight,
                        Width = buttonWidth,
                        Name = $"cr{col}{row}",
                        Row = row,
                        Column = col,
                        Value = 0,
                        Region = GetRegion(col, row),
                        Candidates = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                    };

                    customButton.Click += Cell_Click;
                    PuzzleGrid.Children.Add(customButton);
                    customButtonList.Add(customButton);
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
                FileContents = RemoveNewLines(LoadGameFromDisk());
                if (FileContents.Length != 81)
                {
                    MessageBox.Show(@"Missing file or incorrect file format.");
                    return;
                }
            }

            int counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(FileContents[counter++].ToString());
                    var button = customButtonList.Single(cb => cb.Name == $"cr{col}{row}");

                    // If the value for a cell is greater than 0 it is a clue and cannot be changed by the user.
                    if (value > 0)
                    {
                        button.HasStartValue = true;
                        SetAsLockedCell(ref button);
                        button.Value = value;
                        button.Content = value.ToString();
                    }
                    // If the value is 0 the cell will contain a user-entered value.
                    else if (value == 0)
                    {
                        button.HasStartValue = false;
                        SetAsMarkupCell(ref button);
                        button.Value = value;
                        button.ContextMenu = FindResource("contextMenu1") as ContextMenu;
                        UpdateCandidates(ref button);
                        DisplayCandidates(ref button);
                    }
                }
            }

        }

        //private void DisplayMarkups()
        //{
        //    // DisplayMarkups sets up cells that have no value entered and do not contain a start clue
        //    // to display the possible values for each particular cell.
        //    foreach (var col in Enumerable.Range(1, 9))
        //    {
        //        foreach (var row in Enumerable.Range(1, 9))
        //        {
        //            foreach (var region in Enumerable.Range(1, 9))
        //            {
        //                foreach (var button in customButtonList.Where(cb => cb.Value == 0))
        //                {
        //                    var cb = button;
        //                    SetAsMarkupCell(ref cb);
        //                    //UpdateCandidates(col, row, region);
        //                    DisplayCandidates(ref cb);
        //                }
        //            }
        //        }

        //    }
        //}

        //private void ReloadGrid(string fileContents)
        //{
        //    // ReloadGrid loads the values for each cell from the FileContents property.
        //    int counter = 0;
        //    foreach (var row in Enumerable.Range(1, 9))
        //    {
        //        foreach (var col in Enumerable.Range(1, 9))
        //        {
        //            var value = int.Parse(fileContents[counter++].ToString());
        //            var customButton = (CustomButton)FindName($"cr{col}{row}");
        //            if (customButton == null) return;

        //            if (value > 0)
        //            {
        //                customButton.Content = $"{value}";
        //                SetAsLockedCell(ref customButton);
        //            }
        //            else if (value == 0)
        //            {
        //                customButton.Content = value;
        //                SetAsMarkupCell(ref customButton);
        //            }

        //        }
        //    }
        //}

        private void UpdateCandidates(ref CustomButton button)
        {
            if (LastSelectedValue == 0) return;

            var col = button.Column;
            var row = button.Row;
            var region = button.Region;

            foreach (var cb in customButtonList.Where(cb => cb.Column == col && cb.Value != 0))
            {
                if (cb.Value == LastSelectedValue)
                {
                    button.Candidates.Remove(LastSelectedValue);
                }
            }
            foreach (var cb in customButtonList.Where(cb => cb.Row == row && cb.Value != 0))
            {
                if (cb.Value == LastSelectedValue)
                {
                    button.Candidates.Remove(LastSelectedValue);
                }
            }
            foreach (var cb in customButtonList.Where(cb => cb.Region == region && cb.Value != 0))
            {
                if (cb.Value == LastSelectedValue)
                {
                    button.Candidates.Remove(LastSelectedValue);
                }
            }

        }
        private static void DisplayCandidates(ref CustomButton button)
        {
            string nl = Environment.NewLine;
            var candidatesString = string.Empty;
            foreach (int item in Enumerable.Range(1, 9))
            {
                if (button.Candidates.Contains(item))
                {
                    candidatesString += item.ToString();
                }
                else
                {
                    candidatesString += " ";
                }
            }

            foreach (var candidate in button.Candidates)
            {
                candidatesString += candidate.ToString();
            }

            button.Content = string.Format("{0}{1}{2}{1}{3}", candidatesString.Substring(0, 3), nl, candidatesString.Substring(3, 3), candidatesString.Substring(6, 3));
        }

        //private void UpdateCandidates()
        //{
        //    var col = 1;
        //    int row = 1;
        //    var counter = 0;

        //    foreach (var item in fileContents.ToCharArray())
        //    {

        //        Debug.WriteLine($"String position: ({counter.ToString().PadLeft(2)}) Column/Row: ({col}{row}) Value: ({item.ToString()})");
        //        counter++;
        //        col++;
        //        if (counter % 9 == 0)
        //        {
        //            col = 1;
        //            row += 1;
        //        }

        //        //todo: Retrieve buttons from customButtonList to update the values.
        //        // Set button properties here.
        //        var button = customButtonList.Single(cb => cb.Name == $"cr{col}{row}");
        //        SetAsMarkupCell(ref button);
        //        button.Value = item;

        //    }
        //}

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                if (mi.CommandParameter is ContextMenu cm)
                {
                    if (cm.PlacementTarget is CustomButton button)
                    {
                        if (button.HasStartValue)
                        {
                            MessageBox.Show("This cell contains a start clue and cannot be erased.");
                            return;
                        }

                        var content = mi.Header.ToString();
                        int col = int.Parse(button.Name.Substring(2, 1));
                        int row = int.Parse(button.Name.Substring(3, 1));
                        var region = GetRegion(col, row);


                        var oldString = FileContents;
                        var sb = new StringBuilder(oldString);
                        var startPosition = GetPositionInString(col, row);
                        sb.Remove(startPosition, 1);
                        sb.Insert(startPosition, content);
                        var newString = sb.ToString();
                        FileContents = newString;
                        LastSelectedValue = int.Parse(content);

                        if (IsNumeric(content) && IsMoveValid(col, row, region))
                        {
                            button.Value = LastSelectedValue;
                            SetAsNewValueCell(ref button);
                            UpdateCandidates(ref button);
                            DisplayCandidates(ref button);
                            button.Content = content;
                        }
                        else if (content == "Erase")
                        {
                            button.Value = 0;
                            button.Content = string.Empty;
                        }
                        else
                        {
                            MessageBox.Show("The selected number was invalid.");
                        }

                    }
                }
            }
        }

        private static int GetPositionInString(int col, int row)
        {
            int value = int.Parse($"{col}{row}");
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
            foreach (var customButton in customButtonList.Where(cb => cb.Column == col && cb.Value != 0))
            {
                if (customButton.Value == LastSelectedValue) return false;
            }

            // scan through row
            foreach (var customButton in customButtonList.Where(cb => cb.Row == row && cb.Value != 0))
            {
                if (customButton.Value == LastSelectedValue) return false;
            }

            // scan through region
            foreach (var customButton in customButtonList.Where(cb => cb.Region == region && cb.Value != 0))
            {
                if (customButton.Value == LastSelectedValue) return false;
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
                Title = openFileDialog.FileName.Substring(23);
                contents = File.ReadAllText(openFileDialog.FileName);
            }

            return contents;
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadSavedGame();
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
