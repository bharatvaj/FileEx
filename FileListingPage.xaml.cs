using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
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
    public sealed partial class FileListingPage : Page
    {

        #region Variables
        StorageFolder storageFolder;
        ObservableCollection<IStorageItem> FileItems = new ObservableCollection<IStorageItem>();
        public bool GridType = true;

        #region SettingsVaribles
        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        StorageFolder folder;
        IReadOnlyList<IStorageItem> Items;

        ObservableCollection<FileModel> sd = new ObservableCollection<FileModel>();

        public IStorageFolder CurrentFolder = null;
        bool InZipRoot = false;

        List<IStorageItem> clipboard = new List<IStorageItem>();

        //bool singleTap = true;

        App app = (App)Application.Current;

        #region FileOperationVariables
        bool IS_FILE_MOVE = false;
        #endregion
        #endregion

        #endregion

        public FileListingPage()
        {
            this.InitializeComponent();
            storageFolder = ApplicationData.Current.LocalFolder;
            #region SettingsSerializer
            if (settings.Values.Count == 0)
            {
                settings.Values["userName"] = "User";
                //app.UserName = (string)settings.Values["userName"];
                settings.Values["itemType"] = 1;
                GridType = (int)settings.Values["itemType"] == 1;

                settings.Values["musicPlayer"] = "1";
                settings.Values["picturesPlayer"] = "1";
                settings.Values["videoPlayer"] = "1";
                settings.Values["ebookViewer"] = "1";
                settings.Values["musThumbnail"] = "0";
                settings.Values["vidThumbnail"] = "0";
                settings.Values["picThumbnail"] = "0";

                settings.Values["passkey"] = String.Empty;
            }
            else
            {
                //app.UserName = (string)settings.Values["userName"];
                GridType = (int)settings.Values["itemType"] == 1;
            }
            ChangeTemplate();
            #endregion
            Loaded += FileListingPage_Loaded;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                CurrentFolder = e.Parameter as IStorageFolder;
            }
        }
        #region DialogOpenClose
        public void OpenDialog(int dialogNo)
        {
            ProgressDialog.Visibility = Visibility.Visible;
            switch (dialogNo)
            {
                case 0:
                    SubFrameGrid.Visibility = Visibility.Visible;
                    SubFrame.Navigate(typeof(HelpPage));
                    SubFrameHeader.Text = "HELP";
                    break;

                case 1:
                    SubFrameGrid.Visibility = Visibility.Visible;
                    SubFrame.Navigate(typeof(VaultPage));
                    SubFrameHeader.Text = "VAULT";
                    break;
                case 2:
                    FileMoveDialog.Visibility = Visibility.Visible;
                    break;
                case 3:
                    ProgInfoView.Visibility = Visibility.Visible;
                    break;

            }
        }


        public void CloseDialog(int dialogNo)
        {
            ProgressDialog.Visibility = Visibility.Collapsed;
            switch (dialogNo)
            {
                case 0:
                    SubFrameGrid.Visibility = Visibility.Collapsed;
                    break;

                case 1:
                    SubFrameGrid.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    FileMoveDialog.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    ProgInfoView.Visibility = Visibility.Collapsed;
                    break;

            }
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

        private void CloseDialog(object sender, RoutedEventArgs e)
        {
            CloseDialog(2);
        }
        private void AI(String name, String exception)
        {
            ///
        }

        private void FileListingPage_Loaded(object sender, RoutedEventArgs e)
        {

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Disabled;

            SDGridView.ItemsSource = sd;

            this.Addresser.InitializeComponent();
            try
            {
                GetFilesAndFolder(CurrentFolder);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            //GetSDRoot();
        }

        /// <summary>
        /// TODO make async and return storagaeitems array
        /// </summary>
        /// <param name="storageFolder"></param>
        void QueryFiles(StorageFolder storageFolder)
        {
            FileItems.Clear();
            //FileItems.Add(storageFolder.GetParentAsync().GetResults());
            storageFolder.GetItemsAsync().Completed += async (i, o) =>
            {
                foreach (IStorageItem el in i.GetResults())
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        FileItems.Add(el);
                    });
                }
            };
        }
        /// <summary>
        ///  TODO make async
        /// </summary>
        /// <returns></returns>
        public StorageFolder PickFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            return folderPicker.PickSingleFolderAsync().GetResults();
        }

        #region MainGrid
        public void CloseDrawer()
        {
            //SideMenu.CloseDrawer();
        }

        private void ChangeTemplate()
        {
            if (!GridType)
            {
                SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["ListViewItemsPanel"];
                SDGridView.ItemTemplate = (DataTemplate)Resources["ListFoldersView"];
                gridList.ContentText = "list";
                gridList.ImageText = "b";
                settings.Values["itemType"] = 0;
                GridType = false;
            }
            else
            {
                SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["GridViewItemsPanel"];
                SDGridView.ItemTemplate = (DataTemplate)Resources["GridFoldersView"];
                gridList.ContentText = "grid";
                gridList.ImageText = "N";
                settings.Values["itemType"] = 1;
                GridType = true;
            }
        }

        private async void GetSDRoot()
        {

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("FolderTokenSettingsKey"))
            {
                string token = (string)ApplicationData.Current.LocalSettings.Values["FolderTokenSettingsKey"];
                // if we do, use it to get the StorageFolder instance
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                this.Addresser.Reset();
                try
                {
                    GetFilesAndFolder(folder);
                    RootFolder.Text = "3";
                }
                catch (Exception ex)
                {
                    AI("no sdcard is detected", "2");
                }
            }
        }


        #region UserFolderCommunication


        private void Reset()
        {
            sd.Clear();
        }


        #endregion
        #region OrientationHelpers
        /// <summary>
        /// TODO check if this logic is needed
        /// </summary>
        /// <returns></returns>
        //private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        //{
        //    //var b = Window.Current.Bounds;
        //    VisualStateManager.GoToState(this, "Portrait", false);
        //}
        public bool IsVertical()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width) return true;
            else return false;
        }
        #endregion
        #endregion


        #region DeviceStorageEnumerators
        private async void SDCardEnumerator(object sender, RoutedEventArgs e)
        {
            StorageFolder folder;
            // check if we already have a token
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SDCardKey"))
            {
                string token = (string)ApplicationData.Current.LocalSettings.Values["SDCardKey"];
                // if we do, use it to get the StorageFolder instance
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                this.Addresser.Reset();
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, delegate
                {
                    headerText.Text = "SD Card";
                    RootFolder.Text = "3";
                    //SideMenu.CloseDrawer();
                });
                GetFilesAndFolder(folder);
            }
            else
            {
                // if we don't, this is the first time the code is being executed; user has to give
                // us a consent to use the folder
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.FileTypeFilter.Add("*");
                folderPicker.PickSingleFolderAsync();
            }
        }


        private void PhoneEnumerator(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.PickSingleFolderAsync();
        }
        #endregion

        #region FolderPicker


        //public void ContinueFolderPicker(FolderPickerContinuationEventArgs args)
        public void ContinueFolderPicker(IStorageFolder folder)
        {
            if (folder != null)
            {
                try
                {
                    if (folder != null)
                    {
                        if (folder.IsOfType(StorageItemTypes.Folder))
                        {
                            this.Addresser.Reset();
                        }
                        // after that, we can store the folder for future reuse
                        string pickedFolderToken = StorageApplicationPermissions.FutureAccessList.Add(folder);
                        ApplicationData.Current.LocalSettings.Values.Add("FolderTokenSettingsKey", pickedFolderToken);
                        StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                        string mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(folder);
                        headerText.Text = "SD Card";
                        RootFolder.Text = "3";
                        if (GridType)
                        {
                            SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["GridViewItemsPanel"];
                            SDGridView.ItemTemplate = (DataTemplate)Resources["GridFoldersView"];
                        }
                        else
                        {
                            SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["ListViewItemsPanel"];
                            SDGridView.ItemTemplate = (DataTemplate)Resources["ListFoldersView"];
                        }
                        GetFilesAndFolder(folder);
                    }
                    else
                    {
                        return;
                    }
                }
                catch
                { return; }
            }
        }
        #endregion

        #region BottomBarmenuEventHandlers
        private void GridListChange(object sender, RoutedEventArgs e)
        {
            if (GridType)
            {
                SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["ListViewItemsPanel"];
                SDGridView.ItemTemplate = (DataTemplate)Resources["ListFoldersView"];
                gridList.ContentText = "list";
                gridList.ImageText = "b";
                GridType = false;
            }
            else
            {
                SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["GridViewItemsPanel"];
                SDGridView.ItemTemplate = (DataTemplate)Resources["GridFoldersView"];
                gridList.ContentText = "grid";
                gridList.ImageText = "N";
                GridType = true;
            }
        }
        private void SelectAll(object sender, RoutedEventArgs e)
        {
            MultipleSelection(true);
            SDGridView.SelectAll();
            //folderHold.Begin();
        }
        private void selectBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MultipleSelection(false);
        }
        #endregion
        #region FileOperations
        private void AddFile(object sender, RoutedEventArgs e)
        {
            if (CurrentFolder == null) return;
            CurrentFolder.CreateFileAsync("New File", CreationCollisionOption.GenerateUniqueName).Completed += async (es, t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                {
                    sd.Add(new FileModel("New File", "Q"));
                });
            };
        }
        private void AddZip(object sender, RoutedEventArgs e)
        {
            //InputPrompt ip = new InputPrompt();
            //ip.Completed += (async (ex, t) =>
            //{
            //    StorageFile sf = await CurrentFolder.CreateFileAsync(t.Result + ".zip", CreationCollisionOption.GenerateUniqueName);
            //    if (sf != null && CreateZip(sf).IsCompleted)
            //    {

            //        GetFilesAndFolder(CurrentFolder);
            //    }
            //});
            //ip.Title = "Enter file name";
            //ip.Show();
        }

        private async void AddFolder(object sender, RoutedEventArgs e)
        {
            if (CurrentFolder == null) return;
            sd.Add(new FileModel("New Folder", "f"));
            StorageFolder sf = await CurrentFolder.CreateFolderAsync("New Folder", CreationCollisionOption.GenerateUniqueName);
        }
        private async void DeleteItem(object sender, RoutedEventArgs e)
        {


            foreach (var item in SDGridView.SelectedItems)
            {
                try
                {

                    FileModel sdli = (FileModel)item;
                    IStorageItem selectedFile = Items.ElementAt(sd.IndexOf(sdli));
                    await selectedFile.DeleteAsync();
                    sd.Remove(sdli);
                }
                catch (Exception ex)
                {
                    AI("Cannot Delete File", "Cannot delete file because the file may be locked");
                }
            }
            //GetFilesAndFolder(CurrentFolder);
        }

        private async void RenameItem(object sender, RoutedEventArgs e)
        {

            foreach (var item in SDGridView.SelectedItems)
            {
                try
                {
                    FileModel sdli = (FileModel)item;
                    IStorageItem selectedFile = Items.ElementAt(sd.IndexOf(sdli));
                    await selectedFile.RenameAsync("Renamed Folder", NameCollisionOption.GenerateUniqueName);
                    sdli.Name = "New Folder";
                }
                catch (Exception ex)
                {
                    AI("Cannot Rename File", "Cannot rename file because the file may be locked");
                }
            }

            //WinRTXamlToolkit.Controls.InputDialog id = new WinRTXamlToolkit.Controls.InputDialog();

            //id.GotFocus += (s, ex) =>
            //{
            //    try
            //    {
            //        id.Select(0, 3).Text.LastIndexOf('.'));
            //    }
            //    catch
            //    {
            //        txtBoxRe.SelectAll();
            //    }
            //};
            //id.AcceptButton = "Rename";
            //id.CancelButton = "Cancel";

            //id.KeyDown += async (s, ex) =>
            //{
            //    if (ex.Key == Windows.System.VirtualKey.Enter)
            //    {
            //        if (txtBoxRe.Text == String.Empty)
            //        {
            //            txtBoxRe.Visibility = Visibility.Collapsed;
            //            txtBlockRe.Visibility = Visibility.Visible;
            //        }
            //        else if (txtBoxRe.Text.Contains("/") || txtBoxRe.Text.Contains(@"\"))
            //        {
            //            AI("Cannot rename File or Folder", @"A file or folder can't contain any of the following characters: \ / : * ? " + "\"" + " < > |");
            //        }
            //        else
            //        {
            //            try
            //            {
            //                txtBlockRe.Text = txtBoxRe.Text;
            //                txtBoxRe.Visibility = Visibility.Collapsed;
            //                txtBlockRe.Visibility = Visibility.Visible;

            //            }
            //            catch
            //            {
            //                AI("Rename operation failed", "Cannot rename this file at the moment");
            //            }
            //            GetFilesAndFolder(CurrentFolder);
            //        }
            //    }
            //};

            //var stk = itemControl.GetLogicalChildrenByType<TextBlock>(true);
            //var textBlock = stk.ElementAt(1);
            //var sdl = await CurrentFolder.GetItemsAsync();
            //int index = SDGridView.Items.IndexOf(sdli);

        }

        private void CutItems(object sender, RoutedEventArgs e)
        {
            clipboard.Clear();
            var sdl = SDGridView.SelectedItems;
            foreach (var sdli in sdl)
            {
                try
                {
                    var si = (FileModel)sdli;
                    IStorageItem selectedItem = Items.ElementAt(sd.IndexOf(si));
                    clipboard.Add(selectedItem);
                }
                catch (Exception ex)
                {
                    AI("Cannot add to clipboard", ex.Message);
                }

            }
        }


        #region Paste
        private void PasteItem(object sender, RoutedEventArgs e)
        {
            OpenDialog(2);
        }
        private void OverwriteContents(object sender, RoutedEventArgs e)
        {
            FileMoveDialog.Visibility = Visibility.Collapsed;
            ProgInfoView.Visibility = Visibility.Visible;
            int clipCount = 0;
            if ((clipCount = clipboard.Count) != 0)
            {
                progFileDest.Text = CurrentFolder.Name;
                progTotalSize.Text = clipCount.ToString();
                if (IS_FILE_MOVE)//move
                {
                    cmstatus.Text = "moving";
                    try
                    {
                        foreach (var item in clipboard)
                        {
                            progFileSource.Text = item.Name;
                            progCurrentSize.Text = clipboard.IndexOf(item).ToString();
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var tb = ((IStorageFile)item).MoveAsync(CurrentFolder, item.Name, NameCollisionOption.ReplaceExisting);
                                tb.Completed += async (i, s) =>
                                {
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                                    {
                                        ProgInfoView.Visibility = Visibility.Collapsed;
                                        ProgressDialog.Visibility = Visibility.Collapsed;
                                    });
                                };
                            }
                        }
                        GetFilesAndFolder(CurrentFolder);
                    }
                    catch (Exception ex)
                    {
                        AI("Error copying file", ex.Message);
                    }
                }
                else//copy
                {
                    cmstatus.Text = "copying";
                    try
                    {
                        foreach (var item in clipboard)
                        {
                            progFileSource.Text = item.Name;
                            progCurrentSize.Text = clipboard.IndexOf(item).ToString();
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var tb = ((IStorageFile)item).CopyAsync(CurrentFolder, item.Name, NameCollisionOption.ReplaceExisting);
                                tb.Completed += async (i, s) =>
                                {
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                                    {
                                        ProgInfoView.Visibility = Visibility.Collapsed;
                                        ProgressDialog.Visibility = Visibility.Collapsed;
                                    });
                                };
                            }
                        }
                        GetFilesAndFolder(CurrentFolder);
                    }
                    catch (Exception ex)
                    {
                        AI("Error copying file", ex.Message);
                    }
                }
            }
        }

        private void KeepBothFiles(object sender, RoutedEventArgs e)
        {
            FileMoveDialog.Visibility = Visibility.Collapsed;
            ProgInfoView.Visibility = Visibility.Visible;
            int clipCount = 0;
            if ((clipCount = clipboard.Count) != 0)
            {
                progFileDest.Text = CurrentFolder.Name;
                progTotalSize.Text = clipCount.ToString();
                if (IS_FILE_MOVE)//move
                {
                    cmstatus.Text = "moving";
                    try
                    {
                        foreach (var item in clipboard)
                        {
                            progFileSource.Text = item.Name;
                            progCurrentSize.Text = clipboard.IndexOf(item).ToString();
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var tb = ((IStorageFile)item).MoveAsync(CurrentFolder, item.Name, NameCollisionOption.GenerateUniqueName);
                                tb.Completed += async (i, s) =>
                                {
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                                    {
                                        ProgInfoView.Visibility = Visibility.Collapsed;
                                        ProgressDialog.Visibility = Visibility.Collapsed;
                                    });
                                };
                            }
                        }
                        GetFilesAndFolder(CurrentFolder);
                    }
                    catch (Exception ex)
                    {
                        AI("Error copying file", ex.Message);
                    }
                }
                else//copy
                {
                    cmstatus.Text = "copying";
                    try
                    {
                        foreach (var item in clipboard)
                        {
                            progFileSource.Text = item.Name;
                            progCurrentSize.Text = clipboard.IndexOf(item).ToString();
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var tb = ((IStorageFile)item).CopyAsync(CurrentFolder, item.Name, NameCollisionOption.GenerateUniqueName);
                                tb.Completed += async (i, s) =>
                                {
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                                    {
                                        ProgInfoView.Visibility = Visibility.Collapsed;
                                        ProgressDialog.Visibility = Visibility.Collapsed;
                                    });
                                };
                            }
                        }
                        GetFilesAndFolder(CurrentFolder);
                    }
                    catch (Exception ex)
                    {
                        AI("Error copying file", ex.Message);
                    }
                }
            }
        }
        #endregion


        private async void DeleteThisFolder(object sender, RoutedEventArgs e)
        {
            var parent = await (CurrentFolder as StorageFolder).GetParentAsync();
            /// TODO
            //await CurrentFolder.DeleteAsync();
            CurrentFolder = parent;
            GetFilesAndFolder(parent);
        }


        private async void PreviousFolder(object sender, RoutedEventArgs e)
        {
            if (!this.Addresser.Root)
            {
                try
                {
                    int curFoldCharNm = CurrentFolder.Path.LastIndexOf(CurrentFolder.Name);
                    StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(CurrentFolder.Path.Remove(curFoldCharNm));
                    if (sf != null)
                    {
                        CurrentFolder = sf;
                        this.Addresser.RemoveLast();
                        GetFilesAndFolder(sf);
                    }
                }
                catch { }
            }
            else
            {
                //SideMenu.OpenDrawer();
            }
        }


        #region FileShare
        private void ShareContent(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareStorageItemsHandler);
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
        }

        private void ShareStorageItemsHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "File360";
            request.Data.Properties.Description = "files & folders";

            // Because we are making async calls in the DataRequested event handler,
            // we need to get the deferral first.
            DataRequestDeferral deferral = request.GetDeferral();
            List<IStorageItem> selectedFiles = null;

            // Make sure we always call Complete on the deferral.
            try
            {


                var sdl = SDGridView.SelectedItems;
                foreach (var sdli in sdl)
                {
                    try
                    {
                        var si = (FileModel)sdli;
                        selectedFiles.Add(Items.ElementAt(sd.IndexOf(si)));
                    }
                    catch (Exception ex)
                    {
                        AI("err: ", ex.Message);
                    }
                }

                request.Data.SetStorageItems(selectedFiles);
            }
            finally
            {
                deferral.Complete();
            }
        }
        #endregion
        #endregion


        #region FileSortingMethods

        private void OpenSortMenu(object sender, RoutedEventArgs e)
        {
            OpenSortMenuAnim.Begin();
        }
        private void SortMenuItemClick(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            string buttonName = bt.Content.ToString();
            if (buttonName == "Name")
            {
                Sort(0);
            }
            else if (buttonName == "Type")
            {
                Sort(1);
            }
            else if (buttonName == "Size")
            {
                Sort(2);
            }
            else if (buttonName == "Date")
            {
                Sort(3);
            }
            SortMenu.Visibility = Visibility.Collapsed;
        }

        private void ToggleMenu(Panel element)
        {
            if (element.Visibility == Visibility.Visible)
                element.Visibility = Visibility.Collapsed;
            else element.Visibility = Visibility.Visible;
        }


        public void Sort(int sortType)
        {
            try
            {
                switch (sortType)
                {
                    case 0:
                        sd.OrderBy(o => o.Name);
                        break;
                    case 1:
                        sd.OrderBy(o => o.Image);
                        break;
                    case 2:
                        sd.OrderBy(o => o.Name);
                        break;
                    case 3:
                        sd.OrderBy(o => o.Name);
                        break;
                }
            }
            catch (ArgumentNullException)
            {

            }
        }

        #endregion

        #region FileLister
        public void FileCategorize(IReadOnlyList<IStorageItem> Items, IStorageFolder anyFolder)
        {
            if (Items.Count == 0)
            {
                CurrentFolder = anyFolder;
                headerText.Text = anyFolder.Name;
                switch (anyFolder.Path)
                {
                    case @"D:\Documents":
                        //display documents missing sad face
                        break;
                    case @"D:\Music":
                        //display music missing sad face
                        break;
                    case @"D:\Videos":
                        //display videos missing sad face
                        break;
                    case @"D:\Pictures":
                        //display pictures missing sad face
                        break;
                    default:
                        EmptyFolderStack.Visibility = Visibility.Visible;
                        //default options for other empty folders
                        break;

                }
                return;
            }
            else
            {
                EmptyFolderStack.Visibility = Visibility.Collapsed;
                foreach (IStorageItem Data in Items)
                {
                    if (Data.IsOfType(StorageItemTypes.Folder))
                    {
                        IStorageFolder Folder = Data as IStorageFolder;
                        headerText.Text = anyFolder.Name;
                        //if (CurrentFolder.Path == @"D:\" || CurrentFolder.Path == @"C:\")
                        //{
                        //    if (Folder.Name == "Videos") sd.Add(new FileModel(Folder.Name, "o", ""));
                        //    else if (Folder.Name == "Pictures") sd.Add(new FileModel(Folder.Name, "f", ""));
                        //    else if (Folder.Name == "Music") sd.Add(new FileModel(Folder.Name, "g", ""));
                        //    else if (Folder.Name == "Downloads") sd.Add(new FileModel(Folder.Name, "e", ""));
                        //    else if (Folder.Name == "Documents") sd.Add(new FileModel(Folder.Name, "n", ""));
                        //    else sd.Add(new FileModel(Folder.Name, "h", ""));
                        //}
                        //else 
                        sd.Add(new FileModel(Folder.Name, "h", ""));
                        //SDGridView.ItemsSource = sd;
                        //if (GridType)
                        //    SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["GridViewItemsPanel"];
                        //else SDGridView.ItemsPanel = (ItemsPanelTemplate)Resources["ListViewItemsPanel"];
                    }
                    else if (Data.IsOfType(StorageItemTypes.File))
                    {
                        IStorageFile File = (IStorageFile)Data;
                        string fn = File.Name;
                        string fi = File.FileType;
                        #region FileTypes
                        if (fi == ".mp3")
                        {
                            if (((string)settings.Values["musThumbnail"] == "1"))
                            {

                                #region Mp3

                                //var fileStream = await File.OpenStreamForReadAsync();
                                //var tagFile = TagLib.File.Create(new StreamFileAbstraction(File.Name, fileStream, fileStream));
                                //var tags = tagFile.GetTag(TagTypes.Id3v2);
                                //try
                                //{
                                //    if (tags.Pictures != null)
                                //    {
                                //        MemoryStream ms = new MemoryStream(tags.Pictures[0].Data.Data);
                                //        WriteableBitmap wm = null;
                                //        WriteableBitmap wwm = await wm.FromStream(ms, Windows.Graphics.Imaging.BitmapPixelFormat.Unknown);

                                //        ms.Dispose();
                                //        ImageBrush im = new ImageBrush();
                                //        im.ImageSource = wwm;
                                //        im.Stretch = Stretch.UniformToFill;
                                //        sd.Add(new FileModel(fn, im, ""));
                                //        #endregion
                                //    }
                                //}
                                //catch (IndexOutOfRangeException ex)
                                //{
                                //    BitmapImage bm = new BitmapImage(new Uri("ms-appx:///Assets/IMG-20150528-WA0003.jpg", UriKind.Absolute));
                                //    ImageBrush im = new ImageBrush();
                                //    im.ImageSource = bm;
                                //    sd.Add(new FileModel(fn, im, ""));
                                //}
                                //catch (Exception ex)
                                //{
                                //    AI(ex.Message, 2);
                                //}
                                #endregion
                            }
                            else sd.Add(new FileModel(fn, "m"));

                        }
                        else if (fi == ".wma")
                        {
                            if (((string)settings.Values["musThumbnail"] == "1"))
                            { }
                            #region wma-work in progress

                            //var fileStream = await File.OpenStreamForReadAsync();
                            //var tagFile = TagLib.File.Create(new StreamFileAbstraction(File.Name, fileStream, fileStream));
                            //var tags = tagFile.GetTag(TagTypes.Id3v2);
                            //try
                            //{
                            //    if (tags.Pictures != null)
                            //    {
                            //        MemoryStream ms = new MemoryStream(tags.Pictures[0].Data.Data);
                            //        WriteableBitmap wm = null;
                            //        WriteableBitmap wwm = await wm.FromStream(ms, Windows.Graphics.Imaging.BitmapPixelFormat.Unknown);

                            //        ms.Dispose();
                            //        ImageBrush im = new ImageBrush();
                            //        im.ImageSource = wwm;
                            //        im.Stretch = Stretch.UniformToFill;
                            //        sd.Add(new FileModel(fn, im, ""));
                            //        #endregion
                            //    }
                            //}
                            //catch (IndexOutOfRangeException ex)
                            //{
                            //    BitmapImage bm = new BitmapImage(new Uri("ms-appx:///Assets/IMG-20150528-WA0003.jpg", UriKind.Absolute));
                            //    ImageBrush im = new ImageBrush();
                            //    im.ImageSource = bm;
                            //    sd.Add(new FileModel(fn, im, ""));
                            //}
                            //catch (Exception ex)
                            //{
                            //    AI(ex.Message, 2);
                            //}
                            #endregion
                            else sd.Add(new FileModel(fn, "m"));

                        }
                        else if (fi == ".docx")
                        {
                            sd.Add(new FileModel(fn, "k"));

                        }
                        else if (fi == ".png")
                        {
                            if (((string)settings.Values["picThumbnail"] == "1"))
                            {
                                //await GetThumbnailImageAsync((StorageFile)File, ThumbnailMode.ListView);
                            }
                            else sd.Add(new FileModel(fn, "p"));
                        }
                        else if (fi == ".jpg")
                        {
                            if (((string)settings.Values["picThumbnail"] == "1"))
                            {
                                //await GetThumbnailImageAsync((StorageFile)File, ThumbnailMode.ListView);
                            }
                            else sd.Add(new FileModel(fn, "p"));
                        }
                        else if (fi == ".mp4")
                        {
                            if (((string)settings.Values["vidThumbnail"] == "1"))
                            {
                                try
                                {
                                    // await GetThumbnailImageAsync((StorageFile)File, ThumbnailMode.ListView);
                                }
                                catch
                                {
                                    sd.Add(new FileModel(fn, "v"));
                                }
                            }
                            else sd.Add(new FileModel(fn, "v"));
                        }
                        else if (fi == ".mov")
                        {
                            if (((string)settings.Values["vidThumbnail"] == "1"))
                            {
                                try
                                {
                                    //await GetThumbnailImageAsync((StorageFile)File, ThumbnailMode.ListView);
                                }
                                catch
                                {
                                    sd.Add(new FileModel(fn, "v"));
                                }
                            }
                            else sd.Add(new FileModel(fn, "v"));
                        }
                        else if (fi == ".zip")
                        {
                            sd.Add(new FileModel(fn, "z"));

                        }
                        else if (fi == ".cs")
                        {
                            sd.Add(new FileModel(fn, "c"));
                        }
                        else if (fi == ".pdf")
                        {
                            sd.Add(new FileModel(fn, "b"));
                        }
                        else if (fi == ".vcf")
                        {
                            sd.Add(new FileModel(fn, "l"));
                        }
                        else if (fi == ".doc")
                        {
                            sd.Add(new FileModel(fn, "k"));
                        }
                        else if (fi == ".xlx")
                        {
                            sd.Add(new FileModel(fn, "j"));
                        }
                        else if (fi == ".xlsx")
                        {
                            sd.Add(new FileModel(fn, "j"));
                        }
                        else if (fi == ".7z")
                        {
                            sd.Add(new FileModel(fn, "7"));
                        }
                        else if (fi == ".xml")
                        {
                            sd.Add(new FileModel(fn, "x"));
                        }
                        else if (fi == ".txt")
                        {
                            sd.Add(new FileModel(fn, "a"));
                        }
                        else if (fi == ".rar")
                        {
                            sd.Add(new FileModel(fn, "r"));
                        }
                        else
                        {
                            sd.Add(new FileModel(fn, "i"));

                        }
                        #endregion
                    }
                }
            }
        }


        public async void GetFilesAndFolder(IStorageFolder anyFolder)
        {
            if (anyFolder != null)
            {
                sd.Clear();
                IReadOnlyList<IStorageItem> items = await anyFolder.GetItemsAsync();
                try
                {
                    FileCategorize(items, anyFolder);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                //.Completed = (items, a) =>
                //{

                //FileCategorize(items.GetResults(), anyFolder);

                //Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                //{
                //});

                //};
            }
        }

        #endregion

        #region FileExecution
        private async void ExecuteFile(IStorageFile selectedItem)
        {

            if (selectedItem.FileType == ".zip")
            {
                OpenZip(selectedItem);
            }
            else if (selectedItem.FileType == ".mp3")
            {
                if ((string)settings.Values["musicPlayer"] == "1")
                    PlayMedia(selectedItem);
                else await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
            else if (selectedItem.FileType == ".wma")
            {
                if ((string)settings.Values["musicPlayer"] == "1")
                    PlayMedia(selectedItem);
                else await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
            else if (selectedItem.FileType == ".m4a")
            {
                if ((string)settings.Values["musicPlayer"] == "1")
                    PlayMedia(selectedItem);
                else await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
            else if (selectedItem.FileType == ".mp4")
            {
                if ((string)settings.Values["videosPlayer"] == "1")
                {
                    PlayVideo(selectedItem);
                }
                else await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
            else if (selectedItem.FileType == ".mov")
            {
                if ((string)settings.Values["videosPlayer"] == "1")
                {
                    PlayVideo(selectedItem);
                }
                else await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
            else if (selectedItem.FileType == ".jpg")
            {
                if ((string)settings.Values["picturesPlayer"] == "1")
                {
                    DisplayPhoto(selectedItem);
                }
                else await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
            else
            {
                await Windows.System.Launcher.LaunchFileAsync(selectedItem);
            }
        }

        private void DisplayPhoto(IStorageFile selectedItem)
        {
            throw new NotImplementedException();
        }

        private void PlayMedia(IStorageFile selectedItem)
        {
            throw new NotImplementedException();
        }

        private void PlayVideo(IStorageFile selectedItem)
        {
            this.Frame.Navigate(typeof(VideoPage), selectedItem);
        }

        #endregion
        #region ZipFileHelpers
        public async void OpenZip(IStorageFile compressedFile)
        {
            using (ZipArchive archive = new ZipArchive(await compressedFile.OpenStreamForReadAsync()))
            {
                sd.Clear();
                this.Addresser.Address(compressedFile.Name, compressedFile.Path);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    //if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    entry.ExtractToFile(Path.Combine(extractPath, entry.FullName));
                    //}
                    if (entry.Name.Contains('/'))
                        sd.Add(new FileModel(entry.Name, "f"));
                    else sd.Add(new FileModel(entry.Name, "Q"));
                }
            }



            //try
            //{
            //    using (MemoryStream zipMemoryStream = new MemoryStream(WindowsRuntimeBufferExtensions.ToArray(await FileIO.ReadBufferAsync(compressedFile))))
            //    {
            //        using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
            //        {
            //            var files = zipArchive.Entries;
            //            foreach (var file in files)
            //            {
            //                try
            //                {
            //                    var uncompressedFol = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);
            //                }
            //                catch
            //                {
            //                    var uncompressedFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);
            //                }

            //                return ApplicationData.Current.TemporaryFolder;
            //            }
            //            return null;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    AI("", ex.Message);
            //    return null;
            //}
        }


        public async Task<StorageFile> CreateZip(StorageFile file)
        {
            if (CurrentFolder != null)
            {
                try
                {
                    using (MemoryStream zipMemoryStream = new MemoryStream())
                    {
                        using (ZipArchive zipArchive = new System.IO.Compression.ZipArchive(zipMemoryStream, ZipArchiveMode.Create))
                        {
                            try
                            {
                                byte[] buffer = WindowsRuntimeBufferExtensions.ToArray(await FileIO.ReadBufferAsync(file));
                                ZipArchiveEntry entry = zipArchive.CreateEntry(file.Name);
                                using (Stream entryStream = entry.Open())
                                {
                                    await entryStream.WriteAsync(buffer, 0, buffer.Length);
                                }
                                GetFilesAndFolder(CurrentFolder);
                            }
                            catch (Exception ex)
                            {
                                AI("", ex.Message);
                            }
                        }

                        // Created new file to store compressed files
                        var compressedFileName = file.Name + ".zip";
                        StorageFile zipFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(compressedFileName, CreationCollisionOption.GenerateUniqueName);
                        using (IRandomAccessStream zipStream = await zipFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            // Write compressed data from memory to file
                            using (Stream outstream = zipStream.AsStreamForWrite())
                            {
                                byte[] buffer = zipMemoryStream.ToArray();
                                outstream.Write(buffer, 0, buffer.Length);
                                outstream.Flush();
                                return zipFile;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AI("", ex.Message);
                    return null;
                }
            }
            else
            {
                AI("", "Error! This is not a valid folder");
                return null;
            }
        }
        #endregion

        //// TODO call correctly
        void On_back()
        {
            if (SDGridView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                MultipleSelection(false);
            }
            else if (SortMenu.Visibility == Visibility.Visible)
            {
                SortMenu.Visibility = Visibility.Collapsed;
            }
        }


        private async void GridItemClick(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as FileModel;
            int index = SDGridView.Items.IndexOf(file);
            AI("", index.ToString());
            var selectedItem = await CurrentFolder.GetItemAsync(file.Name);
            if (selectedItem.IsOfType(StorageItemTypes.Folder))
            {
                Addresser.Address(selectedItem.Name, selectedItem.Path);
                CurrentFolder = selectedItem as IStorageFolder;
                GetFilesAndFolder((IStorageFolder)selectedItem);
                return;
            }
            else if (selectedItem.IsOfType(StorageItemTypes.File))
            {
                ExecuteFile((IStorageFile)selectedItem);
                return;
            }
        }

        public void MultipleSelection(bool con)
        {
            switch (con)
            {
                case true:
                    SDGridView.SelectionMode = ListViewSelectionMode.Multiple;
                    SDGridView.ItemClick -= GridItemClick;
                    SDGridView.IsItemClickEnabled = false;
                    operationBar.Visibility = Visibility.Visible;
                    operationBarNormal.Visibility = Visibility.Collapsed;
                    break;

                case false:
                    SDGridView.SelectionMode = ListViewSelectionMode.None;
                    SDGridView.ItemClick += GridItemClick;
                    SDGridView.IsItemClickEnabled = true;
                    operationBar.Visibility = Visibility.Collapsed;
                    operationBarNormal.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ItemHolding(object sender, HoldingRoutedEventArgs e)
        {
            MultipleSelection(true);
        }

        private void FileModelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SDGridView.SelectedIndex == -1)
            {
                MultipleSelection(false);
            }
        }


        #region Search
        private void searchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            try
            {
                var list = sd.Where(x => x.Name.Contains(sender.Text));
                sd.Clear();
                if (list.Count() == 0) return;
                foreach (var item in list)
                {
                    sd.Add(item);
                }
                //foreach (var sdsi in sds)
                //{
                //    sd.Add(sdsi);
                //}
            }
            catch (Exception ex)
            {
                AI("", ex.Message);
                sd.Clear();
            }
        }

        #endregion

        private void FilesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem;
            if (clickedItem is StorageFolder)
            {
                StorageFolder folder = e.ClickedItem as StorageFolder;
                QueryFiles(folder);
            }
            else if (clickedItem is StorageFile)
            {
                Debug.WriteLine((e.ClickedItem as StorageFile).Name);
            }
        }
    }
}
