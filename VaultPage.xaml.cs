using System;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


namespace FileEx
{
    public sealed partial class VaultPage : Page
    {
        App app = (App)Application.Current;

        string PASS_KEY, VER_PASS;

        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        public VaultPage()
        {
            this.InitializeComponent();
            PASS_KEY = (string)settings.Values["passkey"];
            //Loaded += VaultPage_Loaded;
        }

        private void VaultPage_Loaded(object sender, RoutedEventArgs e)
        {
            passwordBox.Focus(FocusState.Pointer);
            //int btNo = 0;
            //for (int i = 0; i <= 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        btNo++;
            //        Button bt = new Button();
            //        bt.Content = btNo;
            //        bt.HorizontalAlignment = HorizontalAlignment.Stretch;
            //        bt.VerticalAlignment = VerticalAlignment.Stretch;
            //        bt.Click += (o, t) =>
            //        {
            //            passwordBox.Text += bt.Content;
            //        };
            //        Grid.SetRow(bt, i);
            //        Grid.SetColumn(bt, j);
            //        passwordGrid.Children.Add(bt);
            //    }
            //}
        }

        private void passwordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passwordBox.Text.Count() < 4) return;
            if (PASS_KEY == String.Empty)
            {
                PASS_KEY = passwordBox.Text;
                //add password added animation
            }
            if (passwordBox.Text != PASS_KEY)
            {
                AuthenticationFailed.Begin();
                passwordBox.Text = "";
                return;
            }
            //app.mainPage.PersonalStorageEnumerator();
            //app.mainPage.CloseDialog(0);
            //((Grid)(((Grid)this.Frame.Parent).Parent)).Visibility = Visibility.Collapsed;
            //this.Frame.Content = null;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            passwordBox.Focus(FocusState.Pointer);
        }

        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //((Grid)(((Grid)this.Frame.Parent).Parent)).Visibility = Visibility.Collapsed;
            //this.Frame.Content = null;

        }
    }
}
