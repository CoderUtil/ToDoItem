using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Todos.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace HomeWork1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WholePage : Page
    {

        public static WholePage Current;
        public static TodoItemViewModel ViewModel = new TodoItemViewModel();

        public RandomAccessStreamReference ImageStreamRef { get; private set; }

        public WholePage()
        {
            this.InitializeComponent();
            Current = this;
            

            MainPageFrame.Navigate(typeof(MainPage));
            NewPageFrame.Navigate(typeof(NewPage));

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            tileCreate();   //  调用创建磁贴的函数

            Uri imageUri = new Uri("ms-appx:///Assets/tv.jpg");         //  图片的共享
            this.ImageStreamRef = RandomAccessStreamReference.CreateFromUri(imageUri);
        }

        public void tileCreate()       //  创建磁贴
        {
            Windows.Data.Xml.Dom.XmlDocument document = new Windows.Data.Xml.Dom.XmlDocument();
            document.LoadXml(System.IO.File.ReadAllText("XMLFile1.xml"));
            var Texttitle = document.GetElementsByTagName("text");

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            for (int i = 0; i < ViewModel.AllItems.Count; i++)
            {
                if (i < 5)
                {
                    Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[4].InnerText = ViewModel.AllItems[i].title;
                    Texttitle[1].InnerText = Texttitle[3].InnerText = Texttitle[5].InnerText = ViewModel.AllItems[i].description;
                    TileNotification newTile = new TileNotification(document);
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(newTile);
                }
            }
        }

        private void addButton(object sender, RoutedEventArgs e)   //  点击下标栏的加号
        {
            Frame.Navigate(typeof(NewPage));
        }

        private async void shareButton(object sender, RoutedEventArgs e)
        {

            var messageDialog = new MessageDialog("");

            messageDialog.Commands.Add(new UICommand("Close"));

            messageDialog.DefaultCommandIndex = 0;

            messageDialog.CancelCommandIndex = 1;

            if (ViewModel.selectId == -1)
            {
                messageDialog.Content = "Please choose an item!";
                await messageDialog.ShowAsync();
            }
            else
            {
                DataTransferManager.ShowShareUI();
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            }
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            
            request.Data.Properties.Title = ViewModel.AllItems[ViewModel.selectId].title;
            request.Data.SetText(ViewModel.AllItems[ViewModel.selectId].description);
            request.Data.SetBitmap(ImageStreamRef);
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) return;
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }
        

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            ViewModel.selectId = -1;    //  刚进这个页面, 之前选中的不见了

            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("WholePage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values["image"] != null)
                {
                    StorageFile temp;
                    temp = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["image"]);
                    IRandomAccessStream ir = await temp.OpenAsync(FileAccessMode.Read);
                    BitmapImage bi = new BitmapImage();
                    await bi.SetSourceAsync(ir);
                    NewPage.Current.image.Source = bi;
                    ApplicationData.Current.LocalSettings.Values["image"] = null;
                }


                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("WholePage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["WholePage"] as ApplicationDataCompositeValue;

                    for (int i = 0; i < ViewModel.AllItems.Count(); i++)
                    {
                        ViewModel.AllItems[i].completed = (bool)composite["ischecked" + i];
                    }

                    NewPage.Current.title.Text = (string)composite["Title"];
                    NewPage.Current.detail.Text = (string)composite["Details"];
                    NewPage.Current.dueDate.Date = (DateTimeOffset)composite["Date"];
                    NewPage.Current.isUpdate = (bool)composite["isUpdate"];
                    if (NewPage.Current.isUpdate)
                    {
                        NewPage.Current.create.Content = "Update";
                        NewPage.Current.cancel.Content = "Delete";
                    }
                    ApplicationData.Current.LocalSettings.Values.Remove("WholePage");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) // 从该页面离开时
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["Title"] = NewPage.Current.title.Text;
                composite["Details"] = NewPage.Current.detail.Text;
                composite["Date"] = NewPage.Current.dueDate.Date;
                composite["isUpdate"] = NewPage.Current.isUpdate;

                for (int i = 0; i < ViewModel.AllItems.Count(); i++)
                {
                    composite["ischecked" + i] = ViewModel.AllItems[i].completed;
                }
                ApplicationData.Current.LocalSettings.Values["WholePage"] = composite;

            }
        }
    }
}

