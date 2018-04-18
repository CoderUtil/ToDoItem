using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    sealed partial class App : Application
    {
        public bool issuspend = false;      //  判断当前页面是否挂起

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;        //  添加挂起事件的委托
        }
        
        protected override void OnLaunched(LaunchActivatedEventArgs e)      //  程序启动时调用
        {
            issuspend = false;  //  处于启动状态

            Frame rootFrame = Window.Current.Content as Frame;      //  rootFrame引用Window.Current.Content as Frame这个对象

            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;   //  添加导航失败事件的委托

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)       //  如果处于暂停状态
                {
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NavigationState"))        //  判断是挂起后重启还是第一次打开
                    {
                        rootFrame.SetNavigationState((string)ApplicationData.Current.LocalSettings.Values["NavigationState"]);
                    }
                }

                // 将框架放在当前窗口中 
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(WholePage), e.Arguments);     //  导航到第一个页面, 第二个参数为要传递的值
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }
        
        
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)     //  导航失败时执行
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        
        private void OnSuspending(object sender, SuspendingEventArgs e)     //  程序挂起时执行
        {
            issuspend = true;       //  处于挂起状态
            var deferral = e.SuspendingOperation.GetDeferral();


            Frame frame = Window.Current.Content as Frame;          
            ApplicationData.Current.LocalSettings.Values["NavigationState"] = frame.GetNavigationState();   //  挂起后, 将值保存在LocalSettings对象中, 以便于重启后恢复

            deferral.Complete();
        }
    }
}
