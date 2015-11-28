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
using Windows.UI;
using System.Text;

using Soduko_App.Game_Logic;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Soduko_App
{
    public sealed partial class SodukoPiece : UserControl
    {
        private int _scaleFactor = 90;
        private int _numberValue;
        private bool _anim;
        public event EventHandler<TappedRoutedEventArgs> OnTappedEvent;
        public bool IsPresetPiece;

        public int Row;
        public int Col;

        public static bool UseRotationAnimation = false;

        public SodukoPiece(bool isPresetPiece, bool anim = true)
        {
            this.InitializeComponent();

            if (isPresetPiece)
            {
                CaptionText.Foreground = Settings.FontColor;
            }
            else
            {
                Color captionColor = (Settings.FontColor as SolidColorBrush).Color;
                const byte decrement = 100;
                captionColor.B = (byte)((captionColor.B == (byte)0) ? (captionColor.B + decrement) : (captionColor.B - decrement));
                captionColor.G = (byte)((captionColor.G == (byte)0) ? (captionColor.G + decrement) : (captionColor.G - decrement));
                captionColor.R = (byte)((captionColor.R == (byte)0) ? (captionColor.R + decrement) : (captionColor.R - decrement));

                SolidColorBrush captionBrush = new SolidColorBrush(captionColor);
                CaptionText.Foreground = captionBrush;

                SetUnknown();
            }
            IsPresetPiece = isPresetPiece;
            _anim = anim;
        }

        public int NumberValue
        {
            get 
            {
                return _numberValue; 
            }
            set
            {
                RotateAnimation();
                if (value == -1)
                {
                    SetUnknown();
                }
                else
                {
                    SafeSetPiece(value.ToString());
                }
                _numberValue = value;
            }
        }

        private async void SafeSetPiece(string s)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    CaptionText.Text = s;
                });
        }

        public void SetBorderBrush(bool useBlue)
        {
            Color c;
            if (useBlue)
            {
                c = Color.FromArgb(255, 0, 0, 255);
            }
            else
            {
                c = Color.FromArgb(255, 255, 0, 0);
            }
            SolidColorBrush b = new SolidColorBrush(c);
            border.BorderBrush = b;
        }

        public void RotateAnimation()
        {
            // Keep this?
            // This is the sole root of the game crashing.
            if (_anim && UseRotationAnimation)
            {
                RotateStoryboard.Begin();
            }
        }

        public void SetUnknown()
        {
            CaptionText.Text = "";
            _numberValue = -1;
        }

        private void border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        private void border_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        public void Piece_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OnTappedEvent(this, e);
        }

        public void SetPosition(int row, int column)
        {
            SetValue(Canvas.LeftProperty, _scaleFactor * column);
            SetValue(Canvas.TopProperty, _scaleFactor * row);
            Col = column;
            Row = row;
        }

        public void SetScaling(int scaleFactor)
        {
            _scaleFactor = scaleFactor;
            CaptionText.FontSize = scaleFactor - 8;
            PieceBackground.Width = scaleFactor;
            PieceBackground.Height = scaleFactor;
        }

        public async void SetFocus(bool focus)
        {
            Windows.UI.Core.DispatchedHandler action;
            if (focus)
            {
                action = () =>
                    {
                        border.Background = new SolidColorBrush(Color.FromArgb(255, 150, 200, 255));
                    };
            }
            else
            {
                action = () =>
                {
                    border.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                };
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, action);
        }

        public void LightPiece()
        {
            VisualStateManager.GoToState(this, "LightUp", true);
        }

        public string GetInfoAsString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Row.ToString());
            sb.Append(":");
            sb.Append(Col.ToString());
            sb.Append(":");
            sb.Append(NumberValue.ToString());
            sb.Append(":");
            sb.Append(IsPresetPiece);
            sb.Append(";");

            return sb.ToString();
        }

        public void InitFromInfoString(string data)
        {
            string[] splitData = data.Split(new char[] { ':' });
            Row = int.Parse(splitData[0]);
            Col = int.Parse(splitData[1]);
            NumberValue = int.Parse(splitData[2]);
            IsPresetPiece = Boolean.Parse(splitData[3]);
        }

        /// <summary>
        /// Sets the uppper left hand corner suggestion text. This should contain possiblities for the square. An InvalidOperationException is thrown if the NumberValue is -1.
        /// Updates thread on a seperate thread.
        /// </summary>
        /// <param name="suggestions">An array containing all of the suggestions, an exception is thrown if it is larger then 9 in length.</param>
        public async void SetSuggestionText(int[] suggestions)
        {
            // For the suggestion text to be set the number value has to be -1.
            if (NumberValue != -1)
            {
                throw new InvalidOperationException();
            }
            if (suggestions.Length == 0)
            {
                SetSuggestionTextWarning();
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (int i in suggestions)
            {
                sb.Append(i);
                sb.Append(",");
            }

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    SuggestionText.Text = sb.ToString();
                });
        }

        public async void SetSuggestionTextWarning()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                SuggestionText.Text = "None!";
            });
        }

        /// <summary>
        /// Just to clear the suggestion text in the top left hand corner to nothing.
        /// Updates UI on seperate thread.
        /// </summary>
        public async void SetSuggestionTextToNothing()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    SuggestionText.Text = "";
                });
            
        }
    }
}
