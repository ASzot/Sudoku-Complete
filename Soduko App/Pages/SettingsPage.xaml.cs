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
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

using Soduko_App.Game_Logic;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Soduko_App.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SettingsPage : Soduko_App.Common.LayoutAwarePage
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            SetFontColors();

            BackgroundColorComboBox.SelectedIndex = Settings.GetBackgroundColorAsIndex();
            FontColorComboBox.SelectedIndex = Settings.GetFontColorAsIndex();
            ButtonFontColorComboBox.SelectedIndex = Settings.GetButtonFontColorAsIndex();
            ButtonBackgroundColorComboBox.SelectedIndex = Settings.GetButtonBackgroundAsIndex();
            ButtonBorderColorComboBox.SelectedIndex = Settings.GetButtonBorderAsIndex();
            TitleColorComboBox.SelectedIndex = Settings.GetTitleFontColorAsIndex();

            PictureBackgroundCheckBox.IsChecked = Settings.HasPictureBackground();
            if (Settings.HasPictureBackground())
            {
                PictureURLTextBox.Text = Settings.PictureURL;
            }
        }

        private async void SetFontColors()
        {
            MainGrid.Background = Settings.BackgroundColor;
            if (Settings.HasPictureBackground())
            {
                MainGrid.Background = await Settings.GetImageBrush();
            }

            pageTitle.Foreground = Settings.TitleFontColor;
            FontColorText.Foreground = Settings.FontColor;
            TitleColorText.Foreground = Settings.FontColor;
            BackgroundColorText.Foreground = Settings.FontColor;
            ButtonFontColorText.Foreground = Settings.FontColor;
            ButtonBackgroundColorText.Foreground = Settings.FontColor;
            ButtonBorderColorText.Foreground = Settings.FontColor;
            PictureURLLabel.Foreground = Settings.FontColor;
            PictureBackgroundText.Foreground = Settings.FontColor;
            PictureURLTextBox.Foreground = Settings.FontColor;

            ChooseButton.BorderBrush = Settings.ButtonBorderColor;
            ChooseButton.Background = Settings.ButtonBackgroundColor;
            ChooseButton.Foreground = Settings.ButtonFontColor;

            PictureBackgroundCheckBox.BorderBrush = Settings.ButtonBorderColor;
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

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Save the picture URL data. ( If any ).
            if (PictureBackgroundCheckBox.IsChecked == true)
            {
                string pictureURL = PictureURLTextBox.Text;
                if (pictureURL != null && pictureURL != "")
                {
                    Settings.PictureURL = pictureURL;
                }
                else
                {
                    e.Cancel = true;
                    MessageDialog dlg = new MessageDialog("Please select a picture as the background or uncheck the box!", "Problem");
                    await dlg.ShowAsync();
                }
            }
            else
            {
                Settings.PictureURL = null;
            }
            Settings.Save();

            base.OnNavigatingFrom(e);
        }

        private void ButtonBackgroundColorText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var cbiBtnBackground = (ComboBoxItem)ButtonBackgroundColorComboBox.SelectedItem;
            string btnBackground = (string)cbiBtnBackground.Content;

            Settings.SetButtonBackgroundColor(btnBackground);

            SetFontColors();

            Settings.Save();
        }

        private void ButtonFontColorText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var cbiBtnFont = (ComboBoxItem)ButtonFontColorComboBox.SelectedItem;
            string btnFont = (string)cbiBtnFont.Content;

            Settings.SetButtonFontColor(btnFont);

            SetFontColors();

            Settings.Save();
        }

        private void FontColorText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbiFont = (ComboBoxItem)FontColorComboBox.SelectedItem;
            string fontColor = (string)cbiFont.Content;

            Settings.SetFontColor(fontColor);

            SetFontColors();

            Settings.Save();
        }

        private void BackgroundColorText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbiBackground = (ComboBoxItem)BackgroundColorComboBox.SelectedItem;
            string backgroundColor = (string)cbiBackground.Content;

            Settings.SetBackgroundColor(backgroundColor);

            SetFontColors();

            Settings.Save();
        }

        private void ButtonBorderColorText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbiBorder = (ComboBoxItem)ButtonBorderColorComboBox.SelectedItem;
            string borderColor = (string)cbiBorder.Content;

            Settings.SetButtonBorderColor(borderColor);

            SetFontColors();

            Settings.Save();
        }

        private void TitleColorText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbiTitleFont = (ComboBoxItem)TitleColorComboBox.SelectedItem;
            string titleFontColor = (string)cbiTitleFont.Content;

            Settings.SetTitleFontColor(titleFontColor);

            SetFontColors();

            Settings.Save();
        }

        private void PictureBackgroundCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PictureSelectionPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void PictureBackgroundCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PictureSelectionPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                PictureURLTextBox.Text = file.Name;
                Settings.SetPictureFile(file);
            }
        }
    }
}
