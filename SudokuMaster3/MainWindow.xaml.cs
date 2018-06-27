using System;
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
        public bool IsLocked { get; set; }

        public int? Value { get; set; }

    }

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
        }

        private static string FilterOutReturns(string input)
        {
            return !string.IsNullOrEmpty(input)
                ? string.Join(string.Empty, Regex.Split(input, @"(?:\r\n|\n|\r|[ ])"))
                : string.Empty;
        }

        private void InitializeBoard()
        {
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var button = GetCustomButtonByName($"cr{col}{row}");
                    if (button != null)
                    {
                        button.Background = new SolidColorBrush(Colors.LightYellow);
                    }
                }
            }
        }


        private void GetSavedGame()
        {
            var contents = FilterOutReturns(LoadGameFromDisk());
            if (contents.Length != 81) return;

            int counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(contents[counter].ToString());
                    var button = GetCustomButtonByName($"cr{col}{row}");
                    if (button != null)
                    {
                        if (value > 0)
                        {
                            button.IsLocked = true;
                            button.Background = new SolidColorBrush(Colors.LightSteelBlue);
                            button.Content = $"{value}";
                        }
                        else if (value == 0)
                        {
                            button.IsLocked = false;
                            button.Background = new SolidColorBrush(Colors.LightYellow);
                            button.ContextMenu = (ContextMenu)FindResource("contextMenu");
                        }

                        button.Value = value;
                    }

                    counter++;
                }
            }

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((MenuItem)sender).Header.ToString().Substring(5));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                if (mi.CommandParameter is ContextMenu cm)
                {
                    if (cm.PlacementTarget is CustomButton button)
                    {
                        button.Content = mi.Header.ToString().Substring(5, 1);
                    }
                }
            }
        }

        private CustomButton GetCustomButtonByName(string name)
        {
            var container = PuzzleGrid;
            var btn = container.FindName(name);
            if (btn is CustomButton button)
            {
                return button;
            }

            return null;
        }

        private string LoadGameFromDisk()
        {
            var contents = string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"SDO files (*.sdo)|*.sdo|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Title = openFileDialog.FileName;
                contents = File.ReadAllText(openFileDialog.FileName);
            }

            return contents;
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            GetSavedGame();
        }

        private void ButtonClearText_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = string.Empty;
        }

    }
}
