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
    partial class SodukoPiece : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 43 "..\..\SodukoPiece.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerEntered += this.border_PointerEntered;
                 #line default
                 #line hidden
                #line 43 "..\..\SodukoPiece.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerExited += this.border_PointerExited;
                 #line default
                 #line hidden
                #line 43 "..\..\SodukoPiece.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Piece_Tapped;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 46 "..\..\SodukoPiece.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Piece_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


