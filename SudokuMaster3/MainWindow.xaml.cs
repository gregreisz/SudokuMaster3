using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace SudokuMaster3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private static string FilterOutReturns(string input)
        {
            return !string.IsNullOrEmpty(input)
                ? string.Join(string.Empty, Regex.Split(input, @"(?:\r\n|\n|\r|[ ])"))
                : string.Empty;
        }

        private void InitializeBoard()
        {
            var contents = FilterOutReturns(LoadGameFromDisk());
            var counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var value = int.Parse(contents[counter++].ToString());
                    var button = GetButtonByName($"cr{col}{row}");
                    if (button != null)
                    {
                        button.Content = value > 0 ? value.ToString() : string.Empty;
                        if (value > 0) button.Background = new SolidColorBrush(Colors.LightSteelBlue);
                        if (col != 9)
                        {
                            textBox1.Text += $"{col}{row}{value}";
                        }
                        else
                        {
                            textBox1.Text += $"{col}{row}{value}" + Environment.NewLine;
                        }

                    }
                }


            }
        }

        private Button GetButtonByName(string name)
        {
            var container = UniformGrid;
            var btn = container.FindName(name);
            if (btn is Button button)
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
            InitializeBoard();
        }

        private void ButtonClearText_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = string.Empty;
        }

    }
}
