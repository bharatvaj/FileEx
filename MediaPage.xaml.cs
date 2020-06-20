using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FileEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPage : Page
    {


        #region MusicVariables
        private bool IsMediaOpen = false;
        SystemMediaTransportControls systemControls;
        DispatcherTimer musicDispatcher = new DispatcherTimer();
        string title = String.Empty;
        string artist = String.Empty;
        #endregion
        public MediaPage()
        {
            this.InitializeComponent();

            #region MediaPlayerDispatcherInitialization
            musicDispatcher.Interval = TimeSpan.FromMilliseconds(1000);

            mediaPlayer.MediaFailed += async (s, t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                {
                    UpdateMusic("no media", "", TimeSpan.FromSeconds(0));
                    musicSlider.Value = 0;
                    playPause.Content = "5";
                    currentDuration.Text = "--:--";
                    mediaDuration.Text = "--:--";
                    if (musicDispatcher.IsEnabled)
                        musicDispatcher.Stop();
                });
            };

            mediaPlayer.MediaEnded += async (s, t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                {
                    UpdateMusic("no media", "", mediaPlayer.NaturalDuration.TimeSpan);
                    musicSlider.Value = 0;
                    playPause.Content = "5";
                    currentDuration.Text = "--:--";
                    mediaDuration.Text = "--:--";
                    musicDispatcher.Stop();
                });
            };
            musicDispatcher.Tick += (s, t) =>
            {
                musicSlider.Value = mediaPlayer.Position.TotalSeconds;
                currentDuration.Text = mediaPlayer.Position.Minutes + ":" + mediaPlayer.Position.Seconds;

            };
            #endregion


            #region MusicPlayerInitialization


            // Hook up app to system transport controls.

            systemControls = SystemMediaTransportControls.GetForCurrentView();

            systemControls.ButtonPressed += async (sender, args) =>
            {
                switch (args.Button)

                {

                    case SystemMediaTransportControlsButton.Play:

                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>

                        {
                            mediaPlayer.Play();

                        });

                        break;

                    case SystemMediaTransportControlsButton.Pause:
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>

                        {

                            mediaPlayer.Pause();

                        });

                        break;

                    default:

                        break;

                }
            };

            // Register to handle the following system transpot control buttons.

            systemControls.IsPlayEnabled = true;

            systemControls.IsPauseEnabled = true;
            #endregion

        }
        #region AudioMenuHandler
        private void RepeatButtonChecked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.IsLooping = true;
        }

        private void RepeatButtonUnchecked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.IsLooping = false;
        }
        #endregion

        private void PlayNext(object sender, RoutedEventArgs e)
        {

        }

        //private async void HideStatusBar()
        //{
        //StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

        // Hide the status bar
        //  await statusBar.HideAsync();
        //}

        #region Music
        #region MusicEvents
        //private void MusicSegmentChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    //    if (!(mediaPlayer.CurrentState == MediaPlayerState.Closed))
        //    //    {
        //    //        if (mediaPlayer.CanSeek)
        //    //        {
        //    //            mediaPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        musicSlider.Value = 0;
        //    //    }
        //}
        internal async void PlayMedia(IStorageFile selectedItem)
        {

            mediaPlayer.Source = new Uri(selectedItem.Path);

            mediaPlayer.MediaOpened += async (s, t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, delegate
                {
                    musicSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    playPause.Content = "8";
                    UpdateMusic(title, artist, mediaPlayer.NaturalDuration.TimeSpan);
                    musicDispatcher.Start();
                });
            };
            mediaPlayer.Play();

            #region TagLib AlbumArt
            //            Tag tags = null;
            //            var fileStream = await selectedItem.OpenStreamForReadAsync();
            //            try
            //            {
            //                var tagFile = TagLib.File.Create(new StreamFileAbstraction(selectedItem.Name, fileStream, fileStream));
            //                if (selectedItem.FileType == ".mp3" || selectedItem.FileType == ".wma")
            //                    tags = tagFile.GetTag(TagTypes.Id3v2);
            //                else if (selectedItem.FileType == ".m4a")
            //                    tags = tagFile.GetTag(TagLib.TagTypes.MovieId);
            //                try
            //                {
            //                    title = tags.Title;
            //                }
            //                catch (NullReferenceException)
            //                {
            //                    title = selectedItem.Name;
            //                }
            //                try
            //                {
            //                    artist = String.Concat(tags.Performers);
            //                }
            //                catch (NullReferenceException)
            //                {
            //                    artist = String.Empty;
            //                }
            //                if (tags.Pictures != null)
            //                {
            //                    try
            //                    {
            //                        MemoryStream ms = new MemoryStream(tags.Pictures[0].Data.Data);
            //                        WriteableBitmap wm = null;
            //                        var wwmV = wm.FromStream(ms, Windows.Graphics.Imaging.BitmapPixelFormat.Unknown);
            //                        await wwmV.ContinueWith(delegate
            //                        {
            //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //                            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //                            {
            //                                ms.Dispose();
            //                                ImageBrush im = new ImageBrush();
            //                                im.ImageSource = wwmV.Result;
            //                                im.Stretch = Stretch.UniformToFill;
            //                                musicArt.Visibility = Visibility.Collapsed;
            //                                imageContainerMedia.Background = im;
            //                            });
            //                        });
            //                    }

            //                    catch (IndexOutOfRangeException)
            //                    {
            //                        imageContainerMedia.Background = (SolidColorBrush)App.Current.Resources["PhoneChromeBrush"];
            //                        musicArt.Visibility = Visibility.Visible;
            //                    }
            //                    catch (UnauthorizedAccessException)
            //                    {
            //                    }
            //                }
            //                else
            //                {
            //                    imageContainerMedia.Background = (SolidColorBrush)App.Current.Resources["PhoneChromeBrush"];
            //                    musicArt.Visibility = Visibility.Visible;
            //                }
            //            }
            //            catch
            //            {
            //                imageContainerMedia.Background = (SolidColorBrush)App.Current.Resources["PhoneChromeBrush"];
            //                musicArt.Visibility = Visibility.Visible;
            //            }
            #endregion
        }



        private void UpdateMusic(string songName, string songArtist, TimeSpan duration)
        {
            try
            {
                SongName.Text = songName;
                SongArtist.Text = songArtist;
                mediaDuration.Text = duration.Minutes + ":" + duration.Seconds;
            }
            catch
            {
                SongName.Text = "no media";
                SongArtist.Text = "";
                mediaDuration.Text = "--:--";
            }
        }
        #endregion

        #region MusicMenu
        private void PlayCurrent(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.CurrentState == MediaElementState.Playing)
            {
                mediaPlayer.Pause();
                playPause.Content = "5";
                DurationBlink.Begin();
            }
            else if (mediaPlayer.CurrentState == MediaElementState.Paused)
            {
                mediaPlayer.Play();
                playPause.Content = "8";
                DurationBlink.Stop();
            }

            else if (mediaPlayer.CurrentState == MediaElementState.Stopped)
            {
                KnownFolders.MusicLibrary.GetFilesAsync().Completed += (es, st) =>
                {
                    mediaPlayer.Source = new Uri(es.GetResults().First().Path);
                    mediaPlayer.Play();
                    playPause.Content = "8";
                };

            }

        }
        #endregion


        #endregion

    }
}
