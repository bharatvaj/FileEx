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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using Windows.Storage;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.AccessCache;
using System.IO.Compression;
using Windows.Storage.Streams;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Animation;

namespace FileEx
{
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            //var sf = PickFolder();
            //QueryFiles(sf);
            //filesGridView.ItemClick += FilesGridView_ItemClick;

            //createFile.Click += CreateFile_Click;
            //filesGridView.Tapped += async (o, e) =>
            //{
            //var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            //picker.FileTypeFilter.Add("*");

            //Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    this.Frame.Navigate(typeof(VideoPage), file);

            //}
            //    this.Frame.Navigate(typeof(MainPageOld));
            //};

            #region CriticalStartup
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Disabled;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPageOld_BackRequestedAsync;
            //Window.Current.SizeChanged += Current_SizeChanged;
            #endregion
            ContentFrame.Navigate(typeof(FileListingPage));
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadMemoryData();
            ////SettingsTab.Height = Window.Current.Bounds.Width * 2 / 3;
            //PasteFlipView.SelectionChanged += PasteBarSelectionChanged;
            //SubFrame.ContentTransitions = new TransitionCollection { new PaneThemeTransition { Edge = EdgeTransitionLocation.Bottom } };
        }


        #region NotifyHelpers

        /// <summary>
        ///  TODO Show error details when user taps on the notification
        ///  Use this for reference https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/send-local-toast
        /// </summary>
        /// <param name="header"></param>
        /// <param name="msg"></param>
        public void AI(string header, string msg)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = header
                        },

                        new AdaptiveText()
                        {
                            Text = msg
                        }
                    }
                }
            };
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual
            };

            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        #endregion
        private Task LoadMemoryData()
        {
            IStorageItem sf = ApplicationData.Current.LocalFolder;

            var properties = sf.GetBasicPropertiesAsync().Completed += (es, t) =>
            {
                es.GetResults().RetrievePropertiesAsync(new[] { "System.FreeSpace" }).Completed += async (ex, tx) =>
                {
                    var freeSpace = ex.GetResults()["System.FreeSpace"];
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        float freeSpaceGB = (float)Double.Parse((freeSpace.ToString())) / 1000000000;
                        //FreeSpace.Text = Math.Round(freeSpaceGB, 2).ToString();
                        //sdProgbar.Maximum = 16.00;
                        //sdProgbar.Value = 16.00 - freeSpaceGB;
                    });
                };
            };

            return null;
        }


        #region BackButtonHandler

      
        private async void MainPageOld_BackRequestedAsync(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            //if (FileMoveDialog.Visibility == Visibility.Visible)
            //{
            //    CloseDialog(2);
            //}
            //else if (PictureDisplayer.Visibility == Visibility.Visible)
            //{
            //    PictureFlipView.Items.Clear();
            //    PictureDisplayer.Visibility = Visibility.Collapsed;
            //}
            //else if (SubFrameGrid.Visibility == Visibility.Visible)
            //{
            //    CloseDialog(0);
            //}
            //else if (this.SideMenu.IsDrawerOpen == true)
            //{
            //    this.SideMenu.CloseDrawer();
            //}
            //else if (InZipRoot)
            //{
            //    GetFilesAndFolder(CurrentFolder);
            //    InZipRoot = false;
            //}
            //else if (!this.Addresser.Root)
            //{
            //    try
            //    {
            //        int curFoldCharNm = CurrentFolder.Path.LastIndexOf(CurrentFolder.Name);
            //        StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(CurrentFolder.Path.Remove(curFoldCharNm));
            //        this.Addresser.RemoveLast();
            //        GetFilesAndFolder(sf);
            //    }
            //    catch { }
            //}
            //else
            //{
                //OpenDialog();
            //}
        }

        #endregion


        #region HelpMenu
        private void HelpOpen(object sender, RoutedEventArgs e)
        {
            //OpenDialog(0);
        }

        private void HelpPageClose(object sender, RoutedEventArgs e)
        {
            //CloseDialog(0);
        }
        #endregion

        #region NavigationView

        private void NavView_SelectionChanged(NavigationView sender,
                                      NavigationViewSelectionChangedEventArgs args)
        {
            //if (args.IsSettingsSelected == true)
            //{
            //    NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            //}
            //else if (args.SelectedItemContainer != null)
            //{
            //    var navItemTag = args.SelectedItemContainer.Tag.ToString();
            //    NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            //}
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            //Type _page = null;
            //if (navItemTag == "settings")
            //{
            //    _page = typeof(SettingsPage);
            //}
            //else
            //{
            //    var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            //    _page = item.Page;
            //}
            //// Get the page type before navigation so you can prevent duplicate
            //// entries in the backstack.
            //var preNavPageType = ContentFrame.CurrentSourcePageType;

            //// Only navigate if the selected page isn't currently loaded.
            //if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            //{
            //    ContentFrame.Navigate(_page, null, transitionInfo);
            //}
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            //if (SideMenu.IsDrawerOpen == false)
            //{
            //    SideMenu.OpenDrawer();
            //}
            //else
            //{
            //    SideMenu.CloseDrawer();
            //}
        }
        #endregion

    }
}
