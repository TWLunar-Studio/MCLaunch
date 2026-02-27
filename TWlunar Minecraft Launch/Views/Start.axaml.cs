using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TWlunar_Minecraft_Launch.ViewModels;
using TWlunar_Minecraft_Launch.Views;

namespace TWlunar_Minecraft_Launch.Views;

public partial class Start : UserControl
{
    public Start()
    {
        InitializeComponent();
        DataContext = new StartViewModel();
    }

    private void GoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //启动当前所选版本
    }

    private void VersionListButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("VersionListButton_OnClick 被调用");
        var parent = this.Parent;
        while (parent != null)
        {
            if (parent is MainWindow mainWindow)
            {
                Console.WriteLine("找到 MainWindow");
                // 使用默认高度比例 1/3
                mainWindow.OpenBorder(1.0 / 3.0);
                var viewModel = mainWindow.DataContext as MainWindowViewModel;
                if (viewModel != null)
                {
                    Console.WriteLine("设置 CurrentPage 为 VersionListViewModel");
                    viewModel.CurrentPage = new VersionListViewModel();
                }
                else
                {
                    Console.WriteLine("MainWindow.DataContext 为 null");
                }

                break;
            }

            parent = (parent as Control)?.Parent;
        }

        if (parent == null)
        {
            Console.WriteLine("未找到 MainWindow");
        }
    }

    private void VersionSettingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("VersionSettingButton_OnClick 被调用");
        
        // 查找 MainWindow 并打开配置界面
        var parent = this.Parent;
        while (parent != null)
        {
            if (parent is MainWindow mainWindow)
            {
                // 创建配置 ViewModel
                var configViewModel = new VersionSettingViewModel();
                    
                // 打开 Border，使用全屏覆盖（heightRatio = 1.0）
                mainWindow.OpenBorder(1.0);
                    
                // 设置 CurrentPage
                var viewModel = mainWindow.DataContext as MainWindowViewModel;
                if (viewModel != null)
                {
                    viewModel.CurrentPage = configViewModel;
                }
                break;
            }
            parent = (parent as Control)?.Parent;
        }
    }
}