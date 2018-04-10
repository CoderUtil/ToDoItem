﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.ViewModels;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.ApplicationModel.DataTransfer;

namespace HomeWork1
{
    public sealed partial class NewPage
    {

        private TodoItemViewModel ViewModel = WholePage.ViewModel;
        public static NewPage Current;

        public bool isUpdate = false;

        public RandomAccessStreamReference ImageStreamRef { get; private set; }

        public NewPage()
        {
            this.InitializeComponent();
            Current = this;

            Uri imageUri = new Uri("ms-appx:///Assets/tv.jpg");         //  图片的共享
            this.ImageStreamRef = RandomAccessStreamReference.CreateFromUri(imageUri);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("NewPage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NewPage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["NewPage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["Title"];
                    detail.Text = (string)composite["Details"];
                    dueDate.Date = (DateTimeOffset)composite["Date"];
                    isUpdate = (bool)composite["isUpdate"];
                    if (isUpdate)
                    {
                        create.Content = "Update";
                        cancel.Content = "Delete";
                    }
                    ApplicationData.Current.LocalSettings.Values.Remove("NewPage");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) // 从该页面离开时
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["Title"] = title.Text;
                composite["Details"] = detail.Text;
                composite["Date"] = dueDate.Date;
                composite["isUpdate"] = isUpdate;
                ApplicationData.Current.LocalSettings.Values["NewPage"] = composite;
                
            }
        }

        private void cancel_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (isUpdate != true)
            {
                create.Content = "Create";
                cancel.Content = "Cancel";
            }
            else
            {
                ViewModel.removeTodoItem();
                isUpdate = false;
                create.Content = "Create";
                cancel.Content = "Cancel";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(WholePage));

                WholePage.Current.tileCreate();           //  创建新的Item, 要更新磁贴
            }
        }


        public void Update()
        {
            create.Content = "Update";
            cancel.Content = "Delete";

            var itemIndex = ViewModel.selectId;
            
            image.Source = ViewModel.allItems[itemIndex].image;
            title.Text = ViewModel.allItems[itemIndex].title;
            detail.Text = ViewModel.allItems[itemIndex].description;
            dueDate.Date = ViewModel.allItems[itemIndex].date.Date;

            isUpdate = true;
        }

        private async void create_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("");

            messageDialog.Commands.Add(new UICommand("Close"));

            messageDialog.DefaultCommandIndex = 0;

            messageDialog.CancelCommandIndex = 1;

            if (title.Text == "")
            {
                messageDialog.Content = "Title is empty!";
                await messageDialog.ShowAsync();
            }
            else if (detail.Text == "")
            {
                messageDialog.Content = "Detail is empty!";

                await messageDialog.ShowAsync();
            }
            else if (dueDate.Date < DateTime.Now.Date)
            {
                messageDialog.Content = "Date is small than current date!";

                await messageDialog.ShowAsync();
            }
            else
            {
                if (isUpdate != true)
                    ViewModel.AddTodoItem(image.Source as BitmapImage, title.Text, detail.Text, dueDate.Date.Date);
                else
                {
                    ViewModel.updateTodoItem(image.Source as BitmapImage, title.Text, detail.Text, dueDate.Date.Date);
                    isUpdate = false;
                    create.Content = "Create";
                    cancel.Content = "Cancel";
                }
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(WholePage));

                WholePage.Current.tileCreate();       //  创建新的Item, 要更新磁贴
            }
        }

        private async void Button_Upload_Image(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                ApplicationData.Current.LocalSettings.Values["image"] = StorageApplicationPermissions.FutureAccessList.Add(file);

                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
                    bitmapImage.SetSource(stream);
                    image.Source = bitmapImage;
                }
                catch (Exception)
                {
                    MessageDialog msg = new MessageDialog("发生了些小问题，稍后试试吧", "Oops!");
                    await msg.ShowAsync();
                }
            }
        }
    }
}
