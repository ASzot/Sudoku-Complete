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
using System.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Soduko_App.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class HighscoresPage : Soduko_App.Common.LayoutAwarePage
    {
        public HighscoresPage()
        {
            this.InitializeComponent();
            SodukoGameInfo sgi = new SodukoGameInfo();
            sgi = Serilizer.GetPersistingSodukoGameInfo();

            SetHighscores(sgi.Entries);

            // Set all of the time played data in the UI.
            TimesPlayedData.UpdateUI(TotalTimesPlayedTextBlock, EasyTimesPlayedTextBlock, NormalTimesPlayedTextBlock, HardTimesPlayedTextBlock);

            SetPageData();
        }

        private async void SetPageData()
        {
            MainGrid.Background = Settings.BackgroundColor;
            if (Settings.HasPictureBackground())
            {
                MainGrid.Background = await Settings.GetImageBrush();
            }

            pageTitle.Foreground = Settings.TitleFontColor;
            TotalTimesPlayedTextBlock.Foreground = Settings.FontColor;
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

        private async void SetHighscores(Dictionary<HighscoreKey, HighscoreEntry> highscores)
        {
            StringBuilder finalEasyEntryText = new StringBuilder();
            StringBuilder finalNormalEntryText = new StringBuilder();
            StringBuilder finalHardEntryText = new StringBuilder();

            for( int i = 0; i < highscores.Count; ++i)
            {
                KeyValuePair<HighscoreKey, HighscoreEntry> e = highscores.ElementAt(i);
                if (e.Key.Diff == Difficulty.Easy)
                {
                    finalEasyEntryText.Append(e.Key.Diff.ToString());
                    finalEasyEntryText.Append(": ");
                    finalEasyEntryText.Append(e.Value.Seconds.FromSecondsToTimeFormat());
                    finalEasyEntryText.Append(Environment.NewLine);
                }
                else if (e.Key.Diff == Difficulty.Normal)
                {
                    finalNormalEntryText.Append(e.Key.Diff.ToString());
                    finalNormalEntryText.Append(": ");
                    finalNormalEntryText.Append(e.Value.Seconds.FromSecondsToTimeFormat());
                    finalNormalEntryText.Append(Environment.NewLine);
                }
                else if (e.Key.Diff == Difficulty.Hard)
                {
                    finalHardEntryText.Append(e.Key.Diff.ToString());
                    finalHardEntryText.Append(": ");
                    finalHardEntryText.Append(e.Value.Seconds.FromSecondsToTimeFormat());
                    finalHardEntryText.Append(Environment.NewLine);
                }

            }


            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    EasyHighscore1.Text = finalEasyEntryText.ToString();
                    NormalHighscore1.Text = finalNormalEntryText.ToString();
                    HardHighscore1.Text = finalHardEntryText.ToString();
                });
        }
    }
}
