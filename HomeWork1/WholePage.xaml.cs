using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Todos.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        public static bool narrowToWide = true;

        public WholePage()
        {
            this.InitializeComponent();
            Current = this;

            this.SizeChanged += (sender, e) => {                //  当<Frame name="MainPage">中加载了NewPage且宽度小于800, 此时若扩大宽度至大于800, 会出现两个NewPage. 用于解决这个问题
                if (e.NewSize.Width > 800 && narrowToWide)
                {
                    MainPage.Navigate(typeof(MainPage));    //  刷新MainPage
                    narrowToWide = false;
                }   

                if (e.NewSize.Width < 800)
                {
                    narrowToWide = true;
                }
            };

            MainPage.Navigate(typeof(MainPage));
            NewPage.Navigate(typeof(NewPage));

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)   //  点击下标栏的加号
        {
            MainPage.Navigate(typeof(NewPage));
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame == null) return;
            if (Frame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
    }
}

