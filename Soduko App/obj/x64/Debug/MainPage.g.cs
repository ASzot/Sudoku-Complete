﻿

#pragma checksum "C:\Users\Andrew\Documents\Visual Studio 2012\Projects\Soduko App\Soduko App\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3739472ED3AC6767CB4A059C9B358DF8"
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
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 57 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SettingsButton_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 58 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SodukoSolver_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 59 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.HighscoresButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 60 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerEntered += this.ContinueButton_PointerEntered_1;
                 #line default
                 #line hidden
                #line 60 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerExited += this.ContinueButton_PointerExited_1;
                 #line default
                 #line hidden
                #line 60 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ContinueButton_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 63 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerEntered += this.PlayButton_PointerEntered_1;
                 #line default
                 #line hidden
                #line 63 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerExited += this.PlayButton_PointerExited_1;
                 #line default
                 #line hidden
                #line 63 "..\..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.PlayButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

