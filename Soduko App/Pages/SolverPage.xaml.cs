using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Soduko_App.Game_Logic;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Soduko_App.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SolverPage : Soduko_App.Common.LayoutAwarePage
    {
        private SodukoPuzzle _puzzle;
        private SodukoPiece _selectedPiece;

        public SolverPage()
        {
            this.InitializeComponent();

            SetUIColors();

            InitPuzzle();

            WarningText.Visibility = Visibility.Visible;
        }

        private async void SetUIColors()
        {
            MainGrid.Background = Settings.BackgroundColor;
            if (Settings.HasPictureBackground())
            {
                MainGrid.Background = await Settings.GetImageBrush();
            }
            
            // Set the button stuff.
            HelpButton.Foreground = Settings.ButtonFontColor;
            HelpButton.Background = Settings.ButtonBackgroundColor;
            HelpButton.BorderBrush = Settings.ButtonBorderColor;

            ResetButton.Foreground = Settings.ButtonFontColor;
            ResetButton.Background = Settings.ButtonBackgroundColor;
            ResetButton.BorderBrush = Settings.ButtonBorderColor;

            SolveButton.Foreground = Settings.ButtonFontColor;
            SolveButton.Background = Settings.ButtonBackgroundColor;
            SolveButton.BorderBrush = Settings.ButtonBorderColor;

            OpenTimeoutButton.Foreground = Settings.ButtonFontColor;
            OpenTimeoutButton.Background = Settings.ButtonBackgroundColor;
            OpenTimeoutButton.BorderBrush = Settings.ButtonBorderColor;

            // Set the text stuff.
            pageTitle.Foreground = Settings.TitleFontColor;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void PuzzleCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Scale the canvas accordingly for all screen sizes.
            double height = CanvasGrid.ActualHeight - (PuzzleCanvas.Margin.Top + PuzzleCanvas.Margin.Bottom);
            double width  = CanvasGrid.ActualWidth  - (PuzzleCanvas.Margin.Left + PuzzleCanvas.Margin.Top);
            double minDimension = Math.Min(height,width);
            PuzzleCanvas.Height = minDimension;
            PuzzleCanvas.Width  = minDimension;
        }

        private void OnObjectTapped(object sender, EventArgs e)
        {
            SodukoPiece p = (SodukoPiece)sender;
            if (_selectedPiece != null)
            {
                _selectedPiece.SetFocus(false);
            }

            _selectedPiece = p;
            _selectedPiece.SetFocus(true);
        }

        private async void InitPuzzle()
        {
            _puzzle = new SodukoPuzzle(9, PuzzleCanvas, HintMode.Off);
            _puzzle.OnCompletedGame += _puzzle_OnCompletedGame;
            _puzzle.ShowNumberSelector += OnObjectTapped;

            _puzzle.InitPuzzle();


            if (NumberSelectorStackPanel.Children.Count != 0)
                return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                var clearPiece = new SodukoPiece(true, false);
                clearPiece.SetUnknown();
                clearPiece.OnTappedEvent += NumberSelectorTapped;
                NumberSelectorStackPanel.Children.Add(clearPiece);

                for (int i = 1; i < 10; ++i)
                {
                    var piece = new SodukoPiece(true, false);
                    piece.NumberValue = i;
                    piece.OnTappedEvent += NumberSelectorTapped;
                    NumberSelectorStackPanel.Children.Add(piece);
                }
            });
        }

        private void NumberSelectorTapped(object sender, TappedRoutedEventArgs e)
        {
            GC.Collect();

            if (_selectedPiece == null)
                return;

            int senderValue = ((SodukoPiece)sender).NumberValue;
            _puzzle.SetPiece(_selectedPiece.Row, _selectedPiece.Col, senderValue);

            _selectedPiece.RotateAnimation();
            _selectedPiece.SetFocus(false);
            _selectedPiece.NumberValue = senderValue;

            GC.Collect();
        }

        void _puzzle_OnCompletedGame(object sender, EventArgs e)
        {
            // Don't do anything.
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            // Solve the puzzle given the user input.
            try
            {
                SodukoPiece.UseRotationAnimation = true;
                _puzzle.SolveUserPuzzle();
                SodukoPiece.UseRotationAnimation = false;
            }
            catch (TimeoutException ex)
            {
                // We timed out during the solving of this puzzle.
                MessageDialog dlg = new MessageDialog("There was an error solving this puzzle! Either the puzzle is unsolvable or there was a timeout while solving the puzzle. Ensure the puzzle is solvable or increase the timeout and try again."
                    + Environment.NewLine + Environment.NewLine + "The timeout is currently "
                    + ex.Message + ".",
                    "Error");
                dlg.ShowAsync();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _puzzle.ResetPuzzle();
        }

        private async void AcceptTimeout_Click(object sender, RoutedEventArgs e)
        {
            // The user has input a new timeout.
            // Ensure that it is a valid number.
            string text = NewTimeoutTextBox.Text;
            uint result;
            if (uint.TryParse(text, out result))
            {
                SodukoSolver.SetTimeout(result);
                TimeoutModifierPanel.Visibility = Visibility.Collapsed;
                InitialTimeoutPanel.Visibility = Visibility.Visible;
            }
            else
            {
                // Invalid input.
                MessageDialog dlg = new MessageDialog("Please enter valid input!", "Error");
                await dlg.ShowAsync();
            }
        }

        private void OpenTimeoutButton_Click(object sender, RoutedEventArgs e)
        {
            InitialTimeoutPanel.Visibility = Visibility.Collapsed;
            TimeoutModifierPanel.Visibility = Visibility.Visible;

            NewTimeoutTextBox.Text = SodukoSolver.GetTimeout().ToString();
        }

        private async void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "This tool is to help in the solving of any sudoku puzzle. Simply enter the solvable puzzle's values into the table and click the solve button which will solve the puzzle displaying the results in the table. " +
                "The timeout feature is how long the computer should spend attempting to solve the puzzle." + "The higher the timeout value the higher the chance the puzzle will be solved. " + "Keep in mind a large timeout can also result in long load times if the puzzle is difficult to solve.";
            MessageDialog dlg = new MessageDialog(message, "Help");
            await dlg.ShowAsync();
        }
    }
}
