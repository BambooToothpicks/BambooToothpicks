﻿

#pragma checksum "C:\Users\Logan\Documents\GitHub\BambooToothpicks\BambooToothpicks\BambooToothpicks\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B3F1683FE241422C50B13368D6033466"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace BambooToothpicks
{
    partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private Windows.UI.Xaml.Controls.TextBox txtRssUrl; 
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private Windows.UI.Xaml.Controls.TextBlock TitleText; 
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private Windows.UI.Xaml.Controls.ListView ItemListView; 
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private Windows.UI.Xaml.Controls.TextBlock PostTitleText; 
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private Windows.UI.Xaml.Controls.WebView ContentView; 
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            Application.LoadComponent(this, new System.Uri("ms-appx:///MainPage.xaml"), Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            txtRssUrl = (Windows.UI.Xaml.Controls.TextBox)this.FindName("txtRssUrl");
            TitleText = (Windows.UI.Xaml.Controls.TextBlock)this.FindName("TitleText");
            ItemListView = (Windows.UI.Xaml.Controls.ListView)this.FindName("ItemListView");
            PostTitleText = (Windows.UI.Xaml.Controls.TextBlock)this.FindName("PostTitleText");
            ContentView = (Windows.UI.Xaml.Controls.WebView)this.FindName("ContentView");
        }
    }
}



