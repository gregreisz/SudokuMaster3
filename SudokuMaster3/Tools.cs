using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SudokuMaster3
{
    public static class Tools
    {
        public static string FormatCandidates(string input)
        {
            var lf = Environment.NewLine;
            var output = $"{input.Substring(0, 3)}{lf}{input.Substring(3, 3)}{lf}{input.Substring(6, 3)}";
            return output;
        }

        public static void TraceMessage(string message,
            [CallerMemberName] string callingMethod = "",
            [CallerFilePath] string callingFilePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerLineNumber] int callingFileLineNumber = 0, [CallerMemberName] string caller = null)
        {
            Trace.WriteLine(message + " at line " + lineNumber + " (" + caller + ")");
        }

        public static string RemoveNewLines(string input)
        {
            return !string.IsNullOrEmpty(input)
                ? string.Join(string.Empty, Regex.Split(input, @"(?:\r\n|\n|\r|[ ])"))
                : string.Empty;
        }

        public static int GetRegion(int col, int row)
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

        public static void ShowMessage(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            MessageBox.Show(message + " at line " + lineNumber + " (" + caller + ")");
        }

        public static IEnumerable<Visual> GetChildren(this Visual parent, bool recurse = true)
        {
            if (parent != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(parent);
                for (var i = 0; i < count; i++)
                {
                    // Retrieve child visual at specified index value.
                    if (VisualTreeHelper.GetChild(parent, i) is Visual child)
                    {
                        yield return child;

                        if (recurse)
                        {
                            foreach (var grandChild in child.GetChildren())
                            {
                                yield return grandChild;
                            }
                        }
                    }
                }
            }
        }
    }
}
