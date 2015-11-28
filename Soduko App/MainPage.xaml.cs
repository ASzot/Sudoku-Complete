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
using Soduko_App.Pages;
using Soduko_App.Game_Logic;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Soduko_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ExperienceMgr _expMgr = new ExperienceMgr();

        public MainPage()
        {
            this.InitializeComponent();
            _expMgr.LoadData();
            Settings settings = new Settings();
            Settings.Restore();
            SetFontColor();
            bool continueGameEnabled = Serilizer.IsStateSaved();
            ContinueButton.Visibility = continueGameEnabled ? Visibility.Visible : Visibility.Collapsed;
            if (continueGameEnabled)
            {
                if (!SetContinueButtonText())
                {
                    // Something has gone terribly wrong.
                }
            }

            TimesPlayedData.LoadData();
            TimesPlayedData.UpdateUI(TimesPlayedTextBlock);

            _expMgr.UpdateControls(LevelProgress, CurrentLevel, CurrentExp);
        }



        private async void SetFontColor()
        {
            MainGrid.Background = Settings.BackgroundColor;
            if (Settings.HasPictureBackground())
            {
                MainGrid.Background = await Settings.GetImageBrush();
            }

            TimesPlayedTextBlock.Foreground = Settings.FontColor;
            MainTitleTextBlock.Foreground = Settings.TitleFontColor;

            PlayButton.Foreground = Settings.ButtonFontColor;
            PlayButton.Background = Settings.ButtonBackgroundColor;
            PlayButton.BorderBrush = Settings.ButtonBorderColor;

            HighscoresButton.Foreground = Settings.ButtonFontColor;
            HighscoresButton.Background = Settings.ButtonBackgroundColor;
            HighscoresButton.BorderBrush = Settings.ButtonBorderColor;

            SodukoSolver.Foreground = Settings.ButtonFontColor;
            SodukoSolver.Background = Settings.ButtonBackgroundColor;
            SodukoSolver.BorderBrush = Settings.ButtonBorderColor;

            SettingsButton.Foreground = Settings.ButtonFontColor;
            SettingsButton.Background = Settings.ButtonBackgroundColor;
            SettingsButton.BorderBrush = Settings.ButtonBorderColor;

            SettingsButton.Foreground = Settings.ButtonFontColor;
            SettingsButton.Background = Settings.ButtonBackgroundColor;
            SettingsButton.BorderBrush = Settings.ButtonBorderColor;

            ContinueButton.Foreground = Settings.ButtonFontColor;
            ContinueButton.Background = Settings.ButtonBackgroundColor;
            ContinueButton.BorderBrush = Settings.ButtonBorderColor;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GameSelectorPage));
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            SodukoInitInfo sii = new SodukoInitInfo();
            sii.RestoreState = true;
            Frame.Navigate(typeof(GamePage), sii);
        }

        private void PlayButton_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOverPlay", true);
        }

        private void ContinueButton_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOverContinue", true);
        }

        private void ContinueButton_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void PlayButton_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void HighscoresButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HighscoresPage));
        }

        private bool SetContinueButtonText()
        {
            string data;

            var adcv = ApplicationData.Current.LocalSettings.Values["AppData"] as ApplicationDataCompositeValue;
            if (adcv == null)
                return false;
            Int32  seconds = (int)(float)adcv["SecondsTicking"];
            Difficulty diff = (Difficulty)adcv["Difficulty"];

            data = diff.ToString() + ": " + seconds.FromSecondsToTimeFormat();

            string newData = "Continue: " + data;
            ContinueButton.Content = newData;

            return true;
        }

        private void SodukoSolver_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SolverPage));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

    }
}
