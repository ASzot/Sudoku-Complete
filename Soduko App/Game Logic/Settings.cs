using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace Soduko_App.Game_Logic
{
    class Settings
    {

        public Settings()
        {
            RedBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
            BlueBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 255));
            WhiteBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
            GreenBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
            BlackBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
            CyanBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 162, 255));
            DarkGreenBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 38, 127, 0));
            GrayBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 40, 40, 40));
        }

        private const string ADDRESS_BACKGROUND = "COLOR_SETTINGS_BACKGROUND_DATA";
        private const string ADDRESS_FONT = "COLOR_SETTINGS_FONT_DATA";
        private const string ADDRESS_BTN_FONT = "COLOR_SETTINGS_BTN_FONT_DATA";
        private const string ADDRESS_BTN_BACKGROUND = "COLOR_SETTINGS_BTN_BACKGROUND_DATA";
        private const string ADDRESS_BTN_BORDER = "COLOR_SETTINGS_BTN_BORDER_DATA";
        private const string ADDRESS_TITLE_FONT = "COLOR_SETTINGS_TITLE_FONT_DATA";
        private const string ADDRESS_PICTURE_URL = "BACKGROUND_PICTURE_URL_DATA";
        public static void Save()
        {
            Serilizer.SaveDataToAddress(ADDRESS_BACKGROUND, GetBackgroundColorAsString());
            Serilizer.SaveDataToAddress(ADDRESS_FONT, GetFontColorAsString());
            Serilizer.SaveDataToAddress(ADDRESS_BTN_FONT, GetButtonFontColorAsString());
            Serilizer.SaveDataToAddress(ADDRESS_BTN_BACKGROUND, GetButtonBackgroundColorAsString());
            Serilizer.SaveDataToAddress(ADDRESS_BTN_BORDER, GetButtonBorderColorAsString());
            Serilizer.SaveDataToAddress(ADDRESS_TITLE_FONT, GetTitleFontColorAsString());
            Serilizer.SaveDataToAddress(ADDRESS_PICTURE_URL, PictureURL);
        }

        public static async void Restore()
        {
            string background = (string)Serilizer.RestoreDataFromAddress(ADDRESS_BACKGROUND);
            string font = (string)Serilizer.RestoreDataFromAddress(ADDRESS_FONT);
            string btnFont = (string)Serilizer.RestoreDataFromAddress(ADDRESS_BTN_FONT);
            string btnBackground = (string)Serilizer.RestoreDataFromAddress(ADDRESS_BTN_BACKGROUND);
            string btnBorder = (string)Serilizer.RestoreDataFromAddress(ADDRESS_BTN_BORDER);
            string titleFont = (string)Serilizer.RestoreDataFromAddress(ADDRESS_TITLE_FONT);
            string pictureURL = (string)Serilizer.RestoreDataFromAddress(ADDRESS_PICTURE_URL);

            if (background == null)
            {
                background = "Gray";
                font = "Cyan";
                btnFont = "Black";
                btnBackground = "Cyan";
                btnBorder = "Dark Green";
                titleFont = "Dark Green";
                pictureURL = null;
            }

            SetBackgroundColor(background);
            SetFontColor(font);
            SetButtonFontColor(btnFont);
            SetButtonBackgroundColor(btnBackground);
            SetButtonBorderColor(btnBorder);
            SetTitleFontColor(titleFont);
            PictureURL = pictureURL;
            if (PictureURL != null)
            {
                // We also have a file associated here.
                _pictureFile = await ApplicationData.Current.LocalFolder.GetFileAsync(PictureURL);
            }
        }

        public static void SetBackgroundColor(string backgroundColor)
        {
            if (backgroundColor == "Black")
                BackgroundColor = BlackBrush;
            else if (backgroundColor == "White")
                BackgroundColor = WhiteBrush;
            else if (backgroundColor == "Gray")
                BackgroundColor = GrayBrush;
            else
                throw new ArgumentException();
        }

        public static void SetFontColor(string fontColor)
        {
            FontColor = SetNormalColorBrush(fontColor);
        }

        public static void SetButtonFontColor(string buttonFontColor)
        {
            ButtonFontColor = SetNormalColorBrush(buttonFontColor);
        }

        public static void SetButtonBackgroundColor(string buttonBackgroundColor)
        {
            ButtonBackgroundColor = SetNormalColorBrush(buttonBackgroundColor);
        }

        public static void SetButtonBorderColor(string buttonBorderColor)
        {
            ButtonBorderColor = SetNormalColorBrush(buttonBorderColor);
        }

        public static void SetTitleFontColor(string titleFontColor)
        {
            TitleFontColor = SetNormalColorBrush(titleFontColor);
        }

        public static Brush SetNormalColorBrush(string brushStr)
        {
            if (brushStr == "Green")
                return GreenBrush;
            else if (brushStr == "White")
                return WhiteBrush;
            else if (brushStr == "Blue")
                return BlueBrush;
            else if (brushStr == "Red")
                return RedBrush;
            else if (brushStr == "Black")
                return BlackBrush;
            else if (brushStr == "Cyan")
                return CyanBrush;
            else if (brushStr == "Dark Green")
                return DarkGreenBrush;
            else
                throw new ArgumentException();
        }

        public static int GetFontColorAsIndex()
        {
            return GetNormalColorIndex(FontColor);
        }

        public static int GetButtonFontColorAsIndex()
        {
            return GetNormalColorIndex(ButtonFontColor);
        }

        public static int GetButtonBackgroundAsIndex()
        {
            return GetNormalColorIndex(ButtonBackgroundColor);
        }

        public static int GetButtonBorderAsIndex()
        {
            return GetNormalColorIndex(ButtonBorderColor);
        }

        public static int GetTitleFontColorAsIndex()
        {
            return GetNormalColorIndex(TitleFontColor);
        }

        private static int GetNormalColorIndex(Brush b)
        {
            if (b == GreenBrush)
                return 0;
            else if (b == WhiteBrush)
                return 1;
            else if (b == BlueBrush)
                return 2;
            else if (b == RedBrush)
                return 3;
            else if (b == BlackBrush)
                return 4;
            else if (b == CyanBrush)
                return 5;
            else if (b == DarkGreenBrush)
                return 6;
            else
                throw new ArgumentException();
        }

        public static int GetBackgroundColorAsIndex()
        {
            if (BackgroundColor == BlackBrush)
                return 0;
            else if (BackgroundColor == WhiteBrush)
                return 1;
            else if (BackgroundColor == GrayBrush)
                return 2;
            else
                throw new ArgumentException();
        }

        public static string GetBackgroundColorAsString()
        {
            if (BackgroundColor == WhiteBrush)
                return "White";
            else if (BackgroundColor == BlackBrush)
                return "Black";
            else if (BackgroundColor == GrayBrush)
                return "Gray";
            else
                throw new ArgumentException();
        }

        public static string GetFontColorAsString()
        {
            return GetNormalColorAsString(FontColor);
        }

        public static string GetButtonFontColorAsString()
        {
            return GetNormalColorAsString(ButtonFontColor);
        }

        public static string GetButtonBackgroundColorAsString()
        {
            return GetNormalColorAsString(ButtonBackgroundColor);
        }

        public static string GetButtonBorderColorAsString()
        {
            return GetNormalColorAsString(ButtonBorderColor);
        }

        public static string GetTitleFontColorAsString()
        {
            return GetNormalColorAsString(TitleFontColor);
        }

        private static string GetNormalColorAsString(Brush b)
        {
            if (b == RedBrush)
                return "Red";
            else if (b == BlueBrush)
                return "Blue";
            else if (b == WhiteBrush)
                return "White";
            else if (b == GreenBrush)
                return "Green";
            else if (b == BlackBrush)
                return "Black";
            else if (b == CyanBrush)
                return "Cyan";
            else if (b == DarkGreenBrush)
                return "Dark Green";
            else
                throw new ArgumentException();
        }

        public static bool HasPictureBackground()
        {
            return (PictureURL != null);
        }

        private static async Task<ImageSource> GetImageSource()
        {
            if (PictureURL == null)
                return null;
            else
            {
                // Copy the image over to the local assets directory.
                StorageFolder destination = ApplicationData.Current.LocalFolder;
                
                // The file will already be copied.
                StorageFile pictureInDestinationFolder = await destination.GetFileAsync(_pictureFile.Name);

                string pictureInDestinationFolderStr = pictureInDestinationFolder.Path;

                BitmapImage bi = new BitmapImage(new Uri(pictureInDestinationFolderStr));
                return bi;
            }
        }

        public static async Task<Brush> GetImageBrush()
        {
            ImageSource imageSource = await GetImageSource();
            if (imageSource == null)
                return null;
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = imageSource;
            imageBrush.Stretch = Stretch.Fill;
            
            return imageBrush;
        }

        public static Brush TitleFontColor;
        public static Brush BackgroundColor;
        public static Brush FontColor;
        public static Brush ButtonFontColor;
        public static Brush ButtonBackgroundColor;
        public static Brush ButtonBorderColor;
        public static String PictureURL;


        private static StorageFile _pictureFile;
        public static async void SetPictureFile(StorageFile file)
        {
            // Delete the old picture.
            if (_pictureFile != null)
            {
                await _pictureFile.DeleteAsync();
                await file.CopyAsync(ApplicationData.Current.LocalFolder);
                _pictureFile = file;
            }
            else
            {
                // Check if the file actually does exist.
                try
                {
                    StorageFile possibleFile = await ApplicationData.Current.LocalFolder.GetFileAsync(file.Name);
                }
                catch (Exception)
                {
                    file.CopyAsync(ApplicationData.Current.LocalFolder);
                    _pictureFile = file;
                }
                
            }
            
        }

        private static SolidColorBrush RedBrush;
        private static SolidColorBrush BlueBrush;
        private static SolidColorBrush WhiteBrush;
        private static SolidColorBrush GreenBrush;
        private static SolidColorBrush BlackBrush;
        private static SolidColorBrush CyanBrush;
        private static SolidColorBrush DarkGreenBrush;
        private static SolidColorBrush GrayBrush;
    }
}
