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
using Todos.DataBaseModels;
using Windows.UI.Popups;
using System.Text;

namespace HomeWork1
{
    public sealed partial class MainPage : Page
    {

        public TodoItemViewModel ViewModel = WholePage.ViewModel;       
        public static MainPage Current;         //  MainPage的单例对象
        public int SelectItem = 0;              //  点击选定的Item
        

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

        private void ItemSelect(object sender, SelectionChangedEventArgs e)     //  点击Item时获取选定Item的事件
        {
            ViewModel.selectId = list.SelectedIndex;       //  获取选中的item的标号, 存放在ViewModel中
            if (Window.Current.Bounds.Width < 800)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage));
            }
            NewPage.Current.Update();
        }

        private void clickCheckBox(object sender, RoutedEventArgs e)
        {
            TodoItemDataBase.updateCompleted();
        }
        
        private async void clickSearchButton(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("");

            messageDialog.Commands.Add(new UICommand("Close"));

            messageDialog.DefaultCommandIndex = 0;

            messageDialog.CancelCommandIndex = 1;

            if (searchMessage.Text == "")
            {
                messageDialog.Content = "Please input the message for search";
            }
            else
            {
                StringBuilder result = TodoItemDataBase.query(searchMessage.Text);
                messageDialog.Content = result.ToString();
            }

            await messageDialog.ShowAsync();
        }
    }
}
