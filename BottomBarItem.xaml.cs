using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FileEx
{
    public sealed partial class BottomBarItem : Button
    {
        public BottomBarItem()
        {
            this.InitializeComponent();
            MinWidth = 5;
            MinHeight = 5;
            Loaded += BottomBarItem_Loaded;
        }

        public void Initialize()
        {
            if (IsVertical())
            {
                contentText.FontSize = Window.Current.Bounds.Width / 35;
                imageText.FontSize = Window.Current.Bounds.Width / 20;
            }
            else
            {
                contentText.FontSize = Window.Current.Bounds.Height / 35; imageText.FontSize = Window.Current.Bounds.Height / 20;
            }
        }

        public string ImageText
        {
            get { return imageText.Text; }
            set { imageText.Text = value; }
        }
        public Brush ImageColor
        {
            get { return imageText.Foreground; }
            set { imageText.Foreground = value; }
        }
        public Brush ContentColor
        {
            get { return contentText.Foreground; }
            set { contentText.Foreground = value; }
        }
        public string ContentText
        {
            get { return contentText.Text; }
            set { contentText.Text = value; }
        }
        void BottomBarItem_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            //Initialize();
        }
        public bool IsVertical()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width) return true;
            else return false;
        }
    }
}
