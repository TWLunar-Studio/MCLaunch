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
    }

    private void GoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
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
}