﻿

#pragma checksum "C:\Users\Andrew\Documents\Visual Studio 2012\Projects\Soduko App\Soduko App\SodukoPiece.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "40636C5261A841932EAB88E49AA9C6BA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Soduko_App
{
    partial class SodukoPiece : global::Windows.UI.Xaml.Controls.UserControl
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard RotateStoryboard; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid PieceBackground; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.PlaneProjection RotateTransform; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualStateGroup CommonStates; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState Normal; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState PointerOver; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.VisualState LightUp; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Border border; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock SuggestionText; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock CaptionText; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///SodukoPiece.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            RotateStoryboard = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("RotateStoryboard");
            PieceBackground = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("PieceBackground");
            RotateTransform = (global::Windows.UI.Xaml.Media.PlaneProjection)this.FindName("RotateTransform");
            CommonStates = (global::Windows.UI.Xaml.VisualStateGroup)this.FindName("CommonStates");
            Normal = (global::Windows.UI.Xaml.VisualState)this.FindName("Normal");
            PointerOver = (global::Windows.UI.Xaml.VisualState)this.FindName("PointerOver");
            LightUp = (global::Windows.UI.Xaml.VisualState)this.FindName("LightUp");
            border = (global::Windows.UI.Xaml.Controls.Border)this.FindName("border");
            SuggestionText = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("SuggestionText");
            CaptionText = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("CaptionText");
        }
    }
}


