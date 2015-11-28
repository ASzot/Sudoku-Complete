using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Soduko_App.Game_Logic;
using Windows.UI.Popups;
using System.Threading;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI;
using Soduko_App.Game_Logic.UILogic;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Soduko_App.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage : Soduko_App.Common.LayoutAwarePage
    {
        private SodukoPuzzle _puzzle;
        private ExperienceMgr _expMgr = new ExperienceMgr();
        private SodukoPiece _selectedPiece;
        private DispatcherTimer _dispatchTimer = new DispatcherTimer();
        private float _secondsTicking = 0;
        private Difficulty _difficulty = Difficulty.Easy;
        private SodukoGameInfo _gameInfo = new SodukoGameInfo();
        private SodukoInitInfo _sii = new SodukoInitInfo();
        private bool _hasInitialized = false;
        // This value is in seconds.
        private const int _saveRate = 4;
        // private AppSettingsBar _settingsBar;
        private bool _wasGameBeaten = false;
        private bool _hasSaved = false;

        public GamePage()
        {
            this.InitializeComponent();

            SetFontColors();
        }

        private async void SetFontColors()
        {
            MainGrid.Background = Settings.BackgroundColor;
            if (Settings.HasPictureBackground())
            {
                MainGrid.Background = await Settings.GetImageBrush();
            }

            pageTitle.Foreground = Settings.TitleFontColor;
            difficultyTextBlock.Foreground = Settings.FontColor;

            LoadingText.Foreground = Settings.FontColor;

            TimerTextBlock.Foreground = Settings.FontColor;
            HintModeComboBox.Foreground = Settings.FontColor;
            HintModeLabelText.Foreground = Settings.FontColor;

            HighscoreLabelText.Foreground = Settings.FontColor;
            HighscoresText.Foreground = Settings.FontColor;


            ForfeitButton.Foreground = Settings.ButtonFontColor;
            ForfeitButton.Background = Settings.ButtonBackgroundColor;
            ForfeitButton.BorderBrush = Settings.ButtonBorderColor;

            ResetButton.Foreground = Settings.ButtonFontColor;
            ResetButton.Background = Settings.ButtonBackgroundColor;
            ForfeitButton.BorderBrush = Settings.ButtonBorderColor;
        }


        private void OnSettingsCommand(IUICommand command)
        {
            var id = command.Id.ToString();
            if (id == AppSettingsBar.SETTINGS_COMMAND_SOLVE_PUZZLE)
            {
                // Activate all button related callback code.
                _puzzle.SolvePuzzle(true);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _sii = (SodukoInitInfo)e.Parameter;
            
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
            if (!_wasGameBeaten && !_hasSaved)
            {
                SaveData();
            }
        }

        private async void InitPuzzle()
        {
            _hasSaved = false;
            try
            {
                _puzzle = new SodukoPuzzle(9, PuzzleCanvas, HintMode.Adjacent);
                _puzzle.OnCompletedGame += OnGameCompleted;
            }
            catch (ArgumentException e)
            {
                // Do nothing...
                string failureMessage = e.Message;
            }
            
            _puzzle.ShowNumberSelector += OnObjectTapped;

            if (_sii.RestoreState)
            {
                if (Serilizer.IsStateSaved())
                {
                    LoadData();
                }
            }
            else
            {
                _difficulty = _sii.DesiredDifficulty;
                if (!_puzzle.InitPuzzle(_difficulty))
                {
                    MessageDialog dlg = new MessageDialog("Couldn't init the puzzle!", "Failure");
                    await dlg.ShowAsync();
                }
            }
            // The highscore is completely different and is always saved.
            _gameInfo = Serilizer.GetPersistingSodukoGameInfo();

            _expMgr.LoadData();
            

            DisplayHighScore();
            SetDifficulty(_difficulty);
            ShowNumberSelector();
            StartTimer(true);
            HintModeComboBox.SelectedItem = AdjacentComboBoxItem;
        }

        private async void ShowNumberSelector()
        {
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
                        var piece = new SodukoPiece(true,false);
                        piece.NumberValue = i;
                        piece.OnTappedEvent += NumberSelectorTapped;
                        NumberSelectorStackPanel.Children.Add(piece);
                    }
                });
        }

        private void OnObjectTapped(object sender, EventArgs e)
        {
            SodukoPiece p = (SodukoPiece)sender;
            if (_selectedPiece != null)
            {
                _selectedPiece.SetFocus(false);
                _puzzle.ClearAllSuggestionText();
            }
            if (!p.IsPresetPiece)
            {
                _selectedPiece = p;
                _selectedPiece.SetFocus(true);
                _puzzle.SetSuggestions(_selectedPiece);
            }

        }

        private void NumberSelectorTapped(object sender,TappedRoutedEventArgs e)
        {
            GC.Collect();
            if (_selectedPiece == null)
                return;
            int senderValue = ((SodukoPiece)sender).NumberValue;
            if (senderValue == -1 || _puzzle.EnsureValidMove(_selectedPiece,senderValue,true))
            {
                _puzzle.SetPiece(_selectedPiece.Row, _selectedPiece.Col, senderValue);
                _selectedPiece.RotateAnimation();
                _selectedPiece.SetFocus(false);
                _selectedPiece.NumberValue = senderValue;
                _selectedPiece.SetSuggestionTextToNothing();
                // Update the suggestions.
                _puzzle.SetSuggestions(_selectedPiece);
                if (_puzzle.CheckForWin())
                {
                    OnGameCompleted(this, new EventArgs());
                }
            }
            GC.Collect();
        }

        private async void HideNumberSelector()
        {
            if (NumberSelectorStackPanel.Children.Count == 0)
                return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                NumberSelectorStackPanel.Children.Clear();
            });
            
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _secondsTicking = 0;
            _puzzle.ResetPuzzle();
        }

        private void StartTimer(bool delayedStart)
        {
            _dispatchTimer.Interval = new TimeSpan(0, 0, 0, 1);
            _dispatchTimer.Tick += TimerTick;
            _dispatchTimer.Start();
        }

        private async void TimerTick(object sender, object args)
        {
            // A variable tick time could be added which could be based off the difficulty setting.
            const float tickTime = 1.0f;

            _secondsTicking += tickTime;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                TimerTextBlock.Text = FormatTime(_secondsTicking);
            });

            if (_secondsTicking % 5 == 0)
            {
                SaveData();
            }
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            // Function doesn't even use argument.
            SaveState(null);
            _hasSaved = true;
            Frame.Navigate(typeof(MainPage));
        }

        private void SaveApplicationData()
        {
            _expMgr.SerilizeData();
            // Save the persisting data.
            Serilizer.SavePersistingSodukoGameInfo(_gameInfo);
        }

        private void SaveData()
        {
            if (_wasGameBeaten)
                return;
            if (_hasInitialized)
                return;
            // Save the page data.
            var adcv = new ApplicationDataCompositeValue();
            adcv["SecondsTicking"] = _secondsTicking;
            adcv["Difficulty"] = (int)_difficulty;
            ApplicationData.Current.LocalSettings.Values["AppData"] = adcv;

            // Save the puzzle specific data.
            _puzzle.SaveState();

            
        }

        private void LoadData()
        {
            // Load the page data.
            var adcv = ApplicationData.Current.LocalSettings.Values["AppData"] as ApplicationDataCompositeValue;
            _secondsTicking = (float)adcv["SecondsTicking"];
            _difficulty = (Difficulty)adcv["Difficulty"];

            // Load the puzzle specific data.
            _puzzle.RestoreState();
            _puzzle.SoftInit();
        }

        private async void SetDifficulty(Difficulty d)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    difficultyTextBlock.Text = d.ToString();
                });
        }

        private async void DisplayHighScore()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    HighscoreKey key = new HighscoreKey();
                    key.Diff = _difficulty;
                    key.Place = 1;
                    HighscoreEntry currentEntry = _gameInfo.GetHighscoreEntry(key);
                    HighscoresText.Text = FormatTime(currentEntry.Seconds);
                });
        }

        private string FormatTime(float seconds)
        {
            int secondsWhole = (int)seconds;
            int minutesTicking = secondsWhole / 60;
            int secondsTicking = secondsWhole % 60;
            int hoursTicking = minutesTicking / 60;
            string min, sec, hour;
            if (minutesTicking < 10)
                min = "0" + minutesTicking.ToString();
            else
                min = minutesTicking.ToString();
            if (secondsTicking < 10)
                sec = "0" + secondsTicking.ToString();
            else
                sec = secondsTicking.ToString();
            if (hoursTicking < 10)
                hour = "0" + hoursTicking.ToString();
            else
                hour = hoursTicking.ToString();
            string  r = hour + ":" + min + ":" + sec;
            return r;
        }

        private void LoadingBar_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (!_sii.RestoreState && _sii.DesiredDifficulty == Difficulty.Hard)
            {
                // Display the warning message about the load times.
                LoadingText.Visibility = Visibility.Visible;
            }
            DispatcherTimer waiter = new DispatcherTimer();
            waiter.Interval = new TimeSpan(0, 0, 1);
            waiter.Start();
            // Wait one second for the page to load so we can display the progress bar and initialize the puzzle.
            waiter.Tick += async (object obj1, object obj2) =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                        {

                            ContentGrid.Visibility = Visibility.Collapsed;
                            NumberSelectorGrid.Visibility = Visibility.Collapsed;
                            LoadingText.Visibility = Visibility.Visible;

                            

                            InitPuzzle();

                            _expMgr.UpdateControls(LevelProgress, CurrentLevel, CurrentExp);

                            LoadingBar.Visibility = Visibility.Collapsed;
                            LoadingText.Visibility = Visibility.Collapsed;

                            ContentGrid.Visibility = Visibility.Visible;
                            NumberSelectorGrid.Visibility = Visibility.Visible;
                            SettingsStackPanel.Visibility = Visibility.Visible;

                            waiter.Stop();
                        });
                };
            
        }

        public async void OnGameCompleted(object obj, EventArgs args)
        {
            _wasGameBeaten = true;
            Serilizer.ClearSaveGameCache();

            string message = null;
            string levelUpMessage = null;
            int numberOfLevelups = 0;
            EventHandler onLevelUp = (object o,EventArgs e) =>
            {
                numberOfLevelups++;
            };

            // Sub our level up event just in case we did end level up.
            _expMgr.OnLevelUp += onLevelUp;
            _expMgr.PuzzleCompleted(_difficulty, (int)_secondsTicking);

            if (numberOfLevelups != 0)
            {
                levelUpMessage = Environment.NewLine + "You leveled up " + numberOfLevelups.ToString();
                if (numberOfLevelups > 1)
                    levelUpMessage += " times";
                else
                    levelUpMessage += " time";
            }
            
            if ( _gameInfo.AddIfHighScore(_difficulty, (int)_secondsTicking) )
                message = "New Highscore";
            string finalMessage = "You win!";
            if (message != null)
                finalMessage = finalMessage + Environment.NewLine + message;
            SaveApplicationData();

            // Add to the total number of times the game was beaten.
            TimesPlayedData.OnGameBeaten(_difficulty);

            MessageDialog dlg = new MessageDialog(finalMessage, "Congragulations!");
            await dlg.ShowAsync();
            Frame.Navigate(typeof(MainPage));
        }

        private void PuzzleCanvas_Loaded_1(object sender, RoutedEventArgs e)
        {
            // Scale the canvas accordingly for all screen sizes.
            double height = CanvasGrid.ActualHeight - (PuzzleCanvas.Margin.Top + PuzzleCanvas.Margin.Bottom);
            double width  = CanvasGrid.ActualWidth  - (PuzzleCanvas.Margin.Left + PuzzleCanvas.Margin.Top);
            double minDimension = Math.Min(height,width);
            PuzzleCanvas.Height = minDimension;
            PuzzleCanvas.Width  = minDimension;
        }

        private void HintModeComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            IList<object> selected = e.AddedItems;
            object[] array = selected.ToArray();

            ComboBoxItem cbi = (ComboBoxItem)array[0];

            string selectedName = (string)cbi.Content;
            HintMode finalHintMode = (HintMode)Enum.Parse(typeof(HintMode), selectedName);
            _puzzle.SetHintMode(finalHintMode,_selectedPiece);
        }

        private void MainGrid_Loaded_1(object sender, RoutedEventArgs e)
        {
            //SettingsPane.GetForCurrentView().CommandsRequested += _settingsBar.AppSettingsBar_CommandsRequested;
        }

        private async void ForfeitButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the puzzles answer and don't fire the OnPuzzleCompleted event.
            MessageDialog dlg = new MessageDialog("Are you certain?", "Continue");
            bool solveThePuzzle = false;
            UICommandInvokedHandler okProc = (IUICommand command) =>
                {
                    solveThePuzzle = true;
                };
            dlg.Commands.Add(new UICommand("Ok", okProc));
            dlg.Commands.Add(new UICommand("No"));
            await dlg.ShowAsync();

            if (solveThePuzzle)
            {
                _wasGameBeaten = true;
                Serilizer.ClearSaveGameCache();
                _puzzle.SolvePuzzle(false);
                int tickCount = 0;
                // Wait five seconds and then navigate back.
                DispatcherTimer timer = new DispatcherTimer();
                EventHandler<object> timerTick = (object senderTimer, object args) =>
                    {
                        tickCount++;
                        if (tickCount > 5)
                            Frame.Navigate(typeof(MainPage));
                    };
                timer.Interval = new TimeSpan(0, 0, 5);
                timer.Tick += timerTick;
                // Stop the main timer as we want to stop our game from updating.
                _dispatchTimer.Stop();

            }
        }

    }
}
