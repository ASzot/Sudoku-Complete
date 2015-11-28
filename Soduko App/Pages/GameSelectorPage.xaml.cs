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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Soduko_App.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GameSelectorPage : Soduko_App.Common.LayoutAwarePage
    {
        private SodukoInitInfo _initInfo = new SodukoInitInfo();
        public GameSelectorPage()
        {
            this.InitializeComponent();

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

            // Now set the button stuff.
            EasyButton.Foreground = Settings.ButtonFontColor;
            EasyButton.Background = Settings.ButtonBackgroundColor;
            EasyButton.BorderBrush = Settings.ButtonBorderColor;

            NormalButton.Foreground = Settings.ButtonFontColor;
            NormalButton.Background = Settings.ButtonBackgroundColor;
            NormalButton.BorderBrush = Settings.ButtonBorderColor;

            HardButton.Foreground = Settings.ButtonFontColor;
            HardButton.Background = Settings.ButtonBackgroundColor;
            HardButton.BorderBrush = Settings.ButtonBorderColor;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _initInfo.RestoreState = false;
            base.OnNavigatedTo(e);
        }

        private void EasyButton_Click(object sender, RoutedEventArgs e)
        {
            _initInfo.DesiredDifficulty = Difficulty.Easy;
            Frame.Navigate(typeof(GamePage), _initInfo);
        }

        private void NormalButton_Click(object sender, RoutedEventArgs e)
        {
            _initInfo.DesiredDifficulty = Difficulty.Normal;
            Frame.Navigate(typeof(GamePage), _initInfo);
        }

        private void HardButton_Click(object sender, RoutedEventArgs e)
        {
            _initInfo.DesiredDifficulty = Difficulty.Hard;
            Frame.Navigate(typeof(GamePage), _initInfo);
        }
    }
}
