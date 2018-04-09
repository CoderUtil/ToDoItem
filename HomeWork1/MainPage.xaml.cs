using Todos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Todos.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;
using System.Diagnostics;
using Windows.Storage;

namespace HomeWork1
{
    public sealed partial class MainPage : Page
    {

        public TodoItemViewModel ViewModel = WholePage.ViewModel;
        public static MainPage Current;     //  给每个Page创建一个单例对象.
        public int SelectItem = 0;
        

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;         
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void ItemSelect(object sender, SelectionChangedEventArgs e)
        {
            SelectItem = list.SelectedIndex;            //  获取选中的item的标号
            if (Window.Current.Bounds.Width < 800)
            {
                this.Frame.Navigate(typeof(NewPage));
            }
            NewPage.Current.Update(SelectItem);
        }
    }
}
