﻿

#pragma checksum "C:\Users\Andrew\Documents\Visual Studio 2012\Projects\Soduko App\Soduko App\Pages\GamePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "85E20C4EFAF8B1081E02A75B822719C3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Soduko_App.Pages
{
    partial class GamePage : global::Soduko_App.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 25 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.MainGrid_Loaded_1;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 100 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ForfeitButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 84 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ResetButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 68 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.HintModeComboBox_SelectionChanged_1;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 54 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.PuzzleCanvas_Loaded_1;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 45 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.LoadingBar_Loaded_1;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 38 "..\..\..\Pages\GamePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


