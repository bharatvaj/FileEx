using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
namespace FileEx
{
    public class Addresser:StackPanel
    {
        public bool root = true;
        public void InitializeComponent()
        {
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            Orientation = Windows.UI.Xaml.Controls.Orientation.Horizontal;
            //StackPanel stk = new StackPanel();
            //stk.Orientation = Windows.UI.Xaml.Controls.Orientation.Horizontal;
            //TextBlock content = new TextBlock();
            //TextBlock txt = new TextBlock();
            //stk.Children.Add(content);
            //stk.Children.Add(txt);
            //prev.Content = stk;

        }

        public bool Root
        {
            get { return root; }
            set { root = value; }
        }

        public void Address(string value, string path)
        {
            Button current = new Button();
            current.BorderThickness = new Thickness(0);
            StackPanel currentStk = new StackPanel();
            currentStk.Orientation = Windows.UI.Xaml.Controls.Orientation.Horizontal;
            TextBlock content = new TextBlock();
            TextBlock txtH = new TextBlock();
            current.Content = currentStk;
            txtH.FontFamily = new FontFamily("Assets/Font/iconFont.ttf#iconfont");
            txtH.Text = ".";
            txtH.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            txtH.FontSize = 10;
            txtH.Foreground = (SolidColorBrush)Application.Current.Resources["SystemAccentColor"];
            //content.FontFamily = new FontFamily("Assets/Font/Custom/Raleway-Light.ttf#Raleway");
            content.TextLineBounds = TextLineBounds.Tight;
            content.TextAlignment = TextAlignment.Center;
            content.Text = value;
            currentStk.Children.Add(content);
            currentStk.Children.Add(txtH);
            current.Background = null;
            current.Height = this.ActualHeight;
            current.MinWidth = 0;
            current.Tag = path;
            Children.Add(current);
            root = false;
        }
        public void Reset()
        {
            Children.Clear();
        }
        public void RemoveLast()
        {
            Children.RemoveAt(Children.Count - 1);
            if (Children.Count == 0) root = true;
            else root = false;
        }
        public void SelectedFolder(int foldersNo)
        {
            for (int i = Children.Count-1; i < (Children.Count-foldersNo-1); i--)
            {
                Children.RemoveAt(i);
            }
        }
        
    }
}
