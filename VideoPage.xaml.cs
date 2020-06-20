using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace FileEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPage : Page
    {

        private bool MENU_HIDDEN = true;
        private DispatcherTimer musicDispatcher;
        public VideoPage()
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped | DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;
            this.InitializeComponent();
            Loaded += VideoPage_Loaded;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += VideoPage_BackRequested;
        }

        private void VideoPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
            else Application.Current.Exit();
        }

        private void VideoPage_Loaded(object sender, RoutedEventArgs e)
        {
            MenuHide.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IStorageFile isf = (IStorageFile)e.Parameter;
            mediaPlayer.Source = new Uri(isf.Path);
            mediaPlayer.MediaOpened += (es, st) =>
            {
                UpdateVideoInfo(isf.Name, mediaPlayer.NaturalDuration.TimeSpan.Minutes + ":" + mediaPlayer.NaturalDuration.TimeSpan.Seconds.ToString(), "8");
                musicSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                musicDispatcher.Start();
            };

            musicDispatcher = new DispatcherTimer();
            musicDispatcher.Interval = TimeSpan.FromMilliseconds(1000);


            mediaPlayer.MediaEnded += async (es, st) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                {
                    //UpdateVideoInfo("no media", "--:--", "5");
                    //musicDispatcher.Stop();
                    ExitOrGoBack();
                });
            };

            mediaPlayer.MediaFailed += async (s, t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async delegate
                {
                    //UpdateVideoInfo("no media", "--:--", "5");
                    if (musicDispatcher.IsEnabled)
                        musicDispatcher.Stop();
                    MessageBox mb = new MessageBox();
                    mb.HeadingText = "Cannot Play Media";
                    mb.ContentText = "Cannot play this file, do you want to open with other video player?";
                    mb.LeftButtonHandler += (es, st) =>
                    {
                        ExitOrGoBack();
                    };
                    mb.RightButtonHandler += async (es, st) =>
                    {
                        await Windows.System.Launcher.LaunchFileAsync(isf);
                    };
                    mb.Show();
                });
            };


            musicDispatcher.Tick += (s, t) =>
            {
                musicSlider.Value = mediaPlayer.Position.TotalSeconds;
                currentDuration.Text = mediaPlayer.Position.Minutes + ":" + mediaPlayer.Position.Seconds;

            };
            videoName.Text = isf.Name;

        }

        private void ExitOrGoBack()
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
            else Application.Current.Exit();
        }

        private void UpdateVideoInfo(string name, string dur, string playMode)
        {
            videoName.Text = name;
            mediaDuration.Text = dur;
            musicSlider.Value = 0;
            playPause.Content = playMode;
        }

        private void mediaPlayer_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {

        }

        private void mediaPlayer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }

        private void mediaPlayer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }

        private void PlayCurrent(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.CurrentState == MediaElementState.Playing)
            {
                mediaPlayer.Pause();
                playPause.Content = "5";
                //DurationBlink.Begin();
            }
            else if (mediaPlayer.CurrentState == MediaElementState.Paused)
            {
                mediaPlayer.Play();
                playPause.Content = "8";
                //DurationBlink.Stop();
            }

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (MENU_HIDDEN)
                MenuHideRev.Begin();
            else MenuHide.Begin();
            MENU_HIDDEN = !MENU_HIDDEN;
        }
    }
}
