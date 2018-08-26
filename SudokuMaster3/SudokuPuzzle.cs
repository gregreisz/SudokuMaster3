using System.Windows;

namespace SudokuMaster3
{
    public class SudokuPuzzle : DependencyObject
    {

        public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached("Region", typeof(int), typeof(SudokuPuzzle));

        // Create attached property Region
        public static void SetRegion(UIElement element, int value)
        {
            element.SetValue(RegionProperty, value);
        }

        public static int GetRegion(UIElement element)
        {
            return (int)element.GetValue(RegionProperty);
        }

        // Create attached property Column
        public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached("Column", typeof(int), typeof(SudokuPuzzle));

        public static void SetColumn(UIElement element, int value)
        {
            element.SetValue(ColumnProperty, value);
        }

        public static int GetColumn(UIElement element)
        {
            return (int)element.GetValue(ColumnProperty);
        }
    
        // Create attached property Row
        public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached("Row", typeof(int), typeof(SudokuPuzzle));

        public static void SetRow(UIElement element, int value)
        {
            element.SetValue(RowProperty, value);
        }

        public static int GetRow(UIElement element)
        {
            return (int)element.GetValue(RowProperty);
        }
   
        // Create attached property IsGiven
        public static readonly DependencyProperty IsGivenProperty = DependencyProperty.RegisterAttached("IsGiven", typeof(bool), typeof(SudokuPuzzle));

        public static void SetIsGiven(UIElement element, bool value)
        {
            element.SetValue(IsGivenProperty, value);
        }

        public static bool GetIsGiven(UIElement element)
        {
            return (bool)element.GetValue(IsGivenProperty);
        }
  
        // Create attached property Candidates
        public static readonly DependencyProperty CandidatesProperty = DependencyProperty.RegisterAttached("Candidates", typeof(string), typeof(SudokuPuzzle));

        public static void SetCandidates(UIElement element, string value)
        {
            element.SetValue(CandidatesProperty, value);
        }

        public static string GetCandidates(UIElement element)
        {
            return (string)element.GetValue(CandidatesProperty);
        }
   
        // Create attached property Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(int), typeof(SudokuPuzzle));

        public static void SetValue(UIElement element, int value)
        {
            element.SetValue(ValueProperty, value);
        }

        public static int GetValue(UIElement element)
        {
            return (int)element.GetValue(ValueProperty);
        }
    }
}