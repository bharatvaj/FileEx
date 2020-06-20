using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace File360
{
    public class Dialog : Grid
    {
        Storyboard OpenDialog = new Storyboard();
        Storyboard CloseDialog = new Storyboard();
        public Dialog()
        {
            Height = Window.Current.Bounds.Height;
            Width = Window.Current.Bounds.Width;
            Window.Current.SizeChanged += CurrentSizeChanged;
            Background = new SolidColorBrush((Color)Application.Current.Resources["ContentDialogDimmingColor"]);
            FadeInThemeAnimation fadeInAnimation = new FadeInThemeAnimation();

            Storyboard.SetTarget(fadeInAnimation, this);
            OpenDialog.Children.Add(fadeInAnimation);


            FadeOutThemeAnimation fadeOutAnimation = new FadeOutThemeAnimation();

            Storyboard.SetTarget(fadeOutAnimation, this);
            CloseDialog.Children.Add(fadeOutAnimation);

        }
        
        public bool IsOpen
        {
            get 
            {
                if (Visibility == Visibility.Collapsed)
                    return false;
                else return true;
            }
            set
            {
                if (value == true)
                {
                    Visibility = Visibility.Visible;
                    OpenDialog.Begin();
                }
                else
                {

                    CloseDialog.Begin();
                    CloseDialog.Completed += (s, f) =>
                    {
                        Visibility = Visibility.Collapsed;
                    };
                }
            }
        }

        private void CurrentSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            Height = Window.Current.Bounds.Height;
            Width = Window.Current.Bounds.Width;
            this.UpdateLayout();
        }
    }
}
