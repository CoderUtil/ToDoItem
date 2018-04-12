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

        public static WholePage Current;        //  WholePage的单例对象
        public static TodoItemViewModel ViewModel = new TodoItemViewModel();        //  ViewModel存储Item数据.所有页面共享一份数据

        public WholePage()
        {
            this.InitializeComponent();
            Current = this;         
            

            MainPageFrame.Navigate(typeof(MainPage));
            NewPageFrame.Navigate(typeof(NewPage));

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;       //  创建后退Button

            tileCreate();   //  调用创建磁贴的函数
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

        private void addButton(object sender, RoutedEventArgs e)   //  点击下标栏的加号, 跳转到NewPage页面
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(NewPage));
        }
        

        protected override async void OnNavigatedTo(NavigationEventArgs e)      //  导航到当前页面
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;      //  设置后退按钮的可视性. 当可后退时设为可见

            ViewModel.selectId = -1;    //  刚进这个页面, 重置所选Item

            if (e.NavigationMode == NavigationMode.New)         //  重启时恢复挂起前数据的
            {
                ApplicationData.Current.LocalSettings.Values.Remove("WholePage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values["image"] != null)      //  恢复挂起前的图片
                {
                    StorageFile temp = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["image"]);
                    IRandomAccessStream ir = await temp.OpenAsync(FileAccessMode.Read);
                    BitmapImage bi = new BitmapImage();
                    await bi.SetSourceAsync(ir);
                    NewPage.Current.image.Source = bi;
                    ApplicationData.Current.LocalSettings.Values["image"] = null;
                }


                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("WholePage"))      //  恢复挂起前的其他内容
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["WholePage"] as ApplicationDataCompositeValue;

                    for (int i = 0; i < ViewModel.AllItems.Count(); i++)
                    {
                        ViewModel.AllItems[i].completed = (bool)composite["ischecked" + i];     //  恢复挂起前Item的checkBox值
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

        protected override void OnNavigatedFrom(NavigationEventArgs e) // 导航离开该页面
        {
            bool suspending = ((App)App.Current).issuspend;     //  挂起时保存数据
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

