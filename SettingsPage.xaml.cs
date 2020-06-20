using System;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FileEx
{
    public sealed partial class SettingsPage : Page
    {

        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += SettingsPage_Loaded;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += SettingsPage_BackRequested;
        }

        private void SettingsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {

            #region SettingsSerializer
            if (settings.Containers.Count == 9)
            {
                settings.Values["musicPlayer"] = "1";
                musicToggle.IsChecked = true;
                settings.Values["picturesPlayer"] = "1";
                picturesToggle.IsChecked = true;
                settings.Values["videoPlayer"] = "1";
                videosToggle.IsChecked = true;
                settings.Values["ebookViewer"] = "1";
                ebookToggle.IsChecked = true;
                settings.Values["itemType"] = 1;
                typeComboBox.SelectedIndex = 1;
                settings.Values["musThumbnail"] = "0";
                musThumbnailToggle.IsChecked = false;
                settings.Values["vidThumbnail"] = "0";
                vidThumbnailToggle.IsChecked = false;
                settings.Values["picThumbnail"] = "0";
                picThumbnailToggle.IsChecked = false;
            }
            else
            {
                musicToggle.IsChecked = ((string)settings.Values["musicPlayer"] == "1");
                picturesToggle.IsChecked = ((string)settings.Values["picturesPlayer"] == "1");
                videosToggle.IsChecked = ((string)settings.Values["videosPlayer"] == "1");
                ebookToggle.IsChecked = ((string)settings.Values["ebookViewer"] == "1");
                typeComboBox.SelectedIndex = (int)(settings.Values["itemType"]);
                musThumbnailToggle.IsChecked = ((string)settings.Values["musThumbnail"] == "1");
                vidThumbnailToggle.IsChecked = ((string)settings.Values["vidThumbnail"] == "1");
                picThumbnailToggle.IsChecked = ((string)settings.Values["picThumbnail"] == "1");
            }
            #endregion


            #region InitializePasswordButtons
            for (int i = 1; i <= 9; i++)
            {
                Button b = new Button();
                b.Content = i;
                try
                {
                    b.Height = (Window.Current.Bounds.Width / 3) - 40;
                    b.Width = Window.Current.Bounds.Width / 3;
                }
                catch
                {
                }
                b.Click += B_Click;
                wrapPanel.Children.Add(b);
            }
            #endregion
        }


        #region SettingsHandler
        #region Bluetooth
        private async void OpenBluetoothSettings(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }
        #endregion

        #region HelpMenu
        private void HelpOpen(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HelpPage));
        }
        #endregion
        #region FTP
        private void OpenFTP(object sender, RoutedEventArgs e)
        {
            //FTPOpen.Begin();
        }
        #endregion
        #region Music
        private void musicToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["musicPlayer"] = "1";
        }
        private void musicToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["musicPlayer"] = "0";
        }

        private void musThumbnailToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["musThumbnail"] = "1";
        }

        private void musThumbnailToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["musThumbnail"] = "0";
        }
        #endregion
        #region Videos
        private void videosToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["videosPlayer"] = "1";
        }
        private void videosToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["videosPlayer"] = "0";
        }
        private void vidThumbnailToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["vidThumbnail"] = "1";
        }

        private void vidThumbnailToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["vidThumbnail"] = "0";
        }

        #endregion
        #region Ebooks
        private void ebookToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["ebookViewer"] = "1";
        }
        private void ebookToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["ebookViewer"] = "0";
        }
        #endregion
        #region Pictures
        private void picturesToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["picturesPlayer"] = "1";
        }

        private void picturesToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["picturesPlayer"] = "0";
        }

        private void picThumbnailToggle_Checked(object sender, RoutedEventArgs e)
        {
            settings.Values["picThumbnail"] = "1";
        }

        private void picThumbnailToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            settings.Values["picThumbnail"] = "0";
        }
        #endregion

        #region Folder
        private void FolderViewChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.Values["itemType"] = typeComboBox.SelectedIndex;
        }
        #endregion

        #region Security
        private void PasswordToggle(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = (ToggleSwitch)sender;
            if (ts.IsOn)
            {
                wrapPanel.Visibility = Visibility.Visible;
            }
            else
            {
                wrapPanel.Visibility = Visibility.Visible;
            }

        }
        private void B_Click(object sender, RoutedEventArgs e)
        {
            securityBlock.Text += ((Button)sender).Content;
        }
        #endregion
        #endregion

    }
}
